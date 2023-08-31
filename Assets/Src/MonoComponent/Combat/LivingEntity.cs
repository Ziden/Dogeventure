using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameAddressables;
using Src.Data;
using Src.MonoComponent;
using Src.Services;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class LivingEntity : MonoBehaviour
{
    private event Action<LivingEntity> _onSpawn;
    public event Action OnDie;
    public Action OnStun;
    public Action OnStunEnd;
    public Action<Sequence> OnPlaySequence;
    
    public Stats Stats = new();
    
    public Animator Animator => _animator;
    private InventoryHolder _equips;
    private Animator _animator;
    private AttackerEntity _attackerEntity;
    private AttackableEntity _attackableEntity;
    private Sequence _playingSequence;
    private DateTime _stunnedUntil;
    private Collider _collider;
    private bool _spawned;

    public AttackableEntity AttackableEntity => _attackableEntity;
    public AttackerEntity AttackerEntity => _attackerEntity;
    public InventoryHolder Equips => _equips;
    public Collider Collider => _collider;
    public event Action<LivingEntity> OnSpawn
    {
        add
        {
            _onSpawn += value;
            if (_spawned)
            {
                _onSpawn?.Invoke(this);
            }
        }
        remove => _onSpawn -= value;
    }
    
    public void Die()
    {
        OnDie?.Invoke();
        Main.Services.Vfx.Play(VfxPrefab.CfxR2WwExplosion, transform.position,
            e => e.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f));
        Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Low_impactwav_14905, 2, 0.5f);
        //Main.Audio.PlaySoundEffect(AssetSoundEffect.Explosion_6055, 2, 0.5f);
        Destroy(gameObject);
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _attackerEntity = GetComponent<AttackerEntity>();
        _attackableEntity = GetComponent<AttackableEntity>();
        _equips = GetComponent<InventoryHolder>();
        _collider = GetComponent<Collider>();
        _onSpawn?.Invoke(this);
        _spawned = true;

        GLog.Debug("Registered OnAttacked for " + gameObject.name);
        _attackableEntity.OnAttacked += OnAttacked;
    }

    public bool ZeroLife => Stats.Life <= 0;

    public bool IsStunned => Main.Services.Map.GameFrozen || DateTime.UtcNow < _stunnedUntil;

    public Vector3 Center => _collider.bounds.center;

    public bool IsPlayingSequence =>
        _playingSequence != null && _playingSequence.active && _playingSequence.IsPlaying();

    public void Stun(TimeSpan duration)
    {
        _stunnedUntil = DateTime.UtcNow + duration;
        OnStun?.Invoke();
    }

    private void OnAttacked(DamageDealer dealer)
    {
        Stats.Life -= dealer.Damage;
        GLog.Debug($"{gameObject.name} lost {dealer.Damage} life keeping {Stats.Life}");
    }
	
    public Sequence PlayingSequence
    {
        get => _playingSequence;
        set
        {
            PrepareSequence(value);
            _playingSequence = value;
            OnPlaySequence?.Invoke(value);
            value.Play();
        }
    }
    
    private void PrepareSequence(Sequence seq)
    {
        seq.SetAutoKill(true);
    }

    void Update()
    {
        if (_stunnedUntil != DateTime.MinValue && !IsStunned)
        {
            _stunnedUntil = DateTime.MinValue;
            OnStunEnd?.Invoke();
        }
    }
}
