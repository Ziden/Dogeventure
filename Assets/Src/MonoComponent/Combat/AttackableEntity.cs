using System;
using System.Collections;
using GameAddressables;
using Src;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Entities that can be attacked
/// Damagers needs the "DamageDealer" component
/// </summary>
public class AttackableEntity : MonoBehaviour
{
	public float InvulnerabilitySeconds = 0f;
	
	private DateTime _lastAttacked;
	private Collider _collider;
	private long _lastAttackStartFrame;
	
	public Action<DamageDealer> OnAttacked;
	public Collider Collider => _collider;

	void Start()
	{
		_collider = GetComponent<Collider>();
		if (_collider == null)
		{
			throw new Exception("Attackable Entity must have a collider");
		}
	}
	
	public bool IsInvunlerable() => _lastAttacked + TimeSpan.FromSeconds(InvulnerabilitySeconds) > DateTime.UtcNow;

	public void GetHit(DamageDealer attacker)
	{
		if (IsInvunlerable()) return;
		if (!attacker.CanMultihit && _lastAttackStartFrame == attacker.StartedFrame) return;
		_lastAttackStartFrame = attacker.StartedFrame;
		_lastAttacked = DateTime.UtcNow;
		OnAttacked?.Invoke(attacker.GetComponent<DamageDealer>());
	}
}
