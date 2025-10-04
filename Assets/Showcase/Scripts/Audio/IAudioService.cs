namespace Showcase.Scripts.Audio
{
    public interface IAudioService
    {
        public void PlaySfx(AudioClipTag tag);
        public void PlayMusic(AudioClipTag tag);
        public void SetVolume(AudioMixerParameterType parameter, float volume);
        public float GetVolume(AudioMixerParameterType parameter);
    }
}