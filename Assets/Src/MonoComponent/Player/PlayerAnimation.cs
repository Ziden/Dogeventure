using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using GameAddressables;
using Src;
using Src.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public enum CharacterAnimation
{
    idle, attack, attack2, run, runweapon, hit, getweapon, putweapon,
    raisehands, openchest, landing, lift
}

public enum FaceExpression
{
    Happy, Sad, Neutral
}

public class PlayerAnimation : MonoBehaviour
{
    public GameObject Sad;
    public GameObject Happy;
    public GameObject Neutral;

    public Transform RightFeet;
    public Transform LeftFeet;
    
    private bool _leftHit;
    public bool LockAnimations;
    private Animator _animator;
    private Player _player => Player.Get();
    private PlayerSteps _steps;

    private CharacterAnimation _current;
    public float AttackAnimationSpeed { get; private set; }
    
    public CharacterAnimation Current => _current;
    public bool HasWeapon => _player.Entity.Equips.CurrentWeapon != null;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        Map.Current.OnPlayerInteracted += OnInteract;
        Map.Current.OnPlayerInitialized += OnPlayerInitialized;
        Map.Current.OnPlayerMoved += OnPlayerMoved;
    }

    private void StepLeft()
    {
        if (_player.Floor != null)
        {
            Main.Services.Audio.PlaySoundEffect(_player.Floor.StepSound, 0.7f + Random.value / 2, 0.09f);
            Main.Services.Vfx.Play(VfxPrefab.StepStone, LeftFeet.transform.position, 0.5f);
        }
        else
        {
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.StepGrass, 0.7f + Random.value / 2, 0.09f);
            Main.Services.Vfx.Play(VfxPrefab.StepStone, LeftFeet.transform.position, 0.5f);
        }
    }

    private void StepRight()
    {
        if (_player.Floor != null)
        {
            Main.Services.Audio.PlaySoundEffect(_player.Floor.StepSound, 0.7f + Random.value / 2, 0.09f);
            Main.Services.Vfx.Play(VfxPrefab.StepStone, RightFeet.transform.position, 0.5f);
        }
        else
        {
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.StepGrass, 0.7f + Random.value / 2, 0.09f);
            Main.Services.Vfx.Play(VfxPrefab.StepStone, RightFeet.transform.position, 0.5f);
        }
    }

    void OnPlayerMoved()
    {
        if (LockAnimations || IsPlaying(CharacterAnimation.attack) || IsPlaying(CharacterAnimation.attack2)) return;
        if (_player.LastMoveDirection.x != 0 || _player.LastMoveDirection.y != 0) Play(CharacterAnimation.runweapon);
        else Play(CharacterAnimation.idle);
    }

    public void ChangeExpression(FaceExpression expression)
    {
        Sad.gameObject.SetActive(expression==FaceExpression.Sad);
        Happy.gameObject.SetActive(expression==FaceExpression.Happy);
        Neutral.gameObject.SetActive(expression==FaceExpression.Neutral);
    }

    
    private void PlayWithSpecificTime(CharacterAnimation a, float seconds)
    {
        Play(a);
    }

    void OnPlayerInitialized(Player p)
    {
        _steps = GetComponentInChildren<PlayerSteps>();
        _steps.StepRight += StepRight;
        _steps.StepLeft += StepLeft;
        p.Entity.AttackerEntity.OnBeginAttack += OnBeginAttack;
    }

    private void OnInteract(Interactible interactible)
    {
        var chest = interactible.GetComponent<Chest>();
        if (chest != null)
        {
            StartCoroutine(this.PlayOpenChestSequence(chest));
        }
    }

    public bool IsPlaying(CharacterAnimation a)
    {
        var currentState = _animator.GetCurrentAnimatorStateInfo(0);
        return currentState.IsName(a.ToString()) && (currentState.normalizedTime <= 0 || currentState.normalizedTime >= 1);
    }
    
    private void OnBeginAttack(long attackFrame)
    {
        var anim = CharacterAnimation.attack;
        var v = Random.value;
        if(v < 0.33) Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Swing);
        else if(v < 0.66) Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Swing2);
        else Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Swing3);
        if (HasWeapon)
        {
            if (DateTime.UtcNow < _player.Entity.AttackerEntity.LastAttack + TimeSpan.FromSeconds(_player.Entity.Equips.CurrentWeapon.WeaponData.AttackDurationSeconds + 0.15f))
            {
                if (_leftHit)
                {
                    _leftHit = false;
                    anim = CharacterAnimation.attack2;
                }
            } 
            if(anim == CharacterAnimation.attack) _leftHit = true;
        }
        Play(anim, noBlend:true);
        AttackAnimationSpeed = _animator.GetCurrentAnimatorStateInfo(0).speed;
    }

    
    public void Play(CharacterAnimation a, bool noBlend = false)
    {
        if (a == _current || LockAnimations) return;
        _animator.SetBool(_current.ToString(), false);
        _animator.SetBool(a.ToString(), true);
        _current = a;
        if(noBlend)  _animator.Play(a.ToString(), 0, 0);
    }
}
