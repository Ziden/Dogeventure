using System;
using System.Collections;
using System.Collections.Generic;
using Src.MonoComponent;
using Src.Services;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemPlatform : MonoBehaviour
{
    [SerializeField] private Transform holdAnchor;
    [SerializeField] private bool active;
    [SerializeField] private Animator animator;
    private static readonly int Active1 = Animator.StringToHash("Active");


    private SignalSource _signalSource;

    private void Awake()
    {
        _signalSource = GetComponent<SignalSource>();
    }

    public void SetActive(GameObject item)
    {
        item.transform.SetPositionAndRotation(holdAnchor.position, holdAnchor.rotation);
        this.active = true;
        UpdateActive();
        _signalSource?.Signal();
    }

    private void UpdateActive()
    {
        animator.SetBool(Active1, active);
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateActive();
        GetComponent<AttackableEntity>().OnAttacked += OnAttacked;
    }

    private void OnAttacked(DamageDealer obj)
    {
        if (obj.Owner.TryGetComponent<InventoryHolder>(out var holder))
        {
            var prefab = holder.CurrentWeapon.WeaponPrefab;
            holder.SetCurrentWeapon(null);
            Main.Services.Assets.InstantiateWeaponPrefabAsync(prefab, SetActive);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}