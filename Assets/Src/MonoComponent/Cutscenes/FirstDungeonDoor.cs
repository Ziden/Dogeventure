
using Cinemachine;
using Cysharp.Threading.Tasks;
using Src.MonoComponent;
using Src.Services;
using UnityEngine;

public class FirstDungeonDoor : GameBehaviour
{
	public GameObject LeftDoor;
	public GameObject RightDoor;
	public CinemachineVirtualCamera DoorCamera;
	
	void Start()
	{
		GetComponent<UniqueObject>();
		GetComponent<ColliderTrigger>().PlayerEnter += () => _ = OnEnter();
	}

	private async UniTask OnEnter()
	{
		GLog.Debug("Entered");
		Services.Map.GameFrozen = true;
		Services.Camera.SwapCamera(DoorCamera);
		await UniTask.Delay(1000);
		Services.Map.GameFrozen = false;
	}
}
