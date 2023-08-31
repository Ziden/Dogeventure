using System;
using Assets.Code.Assets.Code.UIScreens.Base;
using GameAddressables;
using Src.Data;
using Src.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Src.UI
{
	public class DialogSetup : UIScreenSetup {
		public string [] Text;
		public Sprite Sprite;
		public Action OnClose;
	}
	
	public class Dialog : UIBase
	{
		private string[] Text;
		public int Current = 0;
		
		public override void OnOpen()
		{
			var setup = GetSetup<DialogSetup>();
			var icon = Root.Q<VisualElement>("Icon");
			if (setup.Sprite == null)
			{
				icon.style.display = DisplayStyle.None;
			}
			else
			{
				icon.style.display = DisplayStyle.Flex;
				var bg = icon.style.backgroundImage.value;
				bg.sprite = setup.Sprite;
				icon.style.backgroundImage = bg;
			}
			Text = setup.Text;
			var label = Root.Q<Label>("Text");
			label.enableRichText = true;
			label.text = Text[Current];
			InputService.OnButton1 += OnInput;
			Main.Services.Map.GameFrozen = true;
		}

		private void OnInput()
		{
			if (Current < Text.Length - 1) Advance();
			else CloseDialog();
		}

		private void Advance()
		{
			Current++;
			Root.Q<Label>("Text").text = Text[Current];
		}

		private void CloseDialog()
		{
			var setup = GetSetup<DialogSetup>();
			setup.OnClose?.Invoke();
			Current = 0;
			ScreenService.Close(this);
			Main.Services.Map.GameFrozen = false;
			InputService.OnButton1 -= OnInput;
		}
		
		public override AssetUIScreen ScreenAsset => AssetUIScreen.Dialog;
	}
}