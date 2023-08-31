using System;
using System.Threading.Tasks;
using GameAddressables;
using Src.Data;
using Src.MonoComponent;
using Src.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
	public AssetScene SceneAsset { get; private set; }
	
	public bool AllowSavingPosition = true;
	public event Action OnPlayerMoved;
	public event Action<Interactible> OnPlayerInteracted;
	public event Action<Interactible> OnSetTargetInteractible;
	public event Action<Chest> OnChestOpened;
	public event Action<AssetScene> OnBeforeTeleportTo;
	public event Action<AssetScene> OnMapUnloaded;
	public event Action<Collectible> OnPlayerCollect;
	public MapMusicSetup [] Music;
	
	public event Action<Player> OnPlayerInitialized
	{
		add
		{
			var p = Player.Get();
			if (p != null && p.Loaded)  value.Invoke(p);
			_onPlayerInitialized += value;
		}
		remove => _onPlayerInitialized -= value;
	}

	public static event Action OnMapInitialized
	{
		add
		{
			if (Map.Current != null && Map.Current.Loaded)  value.Invoke();
			_onMapInitialized += value;
		}
		remove => _onMapInitialized -= value;
	}
	
	private static Map _instance;
	private event Action<Player> _onPlayerInitialized;
	private static event Action _onMapInitialized;
	
	public void TriggerOnPlayerCollect(Collectible c) => OnPlayerCollect?.Invoke(c);
	public void TriggerPlayerInitialized() => _onPlayerInitialized?.Invoke(Player.Get());
	public void TriggerPlayerMoved() => OnPlayerMoved?.Invoke();
	public void TriggerPlayerInteracted(Interactible i) => OnPlayerInteracted?.Invoke(i);
	public void TriggerBeforeTeleport(AssetScene sceneName) => OnBeforeTeleportTo?.Invoke(sceneName);
	public void TriggerMapUnloaded(AssetScene sceneName) => OnMapUnloaded?.Invoke(sceneName);
	public void TriggerChestOpened(Chest chest) => OnChestOpened?.Invoke(chest);
	
	public static Map Current => _instance;
	private bool Loaded = false;
	void Awake()
	{
		_instance = this;
		if (!Main.Loaded)
		{
			SceneAsset = Enum.Parse<AssetScene>(SceneManager.GetActiveScene().name);
			GLog.Debug("Loaded Map "+SceneAsset);
			SceneManager.LoadScene("Main", LoadSceneMode.Additive);
		}
	}

	private void OnMapLoaded(AssetScene scene)
	{
		SceneAsset = scene;
		GLog.Debug("Loaded Map "+SceneAsset);
	}

	private async Task FireMapInitialized()
	{
		await Task.Yield(); // fire after all starts called
		_onMapInitialized?.Invoke();
	}
	
	void Start()
	{
		_ = FireMapInitialized();
		Loaded = true;
		if (Music.Length > 0)
		{
			foreach (var m in Music)
			{
				Main.Services.Audio.PlayMusic(m.Audio, m.Pitch, m.Volume);
			}
		}


		Main.Services.Scenes.OnMapLoaded += OnMapLoaded;
		OnPlayerInitialized += _ => Main.Services.Screens.Close<Loading>();
	}

	private void OnDestroy()
	{
		Main.Services.Scenes.OnMapLoaded -= OnMapLoaded;
	}
}
