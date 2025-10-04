using System;
using Showcase.Scripts.Core.Constants;
using UnityEngine;

namespace Showcase.Scripts.Audio
{
    [CreateAssetMenu(fileName = "Audio Config", menuName = PathConstants.ScriptableObjects + "/Audio Config")]
    public sealed class AudioConfig : ScriptableObject
    {
        [SerializeField] private AudioClipData[] audioClipDatas;
        public AudioClipData[] AudioClipDatas => audioClipDatas;
    }

    [Serializable]
    public class AudioClipData
    {
        [SerializeField] private AudioClipTag audioClipTag;
        [SerializeField] private AudioClip audioClip;

        public AudioClipTag AudioClipTag => audioClipTag;
        public AudioClip AudioClip => audioClip;
    }
}