
using GameAddressables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace EasyAddressables
{
    internal class AssetContainer<K, T> where K : IComparable, IFormattable, IConvertible
    {
        private Dictionary<string, AsyncOperationHandle<T>> _loaded = new Dictionary<string, AsyncOperationHandle<T>>();

        private static GameObject _spawner;

        private Transform GetSpawner()
        {
            if (_spawner == null)
            {
                _spawner = new GameObject("Spawner");
                _spawner.transform.position = new Vector3(0, -500, 0);
            }
            return _spawner.transform;
        }
        
        public async Task LoadAsync(K key, Action<T> onComplete)
        {
            if (!typeof(K).IsEnum)
                throw new Exception("Not enum parameter");

            var i = Convert.ToInt32(key);
            if (!AddressIdMap.IdMap.TryGetValue(i, out var addr))
            {
                throw new Exception("Could not find asset address for " + key);
            }
            await LoadAsync(addr, onComplete);
        }

        public async Task LoadAsync(string address, Action<T> onComplete)
        {
            if (_loaded.TryGetValue(address, out var handle))
            {
                if (!handle.IsValid())
                {
                    return;
                }
                onComplete?.Invoke(handle.Result);
                return;
            }
            handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;
            _loaded[address] = handle;
            onComplete?.Invoke(handle.Result);
        }


        public async Task InstantiateAsync<K>(K key, Action<GameObject> onComplete) where K : IComparable, IFormattable, IConvertible
        {
            var scene = SceneManager.GetActiveScene().name;
            var handle = Addressables.InstantiateAsync(GetAddress(key), GetSpawner());
            await handle.Task;
            if(SceneManager.GetActiveScene().name == scene && handle.Result != null)
                onComplete?.Invoke(handle.Result);
        }

        public async Task InstantiateAsync(string address, Action<GameObject> onComplete)
        {
            var scene = SceneManager.GetActiveScene().name;
            var handle = Addressables.InstantiateAsync(address, GetSpawner());
            await handle.Task;
            if(SceneManager.GetActiveScene().name == scene && handle.Result != null)
                onComplete?.Invoke(handle.Result);
        }

        private string GetAddress<K>(K key) where K : IComparable, IFormattable, IConvertible
        {
            if (!typeof(K).IsEnum)
                throw new Exception("Not enum parameter");

            var i = Convert.ToInt32(key);
            if (!AddressIdMap.IdMap.TryGetValue(i, out var addr))
            {
                throw new Exception("Could not find asset address for " + key);
            }
            return addr;
        }
    }
}
