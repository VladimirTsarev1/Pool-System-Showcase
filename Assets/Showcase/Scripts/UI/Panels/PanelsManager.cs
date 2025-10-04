using System.Collections.Generic;
using Showcase.Scripts.Core.Extensions;
using Showcase.Scripts.UI.Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.UI.Panels
{
    [DisallowMultipleComponent]
    public sealed class PanelsManager : MonoBehaviour
    {
        [SerializeField] private PanelController defaultPanel;
        [SerializeField] private PanelController[] panels;

        private Dictionary<PanelType, PanelController> _panelsDict;
        private PanelController _currentPanel;

        private void Awake()
        {
            Assert.IsNotNull(defaultPanel, $"[{nameof(PanelsManager)}] {nameof(defaultPanel)} is null");

            InitializePanels();
        }

        private void OnDestroy()
        {
            foreach (var (_, panel) in _panelsDict)
            {
                panel.TransitionedToPanel -= HandleTransitionedToPanel;
            }
        }

        public void Initialize()
        {
            InitializePanels();

            ShowDefaultPanel();
        }

        private void InitializePanels()
        {
            _panelsDict = new Dictionary<PanelType, PanelController>();

            foreach (var panel in panels)
            {
                if (panel && panel.PanelType != PanelType.None && _panelsDict.TryAdd(panel.PanelType, panel))
                {
                    panel.Initialize();

                    panel.TransitionedToPanel += HandleTransitionedToPanel;

                    panel.Deactivate();
                }
            }
        }

        private void ShowPanel(PanelType panelType)
        {
            if (!_panelsDict.TryGetValue(panelType, out var panel))
            {
                Debug.LogWarning($"[{nameof(PanelsManager)}] Panel {panelType} not found", this);
                return;
            }

            ShowPanel(panel);
        }

        private void ShowPanel(PanelController panelController)
        {
            if (!panelController)
            {
                Debug.LogWarning($"[{nameof(PanelsManager)}] Panel is null", this);
                return;
            }

            if (_currentPanel)
            {
                _currentPanel.Hide();
            }

            _currentPanel = panelController;
            _currentPanel.Show();
        }

        private void ShowDefaultPanel()
        {
            ShowPanel(defaultPanel);
        }

        private void HandleTransitionedToPanel(PanelType originPanelType, PanelType targetPanelType)
        {
            if (_panelsDict.TryGetValue(originPanelType, out var originalPanel))
            {
                originalPanel.Hide();
            }

            if (_panelsDict.TryGetValue(targetPanelType, out var targetPanel))
            {
                targetPanel.Show();
            }
        }
    }
}