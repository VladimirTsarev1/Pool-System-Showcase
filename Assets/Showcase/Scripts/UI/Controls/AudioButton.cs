using Showcase.Scripts.Audio;
using Showcase.Scripts.Core.Patterns;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Showcase.Scripts.UI.Controls
{
    [DisallowMultipleComponent]
    public sealed class AudioButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField] private bool playOnEnter = true;
        [SerializeField] private bool playOnClick = true;

        [SerializeField] private AudioClipTag enterSound = AudioClipTag.ButtonEnter;
        [SerializeField] private AudioClipTag clickSound = AudioClipTag.PositiveButtonClick;

        private IAudioService _audioService;

        private void Awake()
        {
            _audioService = ServiceLocator.Resolve<IAudioService>();

            Assert.IsNotNull(_audioService, $"[{nameof(AudioButton)}] {nameof(_audioService)} is null");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!playOnEnter)
            {
                return;
            }

            _audioService.PlaySfx(enterSound);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!playOnClick)
            {
                return;
            }

            _audioService.PlaySfx(clickSound);
        }
    }
}