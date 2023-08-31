using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DG.Tweening;
using GameAddressables;
using Src;
using Src.MonoComponent;
using UnityEngine;

public class Slime : MonoBehaviour
{
    private Monster _monster;
    private LivingEntity _entity;
    
    private static readonly int WalkFwd = Animator.StringToHash("WalkFWD");
    public float JumpHeight = 1f;
    public float JumpTime = 1f;
    public float JumpCooldownSeconds = 0f;

    private Cooldown _collideCd = new ()
    {
        Delay = TimeSpan.FromSeconds(0.5f)
    };
    private DateTime NextJump;

    private bool _jumping = false;
    void Start()
    {
        _monster = GetComponent<Monster>();
        _entity = GetComponent<LivingEntity>();
        NextJump = DateTime.UtcNow;
        _entity.OnSpawn += OnSpawned;
    }

    private void OnSpawned(LivingEntity e)
    {
        e.AttackableEntity.OnAttacked += OnAttacked;
        e.OnStunEnd += OnStunEnd;
        e.Stats.MaxLife = 3;
        e.Stats.Life = 3;
    }

    private void StandUp()
    {
        var rot = _monster.Body.rotation.eulerAngles;
        _monster.Body.MoveRotation(Quaternion.Euler(0, rot.y, 0));
        _monster.Body.angularVelocity = Vector3.zero;
        _monster.Body.velocity = Vector3.zero;
        _monster.Body.AddForce(new Vector3(0, 5, 0), ForceMode.VelocityChange);
        Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Slime3, 0.7f, 1);
    }

    private void OnStunEnd()
    {
        if (_entity.ZeroLife) 
            _entity.Die();
        else 
            StandUp();
    }

    private void OnAttacked(DamageDealer attacker)
    {
        _monster.EnableSomePhysics();
        _monster.Body.isKinematic = false;
        if(_entity.IsPlayingSequence) _entity.PlayingSequence.Kill();
        transform.localScale = new Vector3(1, 1, 1);
        var baseVector = (transform.position - attacker.transform.position).normalized;
        _monster.Body.velocity = Vector3.zero;
        _monster.Body.angularVelocity = Vector3.zero;
        var force = new Vector3(baseVector.x * 3, 6, baseVector.z * 3);
        _monster.Body.AddForce(force , ForceMode.VelocityChange);
        _monster.Body.AddTorque(new Vector3(force.x * 9, 0, 0), ForceMode.VelocityChange);
        _entity.Stun(TimeSpan.FromSeconds(1));
        NextJump = DateTime.UtcNow + TimeSpan.FromSeconds(1.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_collideCd.IsCooldown()) return;
        if (!_entity.IsStunned) return;
        if (collision.gameObject.IsStaticMap())
        {
            _collideCd.Trigger();
            Main.Services.Vfx.Play(VfxPrefab.CfxR3HitMiscA, collision.contacts.First().point);
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Slime3, 1.2f, 1);
        } else if (collision.gameObject.IsGround() && _entity.IsStunned)
        {
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Slime3, 1.2f, 1);
        }
    }

    void FixedUpdate()
    {
        if (!_monster.SeeingPlayer)     return;
        if (_entity.PlayingSequence != null && _entity.PlayingSequence.IsPlaying()) return;
        if (DateTime.UtcNow < NextJump)  return;
        if (!_monster.IsOnGround) return;
        if (_entity.IsStunned) return;
        Jump();
    }
    
    private void Jump()
    {
        var pos = transform.position;
        var player = Player.Get();
        var direction = (player.Entity.Center - pos).normalized * 2;
        _monster.DisableSomePhysics();
        transform.LookAt(new Vector3(player.Entity.Center.x, pos.y, player.Entity.Center.z));
        var rot = transform.rotation.eulerAngles;
        _monster.Body.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, rot.y, 0);

        var seq = DOTween.Sequence();
        seq.Append(transform.DOScaleY(0.2f, 0.3f));
        seq.Append(transform.DOScaleY(1.1f, 0.1f));
        seq.Join(transform.DOJump(pos + direction, JumpHeight, 1, JumpTime));
        seq.OnComplete(() =>
        {
            transform.DOScaleY(1f, 0.3f);
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Slime3, 0.7f, 1);
        });
        _entity.PlayingSequence = seq; 
        NextJump = DateTime.UtcNow + TimeSpan.FromSeconds(JumpCooldownSeconds);
    }
}
