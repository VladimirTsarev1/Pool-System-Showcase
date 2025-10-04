using Showcase.Scripts.UI.Data;
using Showcase.Scripts.UI.Panels;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Showcase.Scripts.UI.Controls
{
    [DisallowMultipleComponent]
    public sealed class TransitionButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private PanelType targetPanelType;

        private PanelController _parentPanel;
        
        public void Setup(PanelController parentPanel)
        {
            if (!parentPanel)
            {
                Debug.LogError($"[{nameof(TransitionButton)}] parentPanel is null", this);
                return;
            }

            _parentPanel = parentPanel;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_parentPanel)
            {
                Debug.LogError($"[{nameof(TransitionButton)}] parentPanel is null", this);
                return;
            }

            _parentPanel.TransitionToPanel(targetPanelType);
        }
    }
}