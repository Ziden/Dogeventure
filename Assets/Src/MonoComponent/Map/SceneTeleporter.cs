using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using GameAddressables;
using Src.Data;
using Src.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
	public AssetScene Scene;

	private CinemachineVirtualCamera _cam;
	
	void Start()
	{
		_cam = GetComponentInChildren<CinemachineVirtualCamera>(true);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;

		if (_cam != null)
		{
			Main.Services.Camera.SwapCamera(_cam, 1f);
		}
		
		Main.Services.Scenes.TransitionMap(Scene);
	}

	
}
