using System;
using System.Threading.Tasks;
using DG.Tweening;
using GameAddressables;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Src.MonoComponent
{
	public class Bush : MonoBehaviour
	{
		public bool Heart = false;
		public VfxPrefab DestroyEffect = VfxPrefab.HitSplashLeaves;
		public Vector3 EffectScale = new (0.5f, 0.5f, 0.5f);
		public AssetSoundEffect DestroySound;
		
		private AttackableEntity _attackable;
		
		void Start()
		{
			_attackable = gameObject.GetOrAddComponent<AttackableEntity>();
			_attackable.OnAttacked += OnAttacked;
		}

		private void Drops()
		{
			if (Heart)
			{
				var pos = transform.position;
				pos.Set(pos.x, gameObject.GetClosestFloorY().Value, pos.z);
				Main.Services.Assets.InstantiateObjectPrefabAsync(ObjectPrefab.Heart, o =>
				{
					o.transform.position = pos;
					o.transform.DOJump(o.transform.position, 1, 1, 1);
				});
			}
		}

		private void OnAttacked(DamageDealer dealer)
		{
			var weapon = dealer.GetComponent<Weapon>();
			if (weapon != null && weapon.WeaponData.Prefab.ToString().ToLower().Contains("sword"))
			{
				Main.Services.Vfx.Play(VfxPrefab.CfxR4SwordHitPlaiN_Cross, _attackable.Collider.bounds.center, fx =>
				{
					fx.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
				});
				Main.Services.Vfx.Play(DestroyEffect, _attackable.Collider.bounds.center, fx =>
				{
					fx.transform.localScale = EffectScale;
				});
				if (DestroySound == default)
				{
					var r = Random.value;
					if(r < 0.25) Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Leaf1);
					else if(r < 0.5) Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Leaf2);
					else if(r < 0.75) Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Leaf3);
					else Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Leaf4);
				}
				else
				{
					Main.Services.Audio.PlaySoundEffect(DestroySound);
				}
				Drops();
				Destroy(gameObject);
			}
		}
	}
}