using System;
using System.Collections.Generic;
using GameAddressables;
using Src;
using Src.Services;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Assets.Code.Assets.Code.UIScreens.Base
{
    public class LoadedScreen
    {
        public UIBase UI;
        public GameObject Object;
        public bool IsLoaded = false;
    }

    public class GenericSetup : UIScreenSetup { }

    public interface IScreenService : IService
    {
        T Get<T>() where T : UIBase;
        T Open<T>() where T : UIBase;
        T Open<T, H>(H hooks) where T : UIBase where H : UIScreenSetup;
        void Close<T>() where T : UIBase;
        void Close(UIBase screen);
        bool IsOpen<T>() where T : UIBase;
    }

    public class ScreenService : IScreenService
    {
        public static readonly string BUTTON_CLASS = "unity-button";
        private Dictionary<Type, LoadedScreen> _inScene = new ();
        private GameObject _screensContainer;
        private IAssetService _assets => Main.Services.Assets;
        private GenericSetup _noSetup;

        public void OnSceneChange()
        {
           
        }
        
        public ScreenService()
        {
            _screensContainer = new GameObject("Game Screens Container");
            Object.DontDestroyOnLoad(_screensContainer);
            _noSetup = new GenericSetup();
        }

        private void SetupBasicListeners(VisualElement element)
        {
            //foreach (var button in element.Query(null, BUTTON_CLASS).Build())
            //{
                //button.RegisterCallback<PointerDownEvent>(ev => ServiceContainer.Resolve<IAudioService>().PlaySoundEffect(SoundFX.Button_click), TrickleDown.TrickleDown);
            //}
        }

        public bool IsOpen<T>() where T : UIBase
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                return screen.Object.activeSelf;
            }
            return false;
        }


        public T Get<T>() where T : UIBase
        {
            if (_inScene.TryGetValue(typeof(T), out var screen))
            {
                return screen.UI as T;
            }
            return null;
        }

        public void Preload(UIBase screen, GameObject o)
        {
            screen._setup = _noSetup;
            screen._screenService = this;
            var loadedScreen = new LoadedScreen()
            {
                Object = o,
                UI = screen
            };
            loadedScreen.Object.transform.parent = _screensContainer.transform;
            loadedScreen.Object.transform.localPosition = Vector3.zero;
            var uiDoc = o.GetComponent<UIDocument>();
            loadedScreen.UI._root = uiDoc.rootVisualElement;
            loadedScreen.IsLoaded = true;
            SetupBasicListeners(uiDoc.rootVisualElement);
            _inScene[screen.GetType()] = loadedScreen;
            screen.OnLoaded(uiDoc.rootVisualElement);
            GLog.Debug("Cached screen " + screen.GetType());
        }

        public T Open<T, H>(H setup) where T : UIBase where H : UIScreenSetup
        {
            if (_inScene.TryGetValue(typeof(T), out var loadedScreen))
            {
                if (!loadedScreen.IsLoaded) return loadedScreen.UI as T;
                if (loadedScreen.Object.activeSelf) return loadedScreen.UI as T;
                loadedScreen.UI.OnBeforeOpen();
                loadedScreen.Object.SetActive(true);
                var uiDoc = loadedScreen.Object.GetComponent<UIDocument>();
                loadedScreen.UI._setup = setup;
                loadedScreen.UI._panel = uiDoc.rootVisualElement.panel;
                loadedScreen.UI._root = uiDoc.rootVisualElement;
                SetupBasicListeners(uiDoc.rootVisualElement);
                loadedScreen.UI.OnOpen();
                GLog.Debug("Returning cached UI");
                return loadedScreen.UI as T;
            }

            
            var screen = (T)InstanceFactory.CreateInstance(typeof(T));
            screen._setup = setup;
            screen._screenService = this;
            screen.OnBeforeOpen();
            loadedScreen = new LoadedScreen()
            {
                Object = new GameObject(typeof(T).Name),
                UI = screen
            };
            loadedScreen.Object.transform.parent = _screensContainer.transform;
            loadedScreen.Object.transform.localPosition = Vector3.zero;
            _inScene[typeof(T)] = loadedScreen;
            
            _assets.GetPanelSettingsAsync(AssetPanelSettings.Panel, panel =>
           {
               _assets.GetVisualTreeAssetAsync(screen.ScreenAsset, visualTree =>
               {
                   var uiDoc = loadedScreen.Object.AddComponent<UIDocument>();
                   uiDoc.visualTreeAsset = visualTree;
                   uiDoc.panelSettings = panel;
                   SetupBasicListeners(uiDoc.rootVisualElement);
                   screen._root = uiDoc.rootVisualElement;
                   screen.OnLoaded(uiDoc.rootVisualElement);
                   loadedScreen.IsLoaded = true;
                   _inScene[typeof(T)] = loadedScreen;
                   screen.OnOpen();
               });
           });
            return screen;
        }

        public T Open<T>() where T : UIBase
        {
            return Open<T, GenericSetup>(_noSetup);
        }

        public void Close(UIBase s)
        {
            Close(s.GetType());
        }

        private void Close(Type t)
        {
            if (_inScene.TryGetValue(t, out var screen))
            {
                if(screen.IsLoaded)
                {
                    screen.UI.OnClose();
                }
                screen.Object.SetActive(false);
            }
        }

        public void Close<T>() where T : UIBase
        {
            Close(typeof(T));
        }
    }
}
