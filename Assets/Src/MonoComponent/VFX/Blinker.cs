using System;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    private DateTime _endTime = DateTime.MinValue;
    private readonly TimeSpan _blinkDelay = TimeSpan.FromSeconds(0.08f);
    private DateTime _nextBlink;
    private bool _blinking = false;
    private Renderer [] _renderers;
    private readonly Color _blinkColor = new Color(0.7f, 0.6f, 0.6f);
    private Color [] _originals;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    public void StartBlink(float durationSeconds)
    {
        if (_endTime != DateTime.MinValue)
        {
            Reset();
        }
        _endTime = DateTime.UtcNow + TimeSpan.FromSeconds(durationSeconds);
        _nextBlink = DateTime.UtcNow;
        _renderers = GetComponentsInChildren<Renderer>();
        _originals = new Color[_renderers.Length];
        for (var i = 0; i < _renderers.Length; i++)
        {
            _originals[i] = _renderers[i].material.color;
        }
    }

    private void SetColor(Renderer r, Color c)
    {
        r.material.SetColor(BaseColor, c);
    }

    private void Blink()
    {
        _nextBlink = DateTime.UtcNow + _blinkDelay;
        if (!_blinking)
        {
            foreach (var m in _renderers)
            {
                SetColor(m, _blinkColor);
            }
        }
        else
        {
            for (var i = 0; i < _renderers.Length; i++)
                SetColor(_renderers[i], _originals[i]);
        }
        _blinking = !_blinking;
    }

    // Update is called once per frame
    void Update()
    {
        if (_endTime == DateTime.MinValue) return;
        if (_nextBlink > DateTime.UtcNow) return;
        Blink();
        if (DateTime.UtcNow > _endTime)
        {
            Reset();
        }
    }

    void Reset()
    {
        for (var i = 0; i < _renderers.Length; i++)
        {
            SetColor( _renderers[i], _originals[i]);
        }
        _endTime = DateTime.MinValue;
    }
}
