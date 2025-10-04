using System;
using Showcase.Scripts.Audio;
using Showcase.Scripts.UI.Controls;
using UnityEngine;

namespace Showcase.Scripts.MainMenu.Settings
{
    [DisallowMultipleComponent]
    public sealed class VolumeSlider : SliderView
    {
        public event Action<AudioMixerParameterType, float> VolumeValueChanged;

        [SerializeField] private AudioMixerParameterType mixerParameterType;
        public AudioMixerParameterType MixerParameterType => mixerParameterType;

        protected override void UpdateText(float value)
        {
            var percentage = Mathf.RoundToInt(value * 100);
            valueText.text = $"{percentage}";
        }

        protected override void HandleValueChanged(float value)
        {
            base.HandleValueChanged(value);

            VolumeValueChanged?.Invoke(mixerParameterType, value);
        }
    }
}