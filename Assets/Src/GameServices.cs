using Assets.Code.Assets.Code.UIScreens.Base;
using GameAddressables;
using Src.Services;

namespace Src
{
	public interface IGameServices
	{
		public IAssetService Assets { get; }
		public AudioService Audio { get; }
		public SceneService Scenes { get; }
		public ScreenService Screens { get; }
		public InputService Input { get; }
		public SaveService Save { get; }
		public VfxService Vfx { get; }
		public IPlayerData Data { get; }
		public CameraService Camera { get; }
		public MapService Map { get; }
	}

	public class GameServices : IGameServices
	{
		private readonly ServiceContainer _services;
		private readonly IAssetService _assets;
		
		public GameServices()
		{
			_services = new ServiceContainer();
			_assets = new AssetService();
			
			GLog.Debug("Loaded services");
		}

		public void Build()
		{
			_services.AddService(new AudioService());
			_services.AddService(new InputService());
			_services.AddService(new ScreenService());
			_services.AddService(new SceneService());
			_services.AddService(new VfxService());
			_services.AddService(new SaveService());
			_services.AddService(new PlayerDataService());
			_services.AddService(new CameraService());
			_services.AddService(new MapService());
			
			Scenes.OnMapUnloaded += _services.OnSceneChange;
		}

		public void Update()
		{
			_services.Update();
		}

		public IAssetService Assets => _assets;
		public AudioService Audio => _services.Resolve<AudioService>();
		public SceneService Scenes => _services.Resolve<SceneService>();
		public ScreenService Screens => _services.Resolve<ScreenService>();
		public InputService Input =>  _services.Resolve<InputService>();
		public SaveService Save => _services.Resolve<SaveService>();
		public VfxService Vfx => _services.Resolve<VfxService>();
		public IPlayerData Data => _services.Resolve<PlayerDataService>();
		public CameraService Camera => _services.Resolve<CameraService>();
		public MapService Map => _services.Resolve<MapService>();
	}
}