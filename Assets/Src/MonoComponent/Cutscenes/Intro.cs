using System.Security.Cryptography;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameAddressables;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public CinemachineVirtualCamera DoorCam;
    public CinemachineVirtualCamera IntroCamera;
    public GameObject CloudFar;
    public GameObject CloudNear;
    public GameObject Shine;
    public GameObject Bean;
    public GameObject Explosion;

    void Start()
    {
        Map.OnMapInitialized += () => _ = StartNextFrame();
    }

    private async UniTask StartNextFrame()
    {
        await UniTask.Yield();
        var isNew = Main.Services.Data.Weapons().Count == 0 && Main.Services.Data.DungeonKeys() == 0;
        if (!isNew)
        {
            Destroy(gameObject);
            return;
        }
        await PlaySequence();
    }

    private async UniTask Preload()
    {
        await Main.Services.Assets.GetAudioClipAsync(AssetSoundEffect.Beam);
        await Main.Services.Assets.GetAudioClipAsync(AssetSoundEffect.Shine);
        await Main.Services.Assets.GetAudioClipAsync(AssetSoundEffect.Low_impactwav_14905);
    }

    private async UniTask PlaySequence()
    {
        var p = Player.Get();
       
        p.Graphic.gameObject.SetActive(false);
        Main.Services.Map.GameFrozen = true;
        var cam = Main.Services.Camera;
        cam.SwapCamera(DoorCam, 0);
        
        await Preload();
        await UniTask.Delay(1000);
        cam.SwapCamera(IntroCamera, 5f);

        await UniTask.Delay(5000);
       
        var initialRotation = IntroCamera.transform.rotation;
        cam.SetEase(5f);
        //IntroCamera.LookAt = CloudFar.transform;
        
        IntroCamera.transform.DOLookAt(CloudFar.transform.position, 4f);

        await UniTask.Delay(5000);
        Main.Services.Audio.PlaySyncedSoundEffect(AssetSoundEffect.Shine, () =>
        {
            Shine.gameObject.SetActive(true);
        });
        
        await UniTask.Delay(2000);
        
        Main.Services.Audio.PlaySyncedSoundEffect(AssetSoundEffect.Beam, () =>
        {
            Bean.gameObject.SetActive(true);
            cam.SetEase(1f);
            IntroCamera.LookAt = null;
        });
        
        IntroCamera.transform.DORotateQuaternion(initialRotation, 1f).OnComplete(() =>
        {
            Destroy(CloudFar);
            Destroy(CloudNear);
            Main.Services.Audio.PlaySyncedSoundEffect(AssetSoundEffect.Low_impactwav_14905, () =>
            {
                Explosion.gameObject.SetActive(true);
            });
        });

        await UniTask.Delay(800);
        
        Bean.gameObject.SetActive(false);

        await UniTask.Delay(1000);
        p.Graphic.gameObject.SetActive(true);
        p.Animation.Play(CharacterAnimation.landing);
        p.Animation.LockAnimations = true;
        await UniTask.Delay(3000);
        cam.SetEase(2f);
        cam.ReturnCamera();
        p.Animation.LockAnimations = false;
        Main.Services.Map.GameFrozen = false;
        p.CalculateInputRotation();
        Destroy(gameObject);
    }

    void Update()
    {
        if (CloudFar == null) return;
        CloudFar.transform.Rotate(0, 0.1f, 0 );
        CloudNear.transform.Rotate(0, 0.2f, 0 );
    }
}
