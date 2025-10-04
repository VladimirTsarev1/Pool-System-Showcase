using System;
using Showcase.Scripts.Core.Constants;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Providers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace Showcase.Scripts.Audio
{
    [DisallowMultipleComponent]
    public sealed class AudioManager : BaseSingleton<AudioManager>, IAudioService
    {
        [SerializeField] private AudioMixer masterMixer;

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        protected override void OnSingletonAwake()
        {
            Assert.IsNotNull(masterMixer, $"[{nameof(AudioManager)}] {nameof(masterMixer)} is null");
            Assert.IsNotNull(musicSource, $"[{nameof(AudioManager)}] {nameof(musicSource)} is null");
            Assert.IsNotNull(sfxSource, $"[{nameof(AudioManager)}] {nameof(sfxSource)} is null");
        }

        public void PlaySfx(AudioClipTag clipTag)
        {
            var audioClip = ConfigProvider.Audio.GetAudioClip(clipTag);

            sfxSource.PlayOneShot(audioClip);
        }

        public void PlayMusic(AudioClipTag clipTag)
        {
            var audioClip = ConfigProvider.Audio.GetAudioClip(clipTag);

            if (musicSource.clip == audioClip)
            {
                return;
            }

            musicSource.clip = audioClip;
            musicSource.Play();
        }
        
        #region Set Volume

        public void SetVolume(AudioMixerParameterType parameter, float volume)
        {
            float dB = volume > 0.001f ? Mathf.Log10(volume) * 20 : -80f;

            switch (parameter)
            {
                case AudioMixerParameterType.MasterVolume:
                    SetMasterVolume(dB);
                    break;
                case AudioMixerParameterType.MusicVolume:
                    SetMusicVolume(dB);
                    break;
                case AudioMixerParameterType.SfxVolume:
                    SetSfxVolume(dB);
                    break;
            }
        }

        private void SetMasterVolume(float dB)
        {
            masterMixer.SetFloat(AudioConstants.MasterVolume, dB);
        }

        private void SetMusicVolume(float dB)
        {
            masterMixer.SetFloat(AudioConstants.MusicVolume, dB);
        }

        private void SetSfxVolume(float dB)
        {
            masterMixer.SetFloat(AudioConstants.SfxVolume, dB);
        }

        #endregion

        #region Get Volume

        public float GetVolume(AudioMixerParameterType parameter)
        {
            string parameterName = parameter switch
            {
                AudioMixerParameterType.MasterVolume => AudioConstants.MasterVolume,
                AudioMixerParameterType.MusicVolume => AudioConstants.MusicVolume,
                AudioMixerParameterType.SfxVolume => AudioConstants.SfxVolume,
                _ => throw new ArgumentException($"Unknown mixer: {parameter}")
            };

            masterMixer.GetFloat(parameterName, out float dB);
            return dB <= -80f ? 0f : Mathf.Pow(10, dB / 20);
        }

        #endregion
    }
}