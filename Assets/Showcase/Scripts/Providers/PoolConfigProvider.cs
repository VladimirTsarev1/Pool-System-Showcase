using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.Pooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Showcase.Scripts.Providers
{
    /// <summary>
    /// Provider for pool configuration access
    /// </summary>
    public sealed class PoolConfigProvider
    {
        private readonly string _label = "Pool Configs";

        private AsyncOperationHandle<IList<PoolConfig>> _handle;
        private Dictionary<PoolType, PoolConfig> _configs;

        public async Task InitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _handle = Addressables.LoadAssetsAsync<PoolConfig>(_label, null);

            IList<PoolConfig> iList;
            try
            {
                iList = await _handle.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(PoolConfigProvider)}] '{_label}' failed: {e.Message}");
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (!_handle.IsValid() || _handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[{nameof(PoolConfigProvider)}] '{_label}' status: {_handle.Status}.");
                return;
            }

            if (iList == null || iList.Count == 0)
            {
                Debug.LogError($"[{nameof(PoolConfigProvider)}] '{_label}' returned no assets.");
                return;
            }

            var poolConfigs = new List<PoolConfig>(iList);
            _configs = poolConfigs.ToDictionary(config => config.PoolType, config => config);

#if UNITY_EDITOR
            ValidateConfigs();
#endif
        }

        public PoolConfig GetConfig(PoolType poolType)
        {
            if (!_configs.TryGetValue(poolType, out var config))
            {
                Debug.LogError($"[{nameof(PoolConfigProvider)}] Pools config not found for type: {poolType}");
                return null;
            }

            return config;
        }

        public IEnumerable<PoolType> GetAvailablePoolTypes() => _configs.Keys;

#if UNITY_EDITOR
        private void ValidateConfigs()
        {
            foreach (var (poolType, config) in _configs)
            {
                if (config.PrefabReference == null || !config.PrefabReference.RuntimeKeyIsValid())
                {
                    Debug.LogError(
                        $"[{nameof(PoolConfigProvider)}] Addressable PrefabReference is invalid for {poolType}",
                        config);
                }
            }
        }
#endif
    }
}