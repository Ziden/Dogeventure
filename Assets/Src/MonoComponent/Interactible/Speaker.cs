using Assets.Code.Assets.Code.UIScreens.Base;
using Src.UI;
using UnityEngine;

namespace Src.MonoComponent
{
	public class Speaker : MonoBehaviour
	{
		public string [] Text;
		public Sprite Sprite;
		
		void Start()
		{
			GetComponent<Interactible>().OnInteract += OnInteract;
		}

		private void OnInteract()
		{
			Main.Services.Screens.Open<Dialog, DialogSetup>(new DialogSetup()
			{
				Text = Text,
				Sprite = Sprite
			});
		}
	}
}