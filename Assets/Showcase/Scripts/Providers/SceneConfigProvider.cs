using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.SceneFlow;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Showcase.Scripts.Providers
{
    public sealed class SceneConfigProvider
    {
        private readonly string _label = "Scene Configs";

        private AsyncOperationHandle<IList<SceneConfig>> _handle;
        private Dictionary<SceneType, SceneConfig> _configs;

        public async Task InitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _handle = Addressables.LoadAssetsAsync<SceneConfig>(_label, null);

            IList<SceneConfig> iList;
            try
            {
                iList = await _handle.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(SceneConfigProvider)}] '{_label}' failed: {e.Message}");
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (!_handle.IsValid() || _handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[{nameof(SceneConfigProvider)}] '{_label}' status: {_handle.Status}.");
                return;
            }

            if (iList == null || iList.Count == 0)
            {
                Debug.LogError($"[{nameof(SceneConfigProvider)}] '{_label}' returned no assets.");
                return;
            }

            var sceneConfigs = new List<SceneConfig>(iList);
            _configs = sceneConfigs.ToDictionary(config => config.SceneType, config => config);
        }

        public string GetSceneName(SceneType sceneType)
        {
            return _configs[sceneType].SceneName;
        }
    }
}