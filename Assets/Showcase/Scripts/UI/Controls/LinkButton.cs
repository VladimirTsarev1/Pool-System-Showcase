using UnityEngine;
using UnityEngine.EventSystems;

namespace Showcase.Scripts.UI.Controls
{
    [DisallowMultipleComponent]
    public sealed class LinkButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string url;

        private bool _urlIsIsNullOrEmpty;

        private void Awake()
        {
            _urlIsIsNullOrEmpty = string.IsNullOrEmpty(url) || string.IsNullOrWhiteSpace(url);

            if (_urlIsIsNullOrEmpty)
            {
                Debug.LogWarning($"[{nameof(LinkButton)}] {nameof(url)} is null or empty");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_urlIsIsNullOrEmpty)
            {
                return;
            }

            Application.OpenURL(url);
        }
    }
}