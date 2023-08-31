using System;
using UnityEngine;

namespace Src.MonoComponent
{
	public class ConfigManager : MonoBehaviour
	{
		public CollectibleConfig CollectiblesConfig;
		public WeaponConfig WeaponsConfig;

		private static ConfigManager _instance;

		private void Start()
		{
			_instance = this;
		}

		public static CollectibleConfig Collectibles => _instance.CollectiblesConfig;
		public static WeaponConfig Weapons => _instance.WeaponsConfig;
	}
}