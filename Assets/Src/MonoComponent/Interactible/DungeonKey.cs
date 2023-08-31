
using System;
using Unity.VisualScripting;
using UnityEngine;

[Flags]
public enum DungeonKeys : byte
{
	None, D1, D2, D3, D4, D5, D6, D7, D8
}

public class DungeonKey : MonoBehaviour
{
	public DungeonKeys Dungeon;

	void Awake()
	{
		var item = this.GetOrAddComponent<Item>();
		item.Type = ItemType.DungeonKey;
	}
}
