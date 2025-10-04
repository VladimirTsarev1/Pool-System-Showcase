using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.UI.Controls
{
    [DisallowMultipleComponent]
    public sealed class DropdownView : MonoBehaviour
    {
        public event Action<int> ValueChanged;

        [SerializeField] private TMP_Dropdown dropdown;

        public int Value => dropdown.value;

        private void Awake()
        {
            Assert.IsNotNull(dropdown, $"[{nameof(DropdownView)}] {nameof(dropdown)} is null");
        }

        private void OnEnable()
        {
            dropdown.onValueChanged.AddListener(HandleValueChanged);
        }

        private void OnDisable()
        {
            if (!dropdown)
            {
                return;
            }

            dropdown.onValueChanged.RemoveListener(HandleValueChanged);
        }

        public void SetupOptions(List<string> options)
        {
            if (options == null || options.Count <= 0)
            {
                Debug.LogWarning($"[{nameof(DropdownView)}] Trying to add null or empty options list", this);
                return;
            }

            dropdown.ClearOptions();
            dropdown.AddOptions(options);
        }

        public void SetValue(int value, bool sendEvent = false)
        {
            if (value < 0 || value >= dropdown.options.Count)
            {
                Debug.LogError($"[{nameof(DropdownView)}] Value {value} is out of range", this);
                return;
            }

            if (sendEvent)
            {
                dropdown.value = value;
            }
            else
            {
                dropdown.SetValueWithoutNotify(value);
            }
        }

        public void SetInteractable(bool interactable)
        {
            dropdown.interactable = interactable;
        }

        private void HandleValueChanged(int value)
        {
            ValueChanged?.Invoke(value);
        }
    }
}