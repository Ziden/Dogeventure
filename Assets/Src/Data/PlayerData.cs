using System;
using System.Collections.Generic;
using GameAddressables;
using UnityEngine;

namespace Src.Data
{

	[Serializable]
	public class PlayerData
	{
		public SerializableVector3 PlayerPosition = Vector3.zero;
		public SerializableVector3 LastMapEntrance = Vector3.zero;
		public HashSet<uint> Interacted = new();
		public HashSet<ushort> Pages = new();
		public DungeonKeys DungeonKeys;
		public Dictionary<CollectiblePrefab, ushort> Collectibles = new();
		public StatData Stats = new ()
		{
			Life = 3, MaxLife = 3
		};
		public Loadout Loadout = new();
	}
}