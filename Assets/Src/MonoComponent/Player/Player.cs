using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using GameAddressables;
using Src.Data;
using Src.MonoComponent;
using Src.Services;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Get() => _instance;

    public IPlayerData Data => Main.Services.Data;
    public Transform Graphic;
    public CinemachineSmoothPath GetItemTrack;
    public bool BlockControls { get; set; }
    private float _speed = 5f;
    private Vector3 _lastMove = Vector3.zero;
    private Coroutine _routineAnimation;
    private LivingEntity _entity;
    private PlayerAnimation _anim;
    private PlayerWorldInteraction _interaction;
    private CharacterController _controller;
    private Quaternion _inputRotation;
    private List<Vector3> _lastPositions = new(10);
    private Ray _stepCast = new Ray();
    private Ground _floor;
    
    public ref Vector3 LastMoveDirection => ref _lastMove;
    public LivingEntity Entity => _entity;
    public PlayerWorldInteraction WorldInteraction => _interaction;
    public Collider Collider => _entity.Collider;
    public IReadOnlyCollection<Vector3> LastPositions => _lastPositions;
    public PlayerAnimation Animation => _anim;
    public bool Loaded { get; private set; }
    public Ground Floor => _floor;
    
    void Awake()
    {
        _instance = this;
    }
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _entity = GetComponent<LivingEntity>();
        _anim = GetComponent<PlayerAnimation>();
        _interaction = GetComponent<PlayerWorldInteraction>();
        _entity.OnSpawn += OnSpawn;
        CalculateInputRotation();
    }
    
    public void CalculateInputRotation()
    {
        _inputRotation = Quaternion.Euler(0, 45, 0);
        //_inputRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }

    private void OnSpawn(LivingEntity entity)
    {
        _entity = entity;
        _entity.AttackerEntity.OnBeginAttack += OnBeginAttack;
        _entity.AttackableEntity.OnAttacked += OnAttacked;
        LoadPlayerData();
        Map.Current.TriggerPlayerInitialized();
        Loaded = true;
    }

    private void LoadPlayerData()
    {
        _entity.Stats.Data = Data.Stats();
        _entity.Equips.SetCurrentWeapon(Data.CurrentWeapon());
        if (Map.Current.AllowSavingPosition)
        {
            if (Data.LastEntrance() != Vector3.zero)
            {
                transform.position = Data.LastEntrance();
            }
            else if (Data.PlayerPosition() != Vector3.zero)
            {
                transform.position = Data.PlayerPosition();
            }
        }
    }
    
    void Update()
    {
        if (Main.Services.Map.GameFrozen) return;
        TryKeepWeapon();
    }

    private void OnAttacked(DamageDealer dealer)
    {
        Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Damage_taken);
        var target = dealer.Owner.transform.position;
        Graphic.transform.LookAt(new Vector3(target.x, Graphic.position.y, target.z));
        _anim.Play(CharacterAnimation.hit);
        Main.Services.Vfx.Play(VfxPrefab.CfxR3HitMiscA, _entity.Collider.bounds.center, fx =>
        {
            fx.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        });
        var blinker = gameObject.GetOrAddComponent<Blinker>();
        blinker.StartBlink(_entity.AttackableEntity.InvulnerabilitySeconds);
        var direction = -Graphic.forward;
        _entity.PlayingSequence = transform.DOJump(transform.position + new Vector3(direction.x, 0, direction.z), 0.01f, 1, 0.4f).SetAutoKill(true);
    }

    private void OnBeginAttack(long attackFrame)
    {
        if(_lastMove.x != 0 || _lastMove.y != 0) Graphic.rotation = Quaternion.LookRotation(_lastMove);
    }

    private void TryKeepWeapon()
    {
        if (_routineAnimation == null && Main.Services.Map.Aggro.Count == 0 && !_entity.Equips.IsWeaponKept &&
            _entity.AttackerEntity.LastAttack + TimeSpan.FromSeconds(3) < DateTime.UtcNow)
        {
            _routineAnimation = StartCoroutine(KeepWeapon(_entity.Equips.CurrentWeapon));
        }
    }

    private IEnumerator KeepWeapon(Weapon activeWeapon)
    {
        _anim.LockAnimations = true;
        _anim.Play(CharacterAnimation.putweapon);
        yield return new WaitForSeconds(0.4f);
        if(!_entity.Equips.IsWeaponKept)
            _entity.Equips.KeepWeapon();
        _anim.LockAnimations = false;
        _routineAnimation = null;
    }
    
    public void MoveDirection(Vector3 dir)
    {
        if (_entity.AttackerEntity.IsAttacking() || _entity.IsPlayingSequence)  return;
        dir  = (_inputRotation * dir).normalized;

        if (dir != Vector3.zero && !_entity.AttackerEntity.IsAttacking())
        {
            Graphic.rotation = Quaternion.RotateTowards(Graphic.rotation, Quaternion.LookRotation(dir), 800f * Time.deltaTime);
        }

        var move = dir * _speed;
        var predicted = transform.position + _controller.center + (move * Time.fixedDeltaTime * 5f);
        _stepCast.origin = predicted;
        _stepCast.direction = Vector3.down;
      
        if (!Physics.Raycast(_stepCast, out var hit, 1 + _controller.stepOffset))
        {
            _controller.velocity.Set(0,0,0);
            _controller.SimpleMove(Vector3.zero);
            return;
        }
        else
        {
            if(hit.transform.gameObject.IsGround())
                _floor = hit.transform.gameObject.GetComponent<Ground>();
        }
        _lastMove = move;
        _controller.SimpleMove(move);
      
        if (Map.Current.AllowSavingPosition)
        {
            if (_lastMove != Vector3.zero && Time.frameCount % 5 == 0)
            {
                _lastPositions.Insert(0, transform.position);
                if (_lastPositions.Count > 10) _lastPositions.RemoveAt(10);
            }
        }
        Map.Current.TriggerPlayerMoved();
    }
}
