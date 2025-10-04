using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.Core.Colors;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Showcase.Scripts.Providers
{
    public sealed class ColorConfigProvider
    {
        private readonly string _label = "Color Configs";

        private AsyncOperationHandle<IList<ColorConfig>> _handle;
        private Dictionary<ColorPaletteType, ColorConfig> _configs;

        public async Task InitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _handle = Addressables.LoadAssetsAsync<ColorConfig>(_label, null);

            IList<ColorConfig> iList;
            try
            {
                iList = await _handle.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(ColorConfigProvider)}] '{_label}' failed: {e.Message}");
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (!_handle.IsValid() || _handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[{nameof(ColorConfigProvider)}] '{_label}' status: {_handle.Status}.");
                return;
            }

            if (iList == null || iList.Count == 0)
            {
                Debug.LogError($"[{nameof(ColorConfigProvider)}] '{_label}' returned no assets.");
                return;
            }

            var colorPaletteConfigs = new List<ColorConfig>(iList);
            _configs = colorPaletteConfigs.ToDictionary(config => config.ColorPaletteType, config => config);
        }

        public ColorConfig GetConfig(ColorPaletteType paletteType)
        {
            if (!_configs.TryGetValue(paletteType, out var config))
            {
                Debug.LogError(
                    $"[{nameof(ColorConfigProvider)}] Color palette config not found for paletteType: {paletteType}");
                return null;
            }

            return config;
        }

        public Color GetRandomColor(ColorPaletteType paletteType)
        {
            var config = GetConfig(paletteType);

            return config.Colors[Random.Range(0, config.Colors.Length)];
        }
    }
}