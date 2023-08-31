using System;
using UnityEngine;

/// <summary>
/// Attached to items that deal damage (e.g spells, weapons, arrows)
/// </summary>
public class DamageDealer : MonoBehaviour
{
	public bool CanMultihit = true;
	private long _startAttackFrame;
	public short Damage;

	private AttackerEntity _owner;

	public AttackerEntity Owner
	{
		set => _owner = value;
		get
		{
			if (_owner != null) return _owner;
			var p = gameObject;
			while (p != null)
			{
				var atk = p.GetComponent<AttackerEntity>();
				if (atk != null)
				{
					_owner = atk;
					return _owner;
				}
				p = p.transform.parent.gameObject;
			}
			return null;
		}
	}
    public TrailRenderer Trail;
    private Collider _collider;
    public Collider Collider => _collider;

    public long StartedFrame => _startAttackFrame;
    
    public void StartAttack()
    {
	    _startAttackFrame = Time.frameCount;
	    Trail.gameObject.SetActive(true);
	    _collider.enabled = true;
    }

    public void DisableAttack()
    {
	    _startAttackFrame = 0;
	    Trail.gameObject.SetActive(false);
	    _collider.enabled = false;
    }
    
    void Start()
    {
	    _collider = GetComponent<Collider>();
    }

    public void Touches(GameObject other)
    {
	    if (!_collider.enabled) return;
	    var attackable = other.GetComponent<AttackableEntity>();
	    if (attackable == null) return;
	    if (Owner == other) return;
	    attackable.GetHit(this);
	    if(_owner != null) _owner.Attack(attackable);
    }
    
    private void OnTriggerEnter(Collider other)
    {
	    Touches(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
	    Touches(other.gameObject);
    }

    private void OnCollisionStay(Collision other)
    {
	    Touches(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
	    Touches(other.gameObject);
    }
}
