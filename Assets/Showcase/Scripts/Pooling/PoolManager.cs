using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Providers;
using UnityEngine;

namespace Showcase.Scripts.Pooling
{
    /// <summary>
    /// Central manager for all object pools
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PoolManager : BaseSingleton<PoolManager>, IPoolService
    {
        private readonly Dictionary<PoolType, Pool> _pools = new Dictionary<PoolType, Pool>();

        private Task _initTask;
        private Transform _container;

        protected override void OnDestroy()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Dispose();
            }

            _pools.Clear();

            base.OnDestroy();
        }

        public Task InitAsync(CancellationToken cancellationToken)
        {
            return _initTask ??= InitInternalAsync(cancellationToken);
        }

        public T GetPoolObject<T>
            (PoolType poolType, Vector3 position, Quaternion rotation = default, float returnTime = float.NaN)
            where T : Component
        {
            if (!_pools.TryGetValue(poolType, out var pool))
            {
                Debug.LogError($"[{nameof(PoolManager)}] Pools not found for type: {poolType}");

                return null;
            }

            var poolObject = pool.Get(position, rotation);

            if (poolObject == null)
            {
                Debug.LogError($"[{nameof(PoolManager)}] There is not a pool object for type: {poolType}");

                return null;
            }

            if (!poolObject.TryGetComponent(out T component))
            {
                Debug.LogError($"[{nameof(PoolManager)}] Pools object isn't contains a component: {typeof(T)}");

                return null;
            }

            poolObject.OnGet(returnTime);

            return component;
        }

        public Pool GetPool(PoolType poolType)
        {
            return _pools.GetValueOrDefault(poolType);
        }

        public GameObject GetPrefab(PoolType poolType)
        {
            if (_pools.TryGetValue(poolType, out var pool))
            {
                return pool.PrefabAsset;
            }

            return null;
        }

        public void ReleaseAllPools()
        {
            foreach (var (_, pool) in _pools)
            {
                pool.ReturnAllActiveObjects();
            }
        }

        private async Task InitInternalAsync(CancellationToken cancellationToken)
        {
            _container = new GameObject("Pools").transform;
            DontDestroyOnLoad(_container);

            var poolTypes = ConfigProvider.Pools.GetAvailablePoolTypes();
            var poolTasks = new List<Task>();

            foreach (var poolType in poolTypes)
            {
                var pool = CreatePool(poolType);
                if (pool != null)
                {
                    poolTasks.Add(pool.InitAsync(cancellationToken));
                }
            }

            await Task.WhenAll(poolTasks);
        }

        private Pool CreatePool(PoolType poolType)
        {
            if (_pools.ContainsKey(poolType))
            {
                Debug.LogWarning(
                    $"[{nameof(PoolManager)}] Pools for type '{poolType}' already exists! Skipping creation.");
                return null;
            }

            var config = ConfigProvider.Pools.GetConfig(poolType);
            if (config == null)
            {
                return null;
            }

            var holderObject = new GameObject($"{poolType}Pools");
            holderObject.transform.SetParent(_container);

            var pool = new Pool(config, holderObject.transform);
            _pools[poolType] = pool;

            Debug.Log(
                $"[{nameof(PoolManager)}] Created pool for {poolType} with {config.InitialSize} pre-warmed objects");

            return pool;
        }
    }
}