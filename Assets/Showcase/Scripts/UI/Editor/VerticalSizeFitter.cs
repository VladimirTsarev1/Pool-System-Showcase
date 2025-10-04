using UnityEngine;

namespace Showcase.Scripts.UI.Editor
{
    [DisallowMultipleComponent]
    public sealed class VerticalSizeFitter : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var rectTransform = GetComponent<RectTransform>();

            var sizeY = 0f;

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (rectTransform.GetChild(i).TryGetComponent(out RectTransform childRectTransform))
                {
                    sizeY += childRectTransform.sizeDelta.y;
                }
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, sizeY);
        }
#endif
    }
}