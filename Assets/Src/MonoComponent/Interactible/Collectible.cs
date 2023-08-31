using System.Threading.Tasks;
using Assets.Code.Assets.Code.UIScreens.Base;
using DG.Tweening;
using GameAddressables;
using Src.UI;
using UnityEngine;

namespace Src.MonoComponent
{
	public class Collectible : MonoBehaviour
	{
		private UniqueObject _u;
		public CollectiblePrefab CollectibleType;
		private bool Collected = false;
		
		void Start()
		{
			_u = GetComponent<UniqueObject>();
			if (_u != null && _u.HasAlreadyInteractedWith())
			{
				Destroy(gameObject);
				return;
			}
			var interact = GetComponent<AttackableEntity>();
			if(interact != null) interact.OnAttacked += OnInteract;
		}

		private void OnInteract(DamageDealer weapon)
		{
			if (Collected) return;
			Collected = true;
			Main.Services.Vfx.Play(VfxPrefab.CfxR4SwordTrailPlaiN_360Spiral, GetComponent<Collider>().GetCenterTop());
			Map.Current.TriggerOnPlayerCollect(this);
			OpenWindow();
			
			var seq = DOTween.Sequence();
			seq.Join(transform.DOJump(Player.Get().transform.position, 1, 1, 0.5f).SetEase(Ease.Linear));
			seq.Join(transform.DORotate(new Vector3(55, 55, 55), 0.5f));
			seq.SetAutoKill(true);
			seq.OnComplete(() =>
			{
				Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Cloth_heavy);
				Main.Services.Vfx.Play(VfxPrefab.HitPow, transform.position);
				Destroy(gameObject);
			});
			seq.Play();
		}

		private void OpenWindow()
		{
			var cfg = ConfigManager.Collectibles.ByType(CollectibleType);
			Main.Services.Screens.Open<GotItem, GotItemSetup>(new GotItemSetup()
			{
				Sprite = cfg.Sprite,
				Text = cfg.Name
			});
		}
	}
}