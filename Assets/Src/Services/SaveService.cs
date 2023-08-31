using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using GameAddressables;
using JetBrains.Annotations;
using Src.Data;
using UnityEngine;

namespace Src.Services
{
	[Serializable]
	public class SerializedLoadout
	{
		public WeaponPrefab[] Weapons;
		public WeaponPrefab? EquippedWeapon;
	}

	public class SaveService : IService
	{
		public static BinaryFormatter bf = new ();

		public event Action<PlayerData> OnLoadFinished;

		private PlayerDataService DataStore => (PlayerDataService)Main.Services.Data;
		public void SaveGame()
		{
			GLog.Debug("Saving Game");
			SaveFile("Save", DataStore.Store);
		}
		
		public void OnSceneChange() {}
		
		public void LoadGame()
		{
			var load = LoadFile("Save") as PlayerData;
			//load = load ?? new PlayerData() { Loadout = new Loadout() { CurrentWeapon = WeaponPrefab.StarterSword }};
			OnLoadFinished?.Invoke(load);
		}

		public static void SaveFile (string prefKey, object serializableObject)
		{
			MemoryStream memoryStream = new MemoryStream ();
			bf.Serialize (memoryStream, serializableObject);
			string tmp = System.Convert.ToBase64String (memoryStream.ToArray ());
			PlayerPrefs.SetString (prefKey, tmp);
		}
   
		public static object LoadFile (string prefKey)
		{
			string tmp = PlayerPrefs.GetString (prefKey, string.Empty);
			if (tmp == string.Empty )
				return null;
			MemoryStream memoryStream = new MemoryStream (System.Convert.FromBase64String (tmp));
			return bf.Deserialize (memoryStream);
		}
	}
}