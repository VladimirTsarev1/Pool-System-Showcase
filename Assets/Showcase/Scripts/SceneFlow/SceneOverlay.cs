using System;
using Showcase.Scripts.Core.Colors;
using Showcase.Scripts.Core.Extensions;
using Showcase.Scripts.Providers;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Showcase.Scripts.SceneFlow
{
    [DisallowMultipleComponent]
    public sealed class SceneOverlay : MonoBehaviour
    {
        public event Action HideStarted;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private SceneOverlayAnimator overlayAnimator;

        public bool IsVisible { get; private set; }
        
        private void Awake()
        {
            Assert.IsNotNull(canvasGroup, $"[{nameof(SceneOverlay)}] {nameof(canvasGroup)} is null");
            Assert.IsNotNull(backgroundImage, $"[{nameof(SceneOverlay)}] {nameof(backgroundImage)} is null");
            Assert.IsNotNull(progressBar, $"[{nameof(SceneOverlay)}] {nameof(progressBar)} is null");
            Assert.IsNotNull(progressText, $"[{nameof(SceneOverlay)}] {nameof(progressText)} is null");
            Assert.IsNotNull(overlayAnimator, $"[{nameof(SceneOverlay)}] {nameof(overlayAnimator)} is null");

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            overlayAnimator.ShowStarted += HandleShowStarted;
            overlayAnimator.ShowEnded += HandleShowEnded;

            overlayAnimator.HideStarted += HandleHideStarted;
            overlayAnimator.HideEnded += HandleHideEnded;
        }

        private void OnDestroy()
        {
            overlayAnimator.ShowStarted -= HandleShowStarted;
            overlayAnimator.ShowEnded -= HandleShowEnded;

            overlayAnimator.HideStarted -= HandleHideStarted;
            overlayAnimator.HideEnded -= HandleHideEnded;
        }

        public void Show()
        {
            backgroundImage.color =
                ConfigProvider.Colors.GetRandomColor(ColorPaletteType.SceneOverlayBackground);

            gameObject.Activate();

            overlayAnimator.PlayShowAnimation();
        }

        public void Hide()
        {
            overlayAnimator.PlayHideAnimation();
        }

        public void UpdateProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);

            if (progressBar)
            {
                progressBar.fillAmount = progress;
            }

            if (progressText)
            {
                var percentage = Mathf.RoundToInt(progress * 100);
                progressText.text = $"{percentage}%";
            }
        }

        #region Animator

        private void HandleShowStarted()
        {
            IsVisible = false;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            if (progressBar)
            {
                progressBar.fillAmount = 0f;
            }

            if (progressText)
            {
                progressText.text = "0%";
            }
        }

        private void HandleShowEnded()
        {
            IsVisible = true;
        }

        private void HandleHideStarted()
        {
            HideStarted?.Invoke();
        }

        private void HandleHideEnded()
        {
            IsVisible = false;

            gameObject.Deactivate();
        }

        #endregion
    }
}