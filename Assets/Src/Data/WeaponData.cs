using System;
using GameAddressables;

namespace Src.Data
{
	[Serializable]
	public class WeaponData 
	{
		public WeaponPrefab Prefab;
		public float AttackDurationSeconds = 0.3f;
		public float RampTime = 0.15f;
		public float HitboxDuration = 0.15f;
		public short Damage = 1;
	}
}