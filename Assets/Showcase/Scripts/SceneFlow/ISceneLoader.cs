using System;

namespace Showcase.Scripts.SceneFlow
{
    public interface ISceneService
    {
        public event Action SceneLoadStarted;
        public event Action SceneLoadCompleted;
        public void LoadScene(SceneType sceneType, bool useOverlay = true);
    }
}