using System;
using Src.MonoComponent;
using UnityEngine;

/// <summary>
/// Entities that can attack and have attack controlled cooldowns
/// </summary>
public class AttackerEntity : MonoBehaviour
{
    public event Action<long> OnBeginAttack;
    public event Action<AttackableEntity> OnAttack;
    
    public float AttackDurationSeconds;
    public float AttackCooldownSeconds;
    
    private DateTime _lastAttack;

    public DateTime LastAttack => _lastAttack;
    private InventoryHolder _holder;
    public void TryAttack()
    {
        if (IsCooldown()) return;

        _holder = GetComponent<InventoryHolder>();
        _lastAttack = DateTime.UtcNow;
        OnBeginAttack?.Invoke(Time.frameCount);
    }

    public void Attack(AttackableEntity e)
    {
        OnAttack?.Invoke(e);
    }

    public bool IsCooldown() => IsAttacking();

    public bool IsAttacking()
    {
        if (_holder != null && _holder.Attacking)
        {
            return true;
        }
        return _lastAttack + TimeSpan.FromSeconds(AttackDurationSeconds) > DateTime.UtcNow;
    }
}
