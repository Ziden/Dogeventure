using System;
using Cinemachine;
using Src;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public CinemachineVirtualCamera FollowPlayerCamera;
    public CinemachineVirtualCamera CartCamera;
    public CinemachineSmoothPath DialogTrack;

    public static event Action<GameCamera> OnCameraInitialized;

    private CinemachineVirtualCamera _currentCamera;
    private Camera _camera;
    
    private CinemachineDollyCart _cart;
    private CinemachineBrain _brain;
    private CinemachineSmoothPath _currentTrack;

    public void SetCameraEase(float secs)
    {
        _brain.m_DefaultBlend.m_Time = secs;
    }
    
    void Start()
    {
        _cart = gameObject.FindComponent<CinemachineDollyCart>();
        _camera = gameObject.FindComponent<Camera>();
        _brain = Camera.main.GetComponent<CinemachineBrain>();
        Map.Current.OnPlayerInitialized += p =>
        {
            var t = p.transform;
            FollowPlayerCamera.Follow = t;
            FollowPlayerCamera.LookAt = t;
        };
        OnCameraInitialized?.Invoke(this);
    }

    public void SwapCamera(CinemachineVirtualCamera camera, float speed = 2f)
    {
        _brain.m_DefaultBlend.m_Time = speed;
        FollowPlayerCamera.gameObject.SetActive(false);
        camera.gameObject.SetActive(true);
        _currentCamera = camera;
    }

    public void PlayTrack(CinemachineSmoothPath track, GameObject lookat=null, float speed=2f)
    {
        _cart.m_Path = track;
        _cart.m_Position = 0;
        _cart.gameObject.SetActive(true);
        track.gameObject.SetActive(true);
        _brain.m_DefaultBlend.m_Time = speed;
        CartCamera.gameObject.SetActive(true);
        if(lookat != null)
            CartCamera.m_LookAt = lookat.transform;
        else
        {
            CartCamera.m_LookAt = Player.Get().transform;
        }
        FollowPlayerCamera.gameObject.SetActive(false);
        _currentCamera = CartCamera;
        _currentTrack = track;
    }

    public void ReturnCamera()
    {
        _cart.gameObject.SetActive(false);
        if(_currentTrack != null)  _currentTrack.gameObject.SetActive(false);
        _currentCamera.gameObject.SetActive(false);
        FollowPlayerCamera.gameObject.SetActive(true);
    }

}
