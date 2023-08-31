using System.Collections;
using GameAddressables;
using Src.MonoComponent;
using UnityEngine;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour
{
    public bool AutoCollect;
    private bool _toPlayer;

    public float LootTime = 3;
    public void Toss()
    {
        var rb = GetComponent<Rigidbody>();
        float min = -8f;
        float max = 8f;
        var force = new Vector3(Random.Range(min, max), 12f, Random.Range(min, max));
        rb.AddForce(force, ForceMode.VelocityChange);
        rb.AddTorque(force, ForceMode.VelocityChange);
        if (AutoCollect) StartCoroutine(Autoloot());
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COIN WITH "+collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Map.Current.TriggerOnPlayerCollect(GetComponent<Collectible>());
            Destroy(gameObject);
            Main.Services.Vfx.Play(VfxPrefab.FxShine, Player.Get().Entity.Center);
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Coinflip_011, Random.value / 2 + 0.75f, 1f);
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Map.Current.TriggerOnPlayerCollect(GetComponent<Collectible>());
            Destroy(gameObject);
            Main.Services.Vfx.Play(VfxPrefab.FxShine, Player.Get().Entity.Center);
            Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Coinflip_011, Random.value / 2 + 0.75f, 1f);
        }
    }

    void Update()
    {
        if (_toPlayer)
        {
            var dir = (Player.Get().transform.position - transform.position).normalized;
            transform.position += dir * Time.deltaTime * 20f;
            transform.Rotate(new Vector3(0, 1, 0));
        }
    }

    private IEnumerator Autoloot()
    {
        yield return new WaitForSeconds(LootTime);
        _toPlayer = true;
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }
    
}
