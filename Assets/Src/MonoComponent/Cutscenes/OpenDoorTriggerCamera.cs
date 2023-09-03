
using Cinemachine;
using Cysharp.Threading.Tasks;
using Src.MonoComponent;

public class OpenDoorTriggerCamera : GameBehaviour
{
	public Door Door;
	public CinemachineVirtualCamera DoorCamera;
	
	void Start()
	{
		var unique = GetComponent<UniqueObject>();
		if(unique.HasAlreadyInteractedWith())
		{
			Door.Duration = 0;
			Door.Open();
			return;
		}
		GetComponent<ColliderTrigger>().PlayerEnter += () => _ = OnEnter();
	}

	private async UniTask OnEnter()
	{
		var p = Player.Get();
		Services.Map.GameFrozen = true;
		p.Animation.Play(CharacterAnimation.idle);
		Services.Camera.SwapCamera(DoorCamera, 0f);
		p.transform.LookAt(Door.transform.position);
		await UniTask.Delay(1000);
		Door.Duration = 5;
		Door.Open();
		await UniTask.Delay((int)(Door.Duration * 1000));
        await UniTask.Delay(2000);
		Services.Camera.SetEase(0f);
        Services.Camera.ReturnCamera();
        Services.Map.GameFrozen = false;
		Destroy(GetComponent<ColliderTrigger>());
	}
}
