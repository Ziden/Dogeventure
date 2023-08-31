using System;
using System.Collections.Generic;
using GameAddressables;
using Unity.VisualScripting;
using UnityEngine;

namespace Src.Services
{
	public class VfxService : IService
	{
		private Dictionary<VfxPrefab, List<Vfx>> _pool = new();

		public void OnSceneChange()
		{
			_pool.Clear();
			GLog.Debug("CLEAR VFX POOL");
		}

		public void Play(VfxPrefab effect, Vector3 position, Action<Vfx> callback = null)
		{
			Play(effect, position, 1f, callback);
		}

		public void Play(VfxPrefab effect, Vector3 position, float size, Action<Vfx> callback = null)
		{
			if (_pool.TryGetValue(effect, out var pooled) && pooled.Count > 0)
			{
				var fx = pooled[0];
				pooled.Remove(fx);
				fx.StartEffect(position);
				if(fx.transform.localScale.x != size)
					fx.transform.localScale = new Vector3(size, size, size);
				callback?.Invoke(fx);
			}
			else
			{
				Main.Services.Assets.InstantiateVfxPrefabAsync(effect, o =>
				{
					GLog.Debug("NEW VFX");
					var fx = o.GetOrAddComponent<Vfx>();
					fx.Effect = effect;
					if(!_pool.ContainsKey(effect)) _pool.Add(effect, new List<Vfx>());
					_pool[effect].Add(fx);
					if(fx.transform.localScale.x != size)
						fx.transform.localScale = new Vector3(size, size, size);
					fx.StartEffect(position);
					callback?.Invoke(fx);
				});
			}
		}

		public void ReturnToPool(Vfx vfx)
		{
			if(!_pool.ContainsKey(vfx.Effect)) _pool.Add(vfx.Effect, new List<Vfx>());
			_pool[vfx.Effect].Add(vfx);
		}
	}
}