using GameAddressables;
using Src.MonoComponent;
using Src.Services;
using UnityEditor;
using UnityEngine;

class MyWindow : EditorWindow {
	[MenuItem ("Game/Save")]

	public static void  ShowWindow () {
		Main.Services.Save.SaveGame();
	}
	
	[MenuItem ("Game/Fix")]

	public static void  Fix ()
	{
		foreach (var b in (FindObjectsOfType(typeof(Bush)) as Bush[]))
		{
			if (b.DestroyEffect != VfxPrefab.HitOnlyLeaves && b.DestroyEffect != VfxPrefab.HitOnlyLeaves)
			{
				b.DestroyEffect = VfxPrefab.HitOnlyLeaves;
				EditorUtility.SetDirty(b);
			}
		}
	}
}