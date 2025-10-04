using System;
using System.Threading;
using System.Threading.Tasks;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Pooling;
using Showcase.Scripts.Providers;
using Showcase.Scripts.SceneFlow;
using UnityEngine;

namespace Showcase.Scripts.Bootstrap
{
    /// <summary>
    /// Entry point for the bootstrap scene. Registers services, initializes
    /// global systems, and then loads the main menu scene
    /// </summary>
    
    [DisallowMultipleComponent]
    public sealed class Bootstrap : MonoBehaviour
    {
        [SerializeField] private GameCompositionRoot gameCompositionRoot;
        [SerializeField] private PoolManager poolManager;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async void Awake()
        {
            try
            {
                Debug.Log("Bootstrap scene started");

                gameCompositionRoot.RegisterServices();

                _cts = new CancellationTokenSource();
                await InitAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async Task InitAsync(CancellationToken cancellationToken)
        {
            await ConfigProvider.InitAsync(cancellationToken);

            var tasks = new Task[]
            {
                poolManager.InitAsync(cancellationToken),
            };

            await Task.WhenAll(tasks);

            await Task.Yield();

            LoadNextScene();
        }

        private void LoadNextScene()
        {
            var sceneLoader = ServiceLocator.Resolve<ISceneService>();

            sceneLoader.LoadScene(SceneType.MainMenu);
        }
    }
}