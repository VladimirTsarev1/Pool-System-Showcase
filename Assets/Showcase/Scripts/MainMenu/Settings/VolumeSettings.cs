using Showcase.Scripts.Audio;
using Showcase.Scripts.Core.Patterns;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.MainMenu.Settings
{
    [DisallowMultipleComponent]
    public sealed class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private VolumeSlider[] volumeSliders;

        private IAudioService _audioService;
        
        private void Awake()
        {
            _audioService = ServiceLocator.Resolve<IAudioService>();

            Assert.IsNotNull(_audioService, $"[{nameof(VolumeSettings)}] {nameof(_audioService)} is null");
        }

        private void Start()
        {
            InitSliders();
        }

        private void OnDestroy()
        {
            foreach (var volumeSlider in volumeSliders)
            {
                volumeSlider.VolumeValueChanged -= UpdateVolumeValue;
            }
        }

        private void InitSliders()
        {
            foreach (var volumeSlider in volumeSliders)
            {
                var volumeParameterValue = _audioService.GetVolume(volumeSlider.MixerParameterType);

                volumeSlider.SetupRange(0f, 1f);
                volumeSlider.SetValue(volumeParameterValue);

                volumeSlider.VolumeValueChanged += UpdateVolumeValue;
            }
        }

        private void UpdateVolumeValue(AudioMixerParameterType mixerParameterType, float value)
        {
            _audioService.SetVolume(mixerParameterType, value);
        }
    }
}