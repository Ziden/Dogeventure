using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameAddressables;
using Src.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Src.Services
{
	public class SceneSettings
	{
		public readonly Action<SceneInstance> OnLoad;
		public readonly Action OnUnload;

		public SceneSettings(Action<SceneInstance> onLoad=null, Action onUnload=null)
		{
			OnLoad = onLoad;
			OnUnload = onUnload;
		}
	}
	
	public class SceneService : IService
	{
		private HashSet<string> _loaded = new();
		private Dictionary<string, SceneSettings> _settings = new();
		private Dictionary<AssetScene, SceneInstance> _loadedAssets = new();

		public event Action OnMapUnloaded;
		public event Action<AssetScene> OnMapLoaded;

		public void OnSceneChange() {}
		
		public SceneService()
		{
			SceneManager.sceneLoaded += (s, l) =>
			{
				_loaded.Add(s.name);
			};
			
			SceneManager.sceneUnloaded += s =>
			{
				_loaded.Remove(s.name);
				OnMapUnloaded?.Invoke();
				if (_settings.TryGetValue(s.name, out var settings))
				{
					settings?.OnUnload();
					_settings.Remove(s.name);
				}
			};
		}

		public void TransitionMap(AssetScene newMap)
		{
			Main.Services.Map.GameFrozen = true;
			Map.Current.TriggerBeforeTeleport(newMap);
			Main.Services.Save.SaveGame();
			Main.Services.Scenes.ReplaceScene(newMap);
			Main.Services.Map.GameFrozen = false;
			OnMapLoaded?.Invoke(newMap);
		}

		public void AddScene(AssetScene scene, SceneSettings settings = null, int priority = 100)
		{
			_ = LoadSceneTask(scene, settings, priority);
		}
		
		public void ReplaceScene(AssetScene scene, SceneSettings settings = null, int priority = 100)
		{
			_ = ReplaceSceneTask(scene, settings, priority);
		}

		private async Task LoadSceneTask(AssetScene scene, SceneSettings settings = null, int priority = 100)
		{
			var address = AddressIdMap.IdMap[(int) scene];
			var op = Addressables.LoadSceneAsync(address, LoadSceneMode.Additive, true, priority);
			if(settings?.OnUnload != null) 	_settings[scene.ToString()] = settings;
			await op.Task;
			_loadedAssets[scene] = op.Result;
			if (settings?.OnLoad != null) settings.OnLoad(op.Result);
		}
		
		private async Task ReplaceSceneTask(AssetScene scene, SceneSettings settings = null, int priority = 100)
		{
			var address = AddressIdMap.IdMap[(int) scene];
			var op = Addressables.LoadSceneAsync(address, LoadSceneMode.Single, true, priority);
			if(settings?.OnUnload != null) 	_settings[scene.ToString()] = settings;
			await op.Task;
			_loadedAssets[scene] = op.Result;
			if (settings?.OnLoad != null) settings.OnLoad(op.Result);
		}

		public void UnloadScene(AssetScene scene, SceneSettings settings = null)
		{
			if (_loadedAssets.ContainsKey(scene))
			{
				Addressables.UnloadSceneAsync(_loadedAssets[scene]);
				_loadedAssets.Remove(scene);
			}
			else SceneManager.UnloadSceneAsync(scene.ToString());
		}

		public bool HasScene(AssetScene scene)
		{
			var address = AddressIdMap.IdMap[(int) scene];
			return _loaded.Contains(address);
		}
	}
}