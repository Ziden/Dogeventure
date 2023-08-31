using System;
using System.Collections.Generic;
using GameAddressables;

namespace Src.Data
{
	[Serializable]
	public class Loadout
	{
		public List<WeaponPrefab> Weapons = new();

		public WeaponPrefab? CurrentWeapon;
	}

}