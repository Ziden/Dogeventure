using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Src.Services;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    public event Action<GameObject> Enter;
    public event Action<GameObject> Leave;

    public event Action PlayerEnter;
    public event Action PlayerLeave;

    private HashSet<GameObject> _colliding = new HashSet<GameObject>();
    
    public void OnTriggerEnter(Collider other)
    {
        GLog.Debug("Collide "+other.tag);
        _colliding.Add(other.gameObject);
        Enter?.Invoke(other.gameObject);
        if (other.CompareTag("Player")) PlayerEnter?.Invoke();
    }

    public void OnTriggerExit(Collider other)
    {
        GLog.Debug("Exit "+other.tag);
        Leave?.Invoke(other.gameObject);
        _colliding.Remove(other.gameObject);
        if (other.CompareTag("Player")) PlayerLeave?.Invoke();
    }

    public void RemoveTrigger()
    {
        foreach (var other in _colliding)
        {
            if (other == null) continue;
            Leave?.Invoke(other.gameObject);
            if (other.CompareTag("Player")) PlayerLeave?.Invoke();
        }
        Destroy(this);
    }
}
