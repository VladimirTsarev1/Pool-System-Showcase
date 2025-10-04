using System;
using Showcase.Scripts.Core.Constants;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.UI.Animation
{
    [DisallowMultipleComponent]
    public class UIAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public event Action ShowStarted;
        public event Action ShowEnded;

        public event Action HideStarted;
        public event Action HideEnded;

        private void Awake() => Assert.IsNotNull(animator, $"[{nameof(UIAnimator)}] {nameof(animator)} is null");

        public void PlayShowAnimation() => animator.SetTrigger(AnimatorConstants.Show);
        public void PlayHideAnimation() => animator.SetTrigger(AnimatorConstants.Hide);

        public void OnShowStart() => ShowStarted?.Invoke();
        public void OnShowEnd() => ShowEnded?.Invoke();

        public void OnHideStart() => HideStarted?.Invoke();
        public void OnHideEnd() => HideEnded?.Invoke();
    }
}