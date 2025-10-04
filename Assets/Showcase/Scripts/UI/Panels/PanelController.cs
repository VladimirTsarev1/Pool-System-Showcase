using System;
using Showcase.Scripts.Core.Extensions;
using Showcase.Scripts.UI.Animation;
using Showcase.Scripts.UI.Controls;
using Showcase.Scripts.UI.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.UI.Panels
{
    [DisallowMultipleComponent]
    public class PanelController : MonoBehaviour
    {
        public event Action<PanelType, PanelType> TransitionedToPanel;

        [SerializeField] private PanelType panelType;
        [SerializeField] private UIAnimator animator;
        [SerializeField] private CanvasGroup canvasGroup;

        public PanelType PanelType => panelType;
        
        private void Awake()
        {
            Assert.IsNotNull(animator,  $"[{nameof(PanelController)}] {nameof(animator)} is null");
            Assert.IsNotNull(canvasGroup,  $"[{nameof(PanelController)}] {nameof(canvasGroup)} is null");

            SetupTransitionButtons();
        }

        private void OnEnable()
        {
            animator.ShowStarted += HandleShowStart;
            animator.ShowEnded += HandleShowEnded;
            animator.HideStarted += HandleHideStarted;
            animator.HideEnded += HandleHideEnded;
        }

        private void OnDisable()
        {
            if (!animator)
            {
                return;
            }

            animator.ShowStarted -= HandleShowStart;
            animator.ShowEnded -= HandleShowEnded;
            animator.HideStarted -= HandleHideStarted;
            animator.HideEnded -= HandleHideEnded;
        }

        public void Initialize()
        {
        }

        public virtual void Show()
        {
            gameObject.Activate();

            animator.PlayShowAnimation();
        }

        public virtual void Hide()
        {
            animator.PlayHideAnimation();
        }

        public void TransitionToPanel(PanelType targetPanelType)
        {
            TransitionedToPanel?.Invoke(panelType, targetPanelType);
        }

        private void SetCanvasGroupInteractable(bool state)
        {
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }

        private void SetupTransitionButtons()
        {
            var transitionButtons = GetComponentsInChildren<TransitionButton>(true);

            for (int i = 0; i < transitionButtons.Length; i++)
            {
                transitionButtons[i].Setup(this);
            }
        }

        #region Handlers

        private void HandleShowStart()
        {
            SetCanvasGroupInteractable(false);
        }

        private void HandleShowEnded()
        {
            SetCanvasGroupInteractable(true);
        }

        private void HandleHideStarted()
        {
            SetCanvasGroupInteractable(false);
        }

        private void HandleHideEnded()
        {
            gameObject.Deactivate();
        }

        #endregion
    }
}