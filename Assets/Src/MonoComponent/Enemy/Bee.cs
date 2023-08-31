using System;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using GameAddressables;
using Unity.VisualScripting;
using UnityEngine;

public class Bee : MonoBehaviour
{
    private Monster _monster;
    private LivingEntity _entity;
    private Animator _animator;

    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int Forward = Animator.StringToHash("forward");

    void Start()
    {
        _monster = GetComponent<Monster>();
        _entity = GetComponent<LivingEntity>();
        _animator = GetComponent<Animator>();
        _entity.OnSpawn += OnSpawned;
        _entity.OnStunEnd += OnStunEnd;
        _animator.SetTrigger(Idle);
    }

    private void OnSpawned(LivingEntity e)
    {
        e.AttackableEntity.OnAttacked += OnAttacked;
        e.Stats.MaxLife = 1;
        e.Stats.Life = 1;
    }
    
    private void OnStunEnd()
    {
        if (_entity.ZeroLife) _entity.Die();
    }

    private void OnAttacked(DamageDealer attacker)
    {
        _animator.speed = 0;
        _monster.EnableSomePhysics();
        _monster.Body.isKinematic = false;
        if(_entity.IsPlayingSequence) _entity.PlayingSequence.Kill();
        var baseVector = (transform.position - attacker.transform.position).normalized;
        _monster.Body.velocity = Vector3.zero;
        _monster.Body.angularVelocity = Vector3.zero;
        var force = new Vector3(baseVector.x * 4, 10, baseVector.z * 4);
        _monster.Body.AddForce(force , ForceMode.VelocityChange);
        _monster.Body.AddTorque(new Vector3(force.x * 9, 0, 0), ForceMode.VelocityChange);
        _entity.Stun(TimeSpan.FromSeconds(1));
    }
    
    void FixedUpdate()
    {
        if (!_monster.SeeingPlayer)
        {
            _animator.SetTrigger(Idle);
            return;
        }
        if (_entity.PlayingSequence != null && _entity.PlayingSequence.IsPlaying()) return;
        if (_entity.IsStunned) return;
        Chase();
    }

    private void Chase()
    {
        var pos = transform.position;
        var direction = (Player.Get().Entity.Center - pos);
        direction.Set(direction.x, 0, direction.z);
        _monster.Body.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 360f * Time.deltaTime);
        _monster.Body.MovePosition(pos + (transform.forward * 2 * Time.fixedDeltaTime));
        _animator.SetTrigger(Forward);
    }
}
