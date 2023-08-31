
using UnityEngine;

public class WeaponCameraCollider : MonoBehaviour
{
	private Weapon _weapon;

	void Start()
	{
		_weapon = transform.parent.GetComponent<Weapon>();
	}

	private void OnTriggerEnter(Collider other)
	{
		_weapon?.DamageDealer.Touches(other.gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		_weapon?.DamageDealer.Touches(other.gameObject);
	}
}
