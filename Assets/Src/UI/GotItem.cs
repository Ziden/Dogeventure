using System;
using System.Threading.Tasks;
using Assets.Code.Assets.Code.UIScreens.Base;
using GameAddressables;
using Src.Data;
using Src.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Src.UI
{
	public class GotItemSetup : UIScreenSetup {
		public string Text;
		public Sprite Sprite;
	}
	
	public class GotItem : UIBase
	{
		private string Text;
		public Sprite Icon;
		
		public override void OnOpen()
		{
			var setup = GetSetup<GotItemSetup>();
			var icon = Root.Q<VisualElement>("Icon");
			GLog.Debug("Opening");
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
			Text = setup.Text + " +1";
			var label = Root.Q<Label>("Name");
			label.enableRichText = true;
			label.text = Text;
			_ = CloseDialog();
		}

		private async Task CloseDialog()
		{
			await Task.Delay(2000);
			ScreenService.Close(this);
		}
		
		public override AssetUIScreen ScreenAsset => AssetUIScreen.GotItem;
	}
}