using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Action<AttackableEntity> OnHitEntity;
    public Action OnHitAnything;

    private float _speed;
    private Vector3 _dir;

    private void OnCollisionEnter(Collision collision)
    {
        OnHitAnything?.Invoke();
    }

    public void Shoot(Vector3 direction, float speed)
    {
        _dir = direction;
        _speed = speed;      
    }

    void Update()
    {
        if(_dir == Vector3.zero) return;
        transform.Translate(_dir * _speed * Time.deltaTime);
    }
}