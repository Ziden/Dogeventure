using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Code.Assets.Code.UIScreens.Base;
using GameAddressables;
using Src.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Src.MonoComponent
{
	public class ScreenCache : MonoBehaviour
	{
		public GameObject Dialog;
		public GameObject GotItem;
		public GameObject Loading;
		
		private void Start()
		{
			_ = Cache();
			DontDestroyOnLoad(gameObject);
		}

		private async Task Cache()
		{
			await Task.Yield();
			var screenService = Main.Services.Screens;
			screenService.Preload(new Dialog(), Dialog);
			screenService.Preload(new GotItem(), GotItem);
			screenService.Preload(new Loading(), Loading);
			Map.OnMapInitialized += () => Map.Current.OnPlayerInitialized += _ => OnLoad();
		}

		private async Task OnLoad()
		{
			await Task.Yield();
			Loading.SetActive(false);
		}
	}
}