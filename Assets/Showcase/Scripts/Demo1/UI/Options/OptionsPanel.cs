using System;
using System.Globalization;
using System.Linq;
using Showcase.Scripts.Demo1.SpawnObjects;
using Showcase.Scripts.Pooling;
using Showcase.Scripts.UI.Controls;
using Showcase.Scripts.UI.Data;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Showcase.Scripts.Demo1.UI.Options
{
    [DisallowMultipleComponent]
    public sealed class OptionsPanel : MonoBehaviour
    {
        #region Events

        public event Action<bool> UsePoolChanged;
        public event Action<bool> CollisionsChanged;
        public event Action<bool> ShowStatsChanged;
        public event Action<int> SpawnTypeChanged;
        public event Action<int> DespawnPolicyChanged;
        public event Action<float> ObjectLifetimeChanged;
        public event Action<float> SpawnRateChanged;
        public event Action<float> InitialVelocityChanged;
        public event Action<float> ObjectSizeChanged;
        public event Action ClearClicked;

        #endregion

        [SerializeField] private Demo1InitialSettingsConfig initialSettings;
        [SerializeField] private OptionsPanelAnimator animator;
        [SerializeField] private SwitchPanelButton switchPanelButton;

        [Space, SerializeField] private Toggle usePoolToggle;
        [SerializeField] private Toggle collisionsToggle;
        [SerializeField] private Toggle showStatsToggle;

        [SerializeField] private DropdownView spawnTypeDropdown;
        [SerializeField] private DropdownView despawnPolicyDropdown;

        [SerializeField] private SliderView objectLifetimeSlider;
        [SerializeField] private SliderView spawnRateSlider;
        [SerializeField] private SliderView initialVelocitySlider;
        [SerializeField] private SliderView objectSizeSlider;

        [SerializeField] private Button clearButton;

        private PanelState _panelState = PanelState.Visible;

        private void Awake()
        {
            Assert.IsNotNull(initialSettings, $"[{nameof(OptionsPanel)}] {nameof(initialSettings)} is null");
            Assert.IsNotNull(animator, $"[{nameof(OptionsPanel)}] {nameof(animator)} is null");

            animator.ShowStarted += HandleShowStarted;
            animator.ShowEnded += HandleShowEnded;

            animator.HideStarted += HandleHideStarted;
            animator.HideEnded += HandleHideEnded;
        }

        private void OnDestroy()
        {
            if (animator)
            {
                animator.ShowStarted -= HandleShowStarted;
                animator.ShowEnded -= HandleShowEnded;

                animator.HideStarted -= HandleHideStarted;
                animator.HideEnded -= HandleHideEnded;
            }

            if (switchPanelButton)
            {
                switchPanelButton.ButtonClicked -= HandleClearClicked;
            }

            if (clearButton)
            {
                clearButton.onClick.RemoveListener(HandleClearClicked);
            }

            if (usePoolToggle)
            {
                usePoolToggle.onValueChanged.RemoveListener(HandleUsePoolChanged);
            }

            if (collisionsToggle)
            {
                collisionsToggle.onValueChanged.RemoveListener(HandleCollisionsChanged);
            }

            if (showStatsToggle)
            {
                showStatsToggle.onValueChanged.RemoveListener(HandleShowStatsChanged);
            }

            if (spawnTypeDropdown)
            {
                spawnTypeDropdown.ValueChanged -= HandleSpawnTypeChanged;
            }

            if (despawnPolicyDropdown)
            {
                despawnPolicyDropdown.ValueChanged -= HandleDespawnPolicyChanged;
            }

            if (objectLifetimeSlider)
            {
                objectLifetimeSlider.ValueChanged -= HandleObjectLifetimeChanged;
            }

            if (spawnRateSlider)
            {
                spawnRateSlider.ValueChanged -= HandleSpawnRateChanged;
            }

            if (initialVelocitySlider)
            {
                initialVelocitySlider.ValueChanged -= HandleInitialVelocityChanged;
            }

            if (objectSizeSlider)
            {
                objectSizeSlider.ValueChanged -= HandleObjectSizeChanged;
            }
        }

        public void Initialize()
        {
            InitializeButtons();

            InitializeToggles();

            InitializeDropdowns();

            InitializeSliders();
        }

        private void HandleSwitchPanel()
        {
            if (_panelState == PanelState.Hidden)
            {
                animator.PlayShowAnimation();
            }
            else if (_panelState == PanelState.Visible)
            {
                animator.PlayHideAnimation();
            }
        }

        #region Initialize

        private void InitializeButtons()
        {
            Assert.IsNotNull(switchPanelButton, $"[{nameof(OptionsPanel)}] {nameof(switchPanelButton)} is null");
            Assert.IsNotNull(clearButton, $"[{nameof(OptionsPanel)}] {nameof(clearButton)} is null");

            switchPanelButton.ButtonClicked += HandleSwitchPanel;
            switchPanelButton.SwitchImage(_panelState);

            clearButton.onClick.AddListener(HandleClearClicked);
        }

        private void InitializeToggles()
        {
            Assert.IsNotNull(usePoolToggle, $"[{nameof(OptionsPanel)}] {nameof(usePoolToggle)} is null");
            Assert.IsNotNull(collisionsToggle, $"[{nameof(OptionsPanel)}] {nameof(collisionsToggle)} is null");
            Assert.IsNotNull(showStatsToggle, $"[{nameof(OptionsPanel)}] {nameof(showStatsToggle)} is null");

            usePoolToggle.isOn = initialSettings.UsePool;
            usePoolToggle.onValueChanged.AddListener(HandleUsePoolChanged);

            collisionsToggle.isOn = initialSettings.Collisions;
            collisionsToggle.onValueChanged.AddListener(HandleCollisionsChanged);

            showStatsToggle.isOn = initialSettings.ShowStats;
            showStatsToggle.onValueChanged.AddListener(HandleShowStatsChanged);
        }

        private void InitializeDropdowns()
        {
            Assert.IsNotNull(spawnTypeDropdown, $"[{nameof(OptionsPanel)}] {nameof(spawnTypeDropdown)} is null");
            Assert.IsNotNull(despawnPolicyDropdown,
                $"[{nameof(OptionsPanel)}] {nameof(despawnPolicyDropdown)} is null");

            var spawnTypeNames = Enum.GetNames(typeof(SpawnType)).ToList();

            for (int i = 0; i < spawnTypeNames.Count; i++)
            {
                spawnTypeNames[i] = spawnTypeNames[i].ToUpper(CultureInfo.InvariantCulture);
            }

            spawnTypeDropdown.SetupOptions(spawnTypeNames);
            spawnTypeDropdown.SetValue((int)initialSettings.SpawnType);

            spawnTypeDropdown.ValueChanged += HandleSpawnTypeChanged;


            var despawnPolicyNames = Enum.GetNames(typeof(DespawnPolicy)).ToList();

            for (int i = 0; i < despawnPolicyNames.Count; i++)
            {
                despawnPolicyNames[i] = despawnPolicyNames[i].ToUpper(CultureInfo.InvariantCulture);
            }

            despawnPolicyDropdown.SetupOptions(despawnPolicyNames);
            despawnPolicyDropdown.SetValue((int)initialSettings.DespawnPolicy);

            despawnPolicyDropdown.ValueChanged += HandleDespawnPolicyChanged;
        }

        private void InitializeSliders()
        {
            Assert.IsNotNull(objectLifetimeSlider, $"[{nameof(OptionsPanel)}] {nameof(objectLifetimeSlider)} is null");
            Assert.IsNotNull(spawnRateSlider, $"[{nameof(OptionsPanel)}] {nameof(spawnRateSlider)} is null");
            Assert.IsNotNull(initialVelocitySlider,
                $"[{nameof(OptionsPanel)}] {nameof(initialVelocitySlider)} is null");
            Assert.IsNotNull(objectSizeSlider, $"[{nameof(OptionsPanel)}] {nameof(objectSizeSlider)} is null");

            objectLifetimeSlider.SetValue(initialSettings.ObjectLifetime);
            objectLifetimeSlider.ValueChanged += HandleObjectLifetimeChanged;
            objectLifetimeSlider.gameObject.SetActive(initialSettings.DespawnPolicy == DespawnPolicy.Timer);

            spawnRateSlider.SetValue(initialSettings.SpawnRate);
            spawnRateSlider.ValueChanged += HandleSpawnRateChanged;

            initialVelocitySlider.SetValue(initialSettings.InitialVelocity);
            initialVelocitySlider.ValueChanged += HandleInitialVelocityChanged;

            objectSizeSlider.SetValue(initialSettings.ObjectSize);
            objectSizeSlider.ValueChanged += HandleObjectSizeChanged;
        }

        #endregion

        #region Handlers

        private void HandleUsePoolChanged(bool isOn)
        {
            UsePoolChanged?.Invoke(isOn);
        }

        private void HandleCollisionsChanged(bool isOn)
        {
            CollisionsChanged?.Invoke(isOn);
        }

        private void HandleShowStatsChanged(bool isOn)
        {
            ShowStatsChanged?.Invoke(isOn);
        }

        private void HandleClearClicked()
        {
            ClearClicked?.Invoke();
        }

        private void HandleSpawnTypeChanged(int value)
        {
            SpawnTypeChanged?.Invoke(value);
        }

        private void HandleDespawnPolicyChanged(int value)
        {
            objectLifetimeSlider.gameObject.SetActive(value == (int)DespawnPolicy.Timer);

            DespawnPolicyChanged?.Invoke(value);
        }

        private void HandleObjectLifetimeChanged(float value)
        {
            ObjectLifetimeChanged?.Invoke(value);
        }

        private void HandleSpawnRateChanged(float value)
        {
            SpawnRateChanged?.Invoke(value);
        }

        private void HandleInitialVelocityChanged(float value)
        {
            InitialVelocityChanged?.Invoke(value);
        }

        private void HandleObjectSizeChanged(float value)
        {
            ObjectSizeChanged?.Invoke(value);
        }

        #endregion

        #region Animator

        private void HandleShowStarted()
        {
            switchPanelButton.SetInteractableState(false);
        }

        private void HandleShowEnded()
        {
            _panelState = PanelState.Visible;

            switchPanelButton.SetInteractableState(true);
            switchPanelButton.SwitchImage(_panelState);
        }

        private void HandleHideStarted()
        {
            switchPanelButton.SetInteractableState(false);
        }

        private void HandleHideEnded()
        {
            _panelState = PanelState.Hidden;

            switchPanelButton.SetInteractableState(true);
            switchPanelButton.SwitchImage(_panelState);
        }

        #endregion
    }
}