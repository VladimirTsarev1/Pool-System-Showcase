using System;
using Showcase.Scripts.Core.Extensions;
using Showcase.Scripts.UI.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Showcase.Scripts.Demo1.UI.Options
{
    [DisallowMultipleComponent]
    public sealed class SwitchPanelButton : MonoBehaviour
    {
        public event Action ButtonClicked;

        [SerializeField] private Button button;
        [SerializeField] private Image showImage;
        [SerializeField] private Image hideImage;

        private void Awake()
        {
            Assert.IsNotNull(button, $"[{nameof(SwitchPanelButton)}] {nameof(button)} is null]");
            Assert.IsNotNull(showImage, $"[{nameof(SwitchPanelButton)}] {nameof(showImage)} is null]");
            Assert.IsNotNull(hideImage, $"[{nameof(SwitchPanelButton)}] {nameof(hideImage)} is null]");
        }

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClick);
        }

        public void SwitchImage(PanelState state)
        {
            if (state == PanelState.Visible)
            {
                showImage.Deactivate();
                hideImage.Activate();
            }
            else if (state == PanelState.Hidden)
            {
                hideImage.Deactivate();
                showImage.Activate();
            }
        }

        public void SetInteractableState(bool state)
        {
            button.interactable = state;
        }

        private void HandleButtonClick()
        {
            ButtonClicked?.Invoke();
        }
    }
}