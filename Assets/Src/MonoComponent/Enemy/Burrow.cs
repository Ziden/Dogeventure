using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DG.Tweening;
using GameAddressables;
using Src;
using Src.MonoComponent;
using Unity.VisualScripting;
using UnityEngine;

public class Burrow : MonoBehaviour
{
    private Monster _monster;
    private LivingEntity _entity;
    private Animator _animator;
    private float _ace = 0f;
    private float _vel = 0f;
    private float _maxVel = 6f;
    private DateTime _nextSmoke = DateTime.MinValue;
    private bool _dug;

    void Start()
    {
        _monster = GetComponent<Monster>();
        _entity = GetComponent<LivingEntity>();
        _animator = GetComponent<Animator>();
        _entity.OnSpawn += OnSpawned;
        _animator.speed = 3f;
    }

    private void OnSpawned(LivingEntity e)
    {
        e.AttackerEntity.OnAttack += OnAttack;
        e.AttackableEntity.OnAttacked += OnAttacked;
        e.Stats.MaxLife = 5;
        e.Stats.Life = 5;
    }

    private void OnAttack(AttackableEntity e)
    {
        HitJump(e.transform.position);
    }

    private void HitJump(Vector3 position)
    {
        var direction = (transform.position - position).normalized * _vel / 2;
        
        Main.Services.Vfx.Play(VfxPrefab.CfxR2GroundHit, position);
        _entity.PlayingSequence = _monster.Body.DOJump(transform.position + new Vector3(direction.x, 0, direction.z), 0.5f, 1, 0.25f)
            .OnComplete(() => _monster.Body.rotation = Quaternion.Euler(0, _monster.Body.rotation.eulerAngles.y, 0))
            .SetAutoKill(true);
        _vel /= 4;
        _ace /= 4;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            HitJump(collision.contacts.First().point);
        }
    }

    private void OnAttacked(DamageDealer attacker)
    {
        var rot = (Player.Get().Entity.Center - transform.position);
        rot.Set(rot.x, 0, rot.z);
        _monster.Body.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rot), 45f);
        var direction = (transform.position - attacker.transform.position).normalized * 3;
        _entity.PlayingSequence = _monster.Body.DOJump(transform.position + new Vector3(direction.x, 0, direction.z), 0.5f, 1, 0.25f)
            .OnComplete(() =>
            {
                if(_entity.ZeroLife) _entity.Die();
            })
            .SetAutoKill(true);
    }

    private void Chase()
    {
        if (_monster.IsOnGround)
        {
            if (DateTime.UtcNow > _nextSmoke)
            {
                Main.Services.Vfx.Play(VfxPrefab.LittleSmmoke, _entity.Collider.bounds.center - new Vector3(0, 0.45f, 0), a =>
                {
                    var rot = a.transform.rotation.eulerAngles;
                    a.transform.rotation = Quaternion.Euler(rot.x, _monster.Body.rotation.eulerAngles.y + 180, rot.z);
                });
                _nextSmoke = DateTime.UtcNow + TimeSpan.FromMilliseconds(100);
            }
        }
        var pos = transform.position;
        var direction = (Player.Get().Entity.Center - pos);
        direction.Set(direction.x, 0, direction.z);
        _monster.Body.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 70f * Time.deltaTime);
        _monster.Body.MovePosition(pos + (transform.forward * _vel * Time.fixedDeltaTime));
        if (_vel < _maxVel) _vel += _ace;
        if (_vel > _maxVel) _vel = _maxVel;
        _ace += 0.005f;
    }

    private void Dig()
    {
        if (!_monster.IsOnGround) return;
        _animator.speed = 0;
        
        _entity.PlayingSequence = transform.DOJump(transform.position + new Vector3(0, -1.3f, 0), 3, 1, 0.5f).OnComplete(() => _monster.Body.isKinematic = true);
        _dug = true;
        gameObject.layer = GameLayers.GROUND;
        foreach (var t in transform)
        {
            ((Transform)t).gameObject.layer = GameLayers.GROUND;
        }
    }

    private void Undig()
    {
        _animator.speed = 3f;
        var rot = (Player.Get().Entity.Center - transform.position);
        _monster.Body.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rot), 360f);
        _monster.Body.isKinematic = false;
        _entity.PlayingSequence = transform.DOJump(transform.position + new Vector3(0, 1.3f, 0), 3, 1, 0.5f).OnComplete(() => _dug = false);
        gameObject.layer = GameLayers.MONSTER;
        foreach (var t in transform)
        {
            var t2 = t as Transform;
            var dmg = t2.GetComponent<DamageDealer>();
            if(dmg == null)
                t2.gameObject.layer = GameLayers.MONSTER;
            else 
                t2.gameObject.layer = GameLayers.ENEMY_DAMAGE_DEALER;
        }
    }
    
    void FixedUpdate()
    {
        if (!_monster.SeeingPlayer)
        {
            if(!_dug)
            {
                Dig();
            }
            _vel = 0.1f;
            return;
        }
        if (_entity.PlayingSequence != null && _entity.PlayingSequence.active) return;
        if (_entity.IsStunned) return;

        if (_dug)
        {
            Undig();
            return;
        }
        Chase();
    }
}
