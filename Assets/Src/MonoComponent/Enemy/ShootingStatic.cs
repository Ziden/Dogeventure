using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;

public class ShootingStatic : MonoBehaviour
{
    public GameObject Projectile;
    public float SecondsDelay;
    public float InitialDelay;
    public float Speed;

    private List<Projectile> _pooled = new List<Projectile>();
    private DateTime _nextShotAt;

    void Start()
    {
        _nextShotAt = DateTime.UtcNow + TimeSpan.FromSeconds(InitialDelay);
    }

    void Update()
    {
        if (DateTime.UtcNow < _nextShotAt) return;
        var projectile = GetProjectile();
        projectile.transform.position = transform.position;
        projectile.gameObject.SetActive(true);
        projectile.Shoot(transform.forward, Speed);
        _nextShotAt = DateTime.UtcNow + TimeSpan.FromSeconds(SecondsDelay);
    }

    private void Pool(Projectile p)
    {
        _pooled.Add(p);
        p.gameObject.SetActive(false);
    }

    private Projectile GetProjectile()
    {
        if(_pooled.Count > 0)
        {
            var p = _pooled[0];
            _pooled.RemoveAt(0);
            return p;
        }
        var o = Instantiate(Projectile);
        var projectile = o.GetOrAddComponent<Projectile>();
        projectile.OnHitAnything = () => Pool(projectile);
        return projectile;
    }
}