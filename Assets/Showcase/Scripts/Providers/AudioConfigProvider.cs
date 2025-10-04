using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Showcase.Scripts.Providers
{
    /// <summary>
    /// Provider for pool configuration access
    /// </summary>
    public sealed class AudioConfigProvider
    {
        private readonly string _label = "Audio Configs";

        private AsyncOperationHandle<IList<AudioConfig>> _handle;
        private Dictionary<AudioClipTag, AudioClip> _audioClips;

        public async Task InitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _handle = Addressables.LoadAssetsAsync<AudioConfig>(_label, null);

            IList<AudioConfig> iList;
            try
            {
                iList = await _handle.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(AudioConfigProvider)}] '{_label}' failed: {e.Message}");
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (!_handle.IsValid() || _handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[{nameof(AudioConfigProvider)}] '{_label}' status: {_handle.Status}.");
                return;
            }

            if (iList == null || iList.Count == 0)
            {
                Debug.LogError($"[{nameof(AudioConfigProvider)}] '{_label}' returned no assets.");
                return;
            }

            var audioConfigs = new List<AudioConfig>(iList);
            var audioList = new List<AudioClipData>();

            foreach (var config in audioConfigs)
            {
                audioList.AddRange(config.AudioClipDatas);
            }

            _audioClips = audioList.ToDictionary(config => config.AudioClipTag, config => config.AudioClip);
        }

        public AudioClip GetAudioClip(AudioClipTag tag)
        {
            if (!_audioClips.TryGetValue(tag, out var audioClip))
            {
                Debug.LogError($"[{nameof(AudioConfigProvider)}] No audio clip with tag: {tag}");
                return null;
            }

            return audioClip;
        }
    }
}