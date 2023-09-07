using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Action<AttackableEntity> OnHitEntity;
    public Action OnHitAnything;
    public float Speed;

    private Vector3 _dir;

    private void OnCollisionEnter(Collision collision)
    {
        OnHitAnything?.Invoke();
    }

    public void Shoot(Vector3 direction)
    {
        _dir = direction;
    }

    void Update()
    {
        if(_dir == Vector3.zero) return;
        transform.Translate(_dir * Speed * Time.deltaTime);
    }
}