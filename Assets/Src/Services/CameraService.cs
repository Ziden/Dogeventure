using Cinemachine;
using GameAddressables;
using UnityEngine;

namespace Src.Services
{
	public class CameraService : IService
	{
		private GameCamera _camera;
		
		public CameraService()
		{
			GameCamera.OnCameraInitialized += c => _camera = c;
		}
		
		public void OnSceneChange()
		{
			
		}

		public void SetEase(float ease) => _camera.SetCameraEase(ease);

		public void PlayTrack(CinemachineSmoothPath track, GameObject lookat = null, float speed = 2f)
		{
			_camera.PlayTrack(track, lookat, speed);
		}
		
		public void ReturnCamera() => _camera.ReturnCamera();

		public void SwapCamera(CinemachineVirtualCamera camera, float speed = 2f)
		{
			_camera.SwapCamera(camera, speed);
		} 
	}
}