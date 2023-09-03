using CartoonFX;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Src.MonoComponent;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum DoorState
{
    Closed, Opening, Open
}

public enum AnimationType
{
    Interpolate
}


public class Door : MonoBehaviour, ISignalActionable
{
    public event Action OnOpened;

    public ParticleSystem[] OpeningEffects;  
    public GameObject[] DoorParts;
    public Vector3[] OpenOffsets;
    public ParticleSystem OpenedEffect;
    public Ease AnimationEase = Ease.Linear;
    public AnimationType AnimationType = AnimationType.Interpolate;
    public float Duration = 3;

    private DoorState _state;
    private Sequence _animations;

    public void Open()
    {
        if (_state != DoorState.Closed) return;
        _animations = DOTween.Sequence();
        for (var x = 0; x < DoorParts.Length; x++) {
            OpenAnimation(DoorParts[x], OpenOffsets[x]);
        }

        _animations
            .SetEase(AnimationEase)
            .OnStart(OnBeginOpening)
            .OnComplete(OnFinishOpening)
            .SetAutoKill(true)
            .Play();
    }

    private void OpenAnimation(GameObject doorPart, Vector3 position)
    {
        if(AnimationType == AnimationType.Interpolate) _animations.Join(doorPart.transform.DOMove(doorPart.transform.position + position, Duration));
    }

    private void OnBeginOpening()
    {
        foreach (var o in OpeningEffects)
        {
            o.time = 0;
            var main = o.main;
            main.duration = Duration * 0.75f;
            o.gameObject.SetActive(true);
            o.Play();
        }
        if (OpenedEffect != null)
        {
            OpenedEffect.Stop();
            OpenedEffect.time = 0;
        }
        _state = DoorState.Opening;
    }

    private void OnFinishOpening()
    {
        foreach (var o in OpeningEffects)
        {
            o.Stop();
            o.gameObject.SetActive(false);
        }
        if(OpenedEffect != null)
        {
            OpenedEffect.time = 0;
            OpenedEffect.Play();
        }
        _state = DoorState.Open;
        OnOpened?.Invoke();
    }

    public void OnAction()
    {
        this.Open();
    }

    public void OnSignal()
    {
        Open();
    }
}