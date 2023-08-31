using System.Collections.Generic;
using System.Linq;
using GameAddressables;
using Src.Data;
using Src.MonoComponent;
using UnityEngine;

namespace Src.Services
{
	public interface IPlayerData
	{
		public SerializableVector3 LastEntrance();
		public SerializableVector3 PlayerPosition();
		public HashSet<uint> Interacted();
		public IReadOnlyDictionary<CollectiblePrefab, ushort> Collectibles();
		public StatData Stats();
		public IReadOnlyList<WeaponPrefab> Weapons();
		public IReadOnlyCollection<ushort> Pages();
		public WeaponPrefab? CurrentWeapon();

		public DungeonKeys DungeonKeys();
	}
	
	public class PlayerDataService : IPlayerData, IService
	{
		public PlayerData Store { get; private set; }

		public SerializableVector3 LastEntrance() => Store.LastMapEntrance;
		public SerializableVector3 PlayerPosition() => Store.PlayerPosition;
		public HashSet<uint> Interacted() => Store.Interacted;
		public IReadOnlyDictionary<CollectiblePrefab, ushort> Collectibles() => Store.Collectibles;
		public StatData Stats() => Store.Stats;
		public IReadOnlyList<WeaponPrefab> Weapons() => Store.Loadout?.Weapons;
		public IReadOnlyCollection<ushort> Pages() => Store.Pages;
		public WeaponPrefab? CurrentWeapon() => Store.Loadout?.CurrentWeapon;

		public DungeonKeys DungeonKeys() => Store.DungeonKeys;
		
		public PlayerDataService()
		{
			Map.OnMapInitialized += OnMapInitialized;
			Main.Services.Save.OnLoadFinished += OnSaveLoaded;
		}

		private void OnSaveLoaded(PlayerData data)
		{
			GLog.Debug("Save Loaded to Player Data");
			Store = data ?? new PlayerData();
		}
		
		public void OnMapInitialized()
		{
			Map.Current.OnPlayerInitialized += OnPlayerInitialized;
			Map.Current.OnPlayerMoved += OnPlayerMove;
			Map.Current.OnBeforeTeleportTo += OnBeforeTeleport;
			Map.Current.OnPlayerCollect += OnCollect;
			Map.Current.OnChestOpened += OnChestOpened;
		}

		private void OnChestOpened(Chest c)
		{
			var u = c.GetComponent<UniqueObject>();
			if (u == null) return;
			Store.Interacted.Add(u.UniqueId);
			Main.Services.Save.SaveGame();
		}

		private void OnCollect(Collectible c)
		{
			if (!Store.Collectibles.ContainsKey(c.CollectibleType))
			{
				Store.Collectibles[c.CollectibleType] = new();
			}
			Store.Collectibles[c.CollectibleType]++;
			var u = c.GetComponent<UniqueObject>();
			if (u == null) return;
			Store.Interacted.Add(u.UniqueId);
		}
		
		private void OnPlayerInitialized(Player p)
		{
			p.Entity.Equips.OnWeaponChanged += OnWeaponChanged;
			p.Entity.Equips.OnObtainItem += OnObtainItem;
		}

		private void OnObtainItem(Item item)
		{
			var p = Player.Get();
			if (item.Type == ItemType.Weapon)
			{
				Store.Loadout.Weapons.Add(item.GetComponent<Weapon>().WeaponPrefab);
			} else if (item.Type == ItemType.Page)
			{
				Store.Pages.Add(item.GetComponent<Page>().PageNumber);
			} else if (item.Type == ItemType.SuperHeart)
			{
				p.Entity.Stats.MaxLife += 1;
				p.Entity.Stats.Life = p.Entity.Stats.MaxLife;
			}else if (item.Type == ItemType.DungeonKey)
			{
				Store.DungeonKeys |= item.GetComponent<DungeonKey>().Dungeon;
			}
		}

		private void OnWeaponChanged(WeaponPrefab? weapon)
		{
			Store.Loadout.CurrentWeapon = weapon;
		}

		private void OnBeforeTeleport(AssetScene newMap)
		{
			if (Map.Current.AllowSavingPosition && Player.Get().LastPositions.Count > 0)
			{
				Store.PlayerPosition = Player.Get().LastPositions.Last();
				Store.LastMapEntrance = Store.PlayerPosition;
			}
		}

		private void OnPlayerMove()
		{
			if (Map.Current.AllowSavingPosition)
			{
				Store.PlayerPosition = Player.Get().transform.position;
			}
		}

		public void OnSceneChange()
		{
		}
	}
}