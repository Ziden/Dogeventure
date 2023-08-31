using Assets.Code.Assets.Code.UIScreens;
using GameAddressables;
using Src.Services;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Code.Assets.Code.UIScreens.Base
{
	/// <summary>
	/// UI hooks to expose ui callbacks to actions.
	/// </summary>
	public interface UIScreenSetup { }

	/// <summary>
	/// Represents a UI screen or element
	/// </summary>
	public abstract class UIBase
	{
		internal IPanel _panel;
		internal UIScreenSetup _setup;
		internal IScreenService _screenService;
		internal VisualElement _root;
		internal IPlayerData PlayerData => Main.Services.Data;

		public IScreenService ScreenService => _screenService;

		public IPanel Panel;

		public VisualElement Root => _root;
		public abstract AssetUIScreen ScreenAsset { get; }
		public virtual void OnLoaded(VisualElement root) { }
		public virtual void OnBeforeOpen() { }
		public virtual void OnOpen() { }
		public virtual void OnClose() { }
        
		public void Hide() => Root.style.visibility = Visibility.Hidden;
		public void Show() => Root.style.visibility = Visibility.Visible;
		public bool IsHidden() => Root.style.visibility == Visibility.Hidden;
		public T GetSetup<T>() => (T)_setup;
	}
}