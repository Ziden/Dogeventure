using System;
using System.Collections;
using System.Threading.Tasks;
using Assets.Code.Assets.Code.UIScreens.Base;
using GameAddressables;
using Src;
using Src.Data;
using Src.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private static Main _instance;
    
    private GameServices _services;

    public static bool Loaded => _instance != null;
    public static IGameServices Services => _instance._services;
    
    void Awake()
    {
        GLog.Debug("Initializing Game");
        SetupUnity();
        _instance = this;
        _services = new GameServices();
        _services.Build();
        Preloads();
        _services.Save.LoadGame();
      
    }

    private void SetupUnity()
    {
        Application.targetFrameRate = 60;
        TaskScheduler.UnobservedTaskException += (s, a) =>
        {
            Debug.LogException(a?.Exception.InnerException);
            Debug.LogException(a?.Exception);
            Debug.LogError(a?.Exception.InnerException.StackTrace);
        };
        AppDomain.CurrentDomain.FirstChanceException += (sender, args) => Debug.LogException(args.Exception);
        AppDomain.CurrentDomain.UnhandledException += (sender, args) => Debug.LogException((Exception)args.ExceptionObject);
    }
    
    void Start()
    {
        if (SceneManager.sceneCount == 1)  Services.Scenes.AddScene(AssetScene.Map);
        DontDestroyOnLoad(gameObject);
    }
    
    private void Preloads()
    {
        Addressables.InitializeAsync();
        Services.Assets.GetVisualTreeAssetAsync(AssetUIScreen.Dialog, _ => { });
        Services.Assets.GetVisualTreeAssetAsync(AssetUIScreen.MainHud, _ => { });
        Services.Assets.GetAudioClipAsync(AssetSoundEffect.Handy_introduction_022_glbml_21786, _ => { });
    }

    void Update() =>  _services?.Update();
}
