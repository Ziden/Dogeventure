

using System;
using System.Collections.Generic;
using GameAddressables;
using Src.MonoComponent;
using UnityEngine;


[Serializable]
public class CollectibleData
{
	public CollectiblePrefab Type;
	public string Name;
	public Sprite Sprite;
}

[CreateAssetMenu]
public class CollectibleConfig : ScriptableObject
{
	public CollectibleData [] Collectibles;

	private Dictionary<CollectiblePrefab, CollectibleData> _cache;

	public CollectibleData ByType(CollectiblePrefab type)
	{
		if(_cache == null) BuildCache();
		return _cache[type];
	}

	private void BuildCache()
	{
		_cache = new Dictionary<CollectiblePrefab, CollectibleData>();
		foreach (var c in Collectibles)
		{
			_cache[c.Type] = c;
		}
	}
}