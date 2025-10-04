using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Showcase.Scripts.MainMenu.UI
{
    [DisallowMultipleComponent]
    public sealed class ExitButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}