using System;
using System.Collections.Generic;
using Src.Data;
using Src.Services;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteAlways]
public class UniqueObject : MonoBehaviour
{
	public uint UniqueId;

	public bool HasAlreadyInteractedWith()
	{
		return Main.Services.Data.Interacted().Contains(UniqueId);
	}

#if UNITY_EDITOR
	
	private static Dictionary<ulong, GameObject> Objects = new();
	private static System.Random rnd = new ();
	private static byte[] _bytes = new byte[4];

	public uint Generate()
	{
		uint n = 0;
		while (n == 0 || Objects.ContainsKey(n))
		{
			rnd.NextBytes(_bytes);
			n = BitConverter.ToUInt16(_bytes, 0);
		}
		return n;
	}

	void OnDestroy()
	{
		Objects.Remove(UniqueId);
	}
	
	private void Check()
	{
		if(!EditorApplication.isPlaying)
		{
			if (UniqueId != 0 && Objects.TryGetValue(UniqueId, out var o) && o == gameObject)
			{
				return;
			}

			if (UniqueId == 0)
			{
				UniqueId = Generate();
				EditorUtility.SetDirty(this);
				GLog.Debug("Assigned Unique ID " + UniqueId+" to object " + gameObject.name);
			}
			else
			{
				GLog.Debug("Loaded Unique ID " + UniqueId+" to object " + gameObject.name);
			}
			if (Objects.ContainsKey(UniqueId))
			{
				Debug.LogError("Object "+Objects[UniqueId].name+" collided id "+UniqueId+" with "+gameObject.name+ "regenerating id");
				UniqueId = Generate();
				EditorUtility.SetDirty(this);
			}
			Objects[UniqueId] = gameObject;
		}
	}
	
	void Start()
	{
		Check();
	}
#endif
}
