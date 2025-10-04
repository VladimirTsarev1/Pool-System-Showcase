using Showcase.Scripts.Audio;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Pooling;
using Showcase.Scripts.SceneFlow;
using UnityEngine;

namespace Showcase.Scripts.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class GameCompositionRoot : MonoBehaviour
    {
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private PoolManager poolManager;
        [SerializeField] private SceneManager sceneManager;

        public void RegisterServices()
        {
            ServiceLocator.Clear();

            if (audioManager != null)
            {
                ServiceLocator.Register<IAudioService>(audioManager);
            }

            if (poolManager != null)
            {
                ServiceLocator.Register<IPoolService>(poolManager);
            }

            if (sceneManager != null)
            {
                ServiceLocator.Register<ISceneService>(sceneManager);
            }
        }
    }
}