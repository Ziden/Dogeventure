using System;
using System.Collections;
using GameAddressables;
using Src.Data;
using Src.Services;
using TMPro;
using UnityEngine;

namespace Src.MonoComponent
{
    public class InventoryHolder : MonoBehaviour
    {
        public Transform WeaponInactive;
        public Transform WeaponActive;
        
        public bool HasWeapon => _currentWeapon != null;
        public event Action<Item> OnObtainItem;
        public event Action<WeaponPrefab?> OnWeaponChanged;
        public event Action<Weapon> OnWeaponHitboxStart;
        public event Action<Weapon> OnWeaponHitboxEnd;
        private AttackerEntity _attacker;
        private PlayerAnimation _anim;
        
        private Weapon _currentWeapon;
        public bool IsWeaponKept { get; private set; } = true;
        public bool Attacking { get; private set; }
        
        
        public Weapon CurrentWeapon => _currentWeapon;
        
        void Start()
        {
            _attacker = GetComponent<AttackerEntity>();
            _attacker.OnBeginAttack += OnBeginAttack;
            _anim = GetComponent<PlayerAnimation>();
        }
        
        public void TakeWeapon()
        {
            if (!IsWeaponKept) return;
            var weapon = _currentWeapon.transform;
            weapon.SetParent(WeaponActive);
            weapon.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            IsWeaponKept = false;
        }

        public void KeepWeapon()
        {
            if (IsWeaponKept) return;
            var weapon = _currentWeapon.transform;
            weapon.SetParent(WeaponInactive);
            weapon.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            IsWeaponKept = true;
        }
        
        private void OnBeginAttack(long attackFrame)
        {
            if (WeaponInactive.childCount > 0 && WeaponActive.childCount == 0)
            {
                TakeWeapon();
            }
            if (IsWeaponKept)
            {
                GLog.Debug("[Equipment Holde] Weapon Kept");
                return;
            }

            _attacker.AttackCooldownSeconds = CurrentHitboxDurationTime;
            _attacker.AttackDurationSeconds = CurrentAttackDuration;
            StartCoroutine(BeginAttack(_currentWeapon));
        }
        
        public void AddItem(Item item)
        {
            var weapon = item.GetComponent<Weapon>();
            if (weapon != null)
            {
                if (!HasWeapon)
                {
                    SetCurrentWeapon(weapon.WeaponPrefab);
                }
            }
            OnObtainItem?.Invoke(item);
        }
        
        private IEnumerator BeginAttack(Weapon weapon)
        {
            GLog.Debug("[Equipment Holder] Begin Attack");
            yield return new WaitForSeconds(CurrenetAttackRampTime);
            weapon.DamageDealer.StartAttack();
            Attacking = true;
            OnWeaponHitboxStart?.Invoke(weapon);
            StartCoroutine(EndAttack(weapon));
        }

        private IEnumerator EndAttack(Weapon weapon)
        {
            var weaponData = ConfigManager.Weapons.ByType(weapon.WeaponPrefab);
            yield return new WaitForSeconds(CurrentHitboxDurationTime);
            Attacking = false;
            weapon.DamageDealer.DisableAttack();
            OnWeaponHitboxEnd?.Invoke(weapon);
        }

        public void SetCurrentWeapon(WeaponPrefab? weapon)
        {
            OnWeaponChange(weapon);
        }

        public void AttachToAnchor(GameObject o, GameObject anchor)
        {
            o.transform.SetParent(anchor.transform);
            o.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void OnWeaponChange(WeaponPrefab? newWeapon)
        {
            GLog.Debug($"[Equipment Holder] Equipping {newWeapon}");
            OnWeaponChanged?.Invoke(newWeapon);
            if (!newWeapon.HasValue)
            {
                foreach (Transform c in WeaponInactive.transform) Destroy(c.gameObject);
                return;
            }
            _ = Main.Services.Assets.InstantiateWeaponPrefabAsync(newWeapon.Value, o =>
            {
                var weapon = o.GetComponent<Weapon>();
                _currentWeapon = weapon;
                foreach (Transform c in WeaponInactive.transform) Destroy(c.gameObject);
                if (Main.Services.Data.CurrentWeapon() != newWeapon)
                {
                    return;
                }
                weapon.SetupDamageDealer(_attacker);
                AttachToAnchor(o, WeaponInactive.gameObject);
            });
        }

        private float _anumMultHack = 1f;

        public float CurrentAttackDuration =>
            CurrentWeapon.WeaponData.AttackDurationSeconds * _anumMultHack;

        public float CurrenetAttackRampTime => CurrentWeapon.WeaponData.RampTime * _anumMultHack;

        public float CurrentHitboxDurationTime => CurrentWeapon.WeaponData.HitboxDuration * _anumMultHack;

    }
}