using System.Collections;
using System.Collections.Generic;
using GameAddressables;
using Src.Data;
using Src.MonoComponent;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponPrefab WeaponPrefab;
    public GameObject CameraCollider;
    private DamageDealer _dealer;

    public WeaponData WeaponData => ConfigManager.Weapons.ByType(WeaponPrefab);
    public DamageDealer DamageDealer => _dealer;

    private Quaternion _cameraRotation;
    
    public void SetupDamageDealer(AttackerEntity attacker)
    {
        DamageDealer.Owner = attacker;
        DamageDealer.Damage = WeaponData.Damage;
        DamageDealer.DisableAttack();
    }
    
    void Start()
    {
        _dealer = GetComponent<DamageDealer>();
        _cameraRotation = Camera.main.transform.rotation;
    }

    void Update()
    {
        CameraCollider.transform.rotation = _cameraRotation;
    }
}
