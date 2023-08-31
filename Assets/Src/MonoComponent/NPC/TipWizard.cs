using Assets.Code.Assets.Code.UIScreens.Base;
using Cinemachine;
using GameAddressables;
using Src.Data;
using Src.UI;
using UnityEngine;

namespace Src.MonoComponent.NPC
{
	public class TipWizard : MonoBehaviour
	{
		private CinemachineVirtualCamera _dialogCamera;
		public GameObject QuestionMark;
		
		void Start()
		{
			GetComponent<Interactible>().OnInteract += OnInteract;
			_dialogCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);
			QuestionMark.SetActive(false);

			Map.Current.OnChestOpened += OnChestOpened;
		}

		private void OnChestOpened(Chest c)
		{
			var w = c.Item.GetComponent<Weapon>();
			if (w != null && w.WeaponPrefab == WeaponPrefab.StarterSword)
			{
				QuestionMark.SetActive(true);
			}
		}

		private void Display(AssetSprite? sprite, string text)
		{
			if (!sprite.HasValue)
			{
				Main.Services.Screens.Open<Dialog, DialogSetup>(new DialogSetup()
				{
					Text = new [] { text },
					OnClose = OnCloseDialog
				});
				return;
			}
			Main.Services.Assets.GetSpriteAsync(sprite.Value, s =>
			{
				Main.Services.Screens.Open<Dialog, DialogSetup>(new DialogSetup()
				{
					Text = new [] { text },
					Sprite = s,
					OnClose = OnCloseDialog
				});
			});
		}

		private void OnCloseDialog()
		{
			Main.Services.Camera.ReturnCamera();
		}

		private void OnInteract()
		{
			QuestionMark.SetActive(false);
			Main.Services.Camera.SwapCamera(_dialogCamera, 0.3f);
			
			var p = Player.Get();

			if (Main.Services.Data.Weapons().Count == 0)
			{
				Display(AssetSprite.Icon_itemicon_sword, "Sword is the Way");
			}
			else
			{
				Display(AssetSprite.Butterfly, "Flies the Path");
			}

		}
	}
}