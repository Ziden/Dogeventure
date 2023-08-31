using System.Collections.Generic;
using GameAddressables;
using Src.Data;
using UnityEngine;


[CreateAssetMenu]
public class WeaponConfig : ScriptableObject
{
	public WeaponData [] Data;

	private Dictionary<WeaponPrefab, WeaponData> _cache;

	public WeaponData ByType(WeaponPrefab type)
	{
		if(_cache == null) BuildCache();
		return _cache[type];
	}

	private void BuildCache()
	{
		_cache = new Dictionary<WeaponPrefab, WeaponData>();
		foreach (var c in Data)
		{
			_cache[c.Prefab] = c;
		}
	}
}