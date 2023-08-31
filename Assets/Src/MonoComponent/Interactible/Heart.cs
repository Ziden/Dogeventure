using System;
using GameAddressables;
using UnityEngine;

public class Heart : MonoBehaviour
{
	private Vector3 _offset = new (0, 0.5f, 0);

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			var p = Player.Get();
			p.Entity.Stats.Life += 1;
		    Main.Services.Vfx.Play(VfxPrefab.CfX2_PickupHeart, transform.position + _offset);
		    Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Down3fast);
			Destroy(gameObject);
		}
	}
}
