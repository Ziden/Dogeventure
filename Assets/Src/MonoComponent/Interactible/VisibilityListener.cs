using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VisibilityListener : MonoBehaviour
{
    public event Action OnVisible;
    public event Action OnInvisible;

    public void Track(Type t)
    {
        var p = Player.Get();
    }
    
    private void OnBecameVisible()
    {
        OnVisible?.Invoke();
    }

    private void OnBecameInvisible()
    {
        OnInvisible?.Invoke();
    }
}
