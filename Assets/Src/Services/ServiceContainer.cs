using System;
using System.Collections.Generic;
using GameAddressables;
using Src.Services;
using UnityEditor;

namespace Src
{
	public interface IService
	{
		void OnSceneChange();
	}

	public interface IAwakeService : IService
	{
		void OnAwake();
	}
	
	public class ServiceContainer
	{
		private Dictionary<Type,  IService> _services = new();
		private List<IUpdatedService> _updated = new();
		
		public void OnSceneChange()
		{
			GLog.Debug("Services changing scene");
			foreach(var s in _services.Values) s.OnSceneChange();
		}
		
		public T Resolve<T>()
		{
			if (!_services.TryGetValue(typeof(T), out var s)) throw new Exception($"Service {nameof(T)} not reg");
			return (T)s;
		}
		
		public void AddService(IService service)
		{
			_services.Add(service.GetType(), service);
			if (service is IUpdatedService up)
			{
				_updated.Add(up);
			}
		}

		public void Update()
		{
			foreach (var s in _updated)
			{
				s.Update();
			}
		}
	}
}