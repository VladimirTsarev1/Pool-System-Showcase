using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.Demo1.UI.Stats
{
    [DisallowMultipleComponent]
    public sealed class StatsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text activePooledText;
        [SerializeField] private TMP_Text inactivePooledText;
        [SerializeField] private TMP_Text instantiatedText;

        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private TMP_Text averageFpsText;

        [SerializeField] private TMP_Text gcAllocatedBytesText;

        private FpsCounter _fpsCounter;
        // private GraphicStats _graphicStats;

        private void Awake()
        {
            Assert.IsNotNull(activePooledText, $"[{nameof(StatsPanel)}] {nameof(activePooledText)} is null");
            Assert.IsNotNull(inactivePooledText, $"[{nameof(StatsPanel)}] {nameof(inactivePooledText)} is null");
            Assert.IsNotNull(instantiatedText, $"[{nameof(StatsPanel)}] {nameof(instantiatedText)} is null");
            Assert.IsNotNull(fpsText, $"[{nameof(StatsPanel)}] {nameof(fpsText)} is null");
            Assert.IsNotNull(averageFpsText, $"[{nameof(StatsPanel)}] {nameof(averageFpsText)} is null");
            Assert.IsNotNull(gcAllocatedBytesText, $"[{nameof(StatsPanel)}] {nameof(gcAllocatedBytesText)} is null");

            _fpsCounter = new FpsCounter();
            // _graphicStats = new GraphicStats();
        }

        private void OnDestroy()
        {
            // _graphicStats?.Dispose();
        }

        private void Update()
        {
            _fpsCounter.Update();
            // _graphicStats.Update();

            UpdateFps();
            // UpdateGraphicStats();
        }

        public void UpdateActivePooledObjectsNumber(int objectsNumber)
        {
            activePooledText.text = objectsNumber.ToString();
        }

        public void UpdateInactivePooledObjectsNumber(int objectsNumber)
        {
            inactivePooledText.text = objectsNumber.ToString();
        }

        public void UpdateInstantiatedObjectsNumber(int objectsNumber)
        {
            instantiatedText.text = objectsNumber.ToString();
        }

        private void UpdateFps()
        {
            fpsText.text = $"{_fpsCounter.Fps}";
            averageFpsText.text = $"{_fpsCounter.AverageFps}";
        }

        private void UpdateGraphicStats()
        {
            // gcAllocatedBytesText.text = $"{_graphicStats.GcAllocatedBytesLastFrame}";
        }
    }
}