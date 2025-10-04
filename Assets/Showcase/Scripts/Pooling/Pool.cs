using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.Core.Extensions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Showcase.Scripts.Pooling
{
    /// <summary>
    /// Generic object pool implementation
    /// </summary>
    public sealed class Pool
    {
        private readonly HashSet<PoolObject> _activeObjects = new HashSet<PoolObject>();
        private readonly Queue<PoolObject> _pooledObjects = new Queue<PoolObject>();

        private readonly PoolConfig _config;
        private readonly Transform _parent;

        private Task _loadPrefabTask;

        private AsyncOperationHandle<GameObject> _prefabHandle;
        private GameObject _prefabAsset;
        private bool _prefabLoaded;

        public GameObject PrefabAsset => _prefabAsset;

        public int ActiveObjectsNumber => _activeObjects.Count;
        public int InactiveObjectsNumber => _pooledObjects.Count;

        public Pool(PoolConfig config, Transform parent = null)
        {
            _config = config;
            _parent = parent;
        }

        public Task InitAsync(CancellationToken cancellationToken)
        {
            return _loadPrefabTask ??= LoadPrefab(cancellationToken);
        }

        public PoolObject Get(Vector3 position, Quaternion rotation = default)
        {
            PoolObject poolObject = _pooledObjects.Count > 0 ? _pooledObjects.Dequeue() : CreateNewObject();

            _activeObjects.Add(poolObject);

            rotation = rotation == default ? Quaternion.identity : rotation;

            poolObject.transform.position = position;
            poolObject.transform.rotation = rotation;

            poolObject.Activate();

            return poolObject;
        }

        public void Return(PoolObject poolObject)
        {
            if (poolObject == null)
            {
                Debug.LogWarning($"[{nameof(Pool)}] Trying to return null object to pool");
                return;
            }

            _activeObjects.Remove(poolObject);

            poolObject.Deactivate();
            _pooledObjects.Enqueue(poolObject);
        }

        private void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var poolObject = CreateNewObject();
                poolObject.Deactivate();
                _pooledObjects.Enqueue(poolObject);
            }
        }

        public void Dispose()
        {
            foreach (var activeObject in _activeObjects)
            {
                if (activeObject == null)
                {
                    continue;
                }

                activeObject.ReturnedToPool -= Return;
                Object.Destroy(activeObject.gameObject);
            }

            _activeObjects.Clear();

            while (_pooledObjects.Count > 0)
            {
                var activeObject = _pooledObjects.Dequeue();
                if (activeObject == null)
                {
                    continue;
                }

                activeObject.ReturnedToPool -= Return;
                Object.Destroy(activeObject.gameObject);
            }

            if (_prefabLoaded)
            {
                Addressables.Release(_prefabHandle);
                _prefabAsset = null;
                _prefabLoaded = false;
            }
        }

        private PoolObject CreateNewObject()
        {
            if (!_prefabAsset.TryGetComponent(out PoolObject poolObject))
            {
                Debug.LogError(
                    $"[{nameof(Pool)}] Object doesn't have a PoolObject component! Type: {_config.PoolType}",
                    _config);
            }

            var poolObjectInstance = Object.Instantiate(poolObject, _parent);
            poolObjectInstance.Initialize(_config);
            poolObjectInstance.ReturnedToPool += Return;

            return poolObjectInstance;
        }

        public void ReturnAllActiveObjects()
        {
            var activeObjectsCopy = new HashSet<PoolObject>(_activeObjects);

            foreach (var activeObject in activeObjectsCopy)
            {
                if (activeObject != null)
                {
                    activeObject.ForceReturnToPool();
                }
            }
        }

        private async Task LoadPrefab(CancellationToken cancellationToken)
        {
            if (_prefabLoaded)
            {
                return;
            }

            var assetReference = _config.PrefabReference;
            if (assetReference == null || !assetReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"[{nameof(Pool)}] Invalid Addressable reference for {_config.PoolType}", _config);
                return;
            }

            _prefabHandle = assetReference.LoadAssetAsync<GameObject>();

            GameObject prefab;
            try
            {
                prefab = await _prefabHandle.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(Pool)}] Exception while loading prefab for {_config.PoolType}: {e.Message}",
                    _config);
                return;
            }

            cancellationToken
                .ThrowIfCancellationRequested();

            if (!_prefabHandle.IsValid() || _prefabHandle.Status != AsyncOperationStatus.Succeeded || prefab == null)
            {
                Debug.LogError(
                    $"[{nameof(Pool)}] Failed to load prefab asset for {_config.PoolType} (status: {_prefabHandle.Status})",
                    _config);
                return;
            }

            _prefabAsset = prefab;
            _prefabLoaded = true;

            Prewarm(_config.InitialSize);
        }
    }
}