using UnityEngine;

namespace Showcase.Scripts.Demo1.UI.Stats
{
    public sealed class FpsCounter
    {
        public int Fps => _fps;
        public int AverageFps => _averageFps;

        private int _fps;
        private int _averageFps;

        private int _frames;
        private float _time;

        public void Update()
        {
            _fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            _frames++;
            _time += Time.unscaledDeltaTime;

            if (_time >= 1f)
            {
                _averageFps = Mathf.RoundToInt(_frames / _time);
                _frames = 0;
                _time = 0f;
            }
        }
    }
}