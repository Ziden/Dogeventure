using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootingStatic : MonoBehaviour
{
    public Projectile Projectile;
    public float SecondsDelay;
    public float InitialDelay;

    private List<Projectile> _pooled = new List<Projectile>();
    private DateTime _nextShotAt;

    void Start()
    {
        _nextShotAt = DateTime.UtcNow + TimeSpan.FromSeconds(InitialDelay);
    }

    void Update()
    {
        if (DateTime.UtcNow < _nextShotAt) return;
        var projectile = Instantiate(Projectile);
        projectile.Shoot(transform.forward);
        projectile.OnHitAnything = () => Pool(projectile);
    }

    private void Pool(Projectile p)
    {
        _pooled.Add(p);
        p.gameObject.SetActive(false);
    }

    private Projectile Get()
    {
        if(_pooled.Count > 0)
        {
            var p = _pooled[0];
            _pooled.RemoveAt(0);
            return p;
        }
        return Instantiate(Projectile);
    }
}