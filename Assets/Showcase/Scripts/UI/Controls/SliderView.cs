using System;
using Showcase.Scripts.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Showcase.Scripts.UI.Controls
{
    [DisallowMultipleComponent]
    public class SliderView : MonoBehaviour
    {
        public event Action<float> ValueChanged;

        [SerializeField] protected Slider slider;
        [SerializeField] protected TMP_Text valueText;

        public float Value => slider.value;
        
        protected virtual void Awake()
        {
            Assert.IsNotNull(slider,  $"[{nameof(SliderView)}] {nameof(slider)} is null");
            Assert.IsNotNull(valueText,  $"[{nameof(SliderView)}] {nameof(valueText)} is null");
        }

        protected virtual void OnEnable()
        {
            slider.onValueChanged.AddListener(HandleValueChanged);
        }

        protected virtual void OnDisable()
        {
            if (!slider)
            {
                return;
            }

            slider.onValueChanged.RemoveListener(HandleValueChanged);
        }

        public void SetupRange(float min, float max)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }

        public void SetValue(float value, bool sendEvent = false)
        {
            if (value < slider.minValue || value > slider.maxValue)
            {
                Debug.LogError($"[{nameof(SliderView)}] Value {value} is out of range", this);
                return;
            }

            if (sendEvent)
            {
                slider.value = value;
            }
            else
            {
                slider.SetValueWithoutNotify(value);
                UpdateText(value);
            }
        }

        protected virtual void HandleValueChanged(float value)
        {
            UpdateValue(value);
        }

        private void UpdateValue(float value)
        {
            UpdateText(value);

            ValueChanged?.Invoke(value);
        }

        protected virtual void UpdateText(float value)
        {
            if (slider.wholeNumbers)
            {
                valueText.text = $"{value}";
            }
            else
            {
                valueText.text = $"{value.OneDecimalToString()}";
            }
        }
    }
}