using System.Collections;
using System.Collections.Generic;
using GameAddressables;
using Src.Services;
using Unity.VisualScripting;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public int CoinsMin;
    public int CoinsMax;

    private LivingEntity _entity;
    
    void Start()
    {
        _entity = GetComponent<LivingEntity>();
        if(_entity != null) _entity.OnDie += OnDie;
    }

    private void OnDie()
    {
        var n = Random.Range(CoinsMin, CoinsMax);
        for (var x = 0; x < n; x++)
        {
            DropCoin(2.5f + x * 0.1f);
        }
    }

    private void DropCoin(float delay)
    {
        var pos = transform.position;
        
        Main.Services.Assets.InstantiateCollectiblePrefabAsync(CollectiblePrefab.Coin, a =>
        {
            a.transform.position = pos;
            var coin = a.GetOrAddComponent<Coin>();
            coin.AutoCollect = true;
            coin.LootTime = delay;
            coin.Toss();
        });
        
    }
}
