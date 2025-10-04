using Showcase.Scripts.Audio;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.MainMenu.Background;
using Showcase.Scripts.Pooling;
using Showcase.Scripts.SceneFlow;
using Showcase.Scripts.UI.Panels;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.MainMenu
{
    [DisallowMultipleComponent]
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private BackgroundController backgroundController;
        [SerializeField] private PanelsManager panelManager;

        private ISceneService _sceneService;
        private IAudioService _audioService;
        private IPoolService _poolService;

        private void Awake()
        {
            _audioService = ServiceLocator.Resolve<IAudioService>();
            _sceneService = ServiceLocator.Resolve<ISceneService>();
            _poolService = ServiceLocator.Resolve<IPoolService>();

            Assert.IsNotNull(_audioService, $"[{nameof(MainMenuController)}] {nameof(_audioService)} is null]");
            Assert.IsNotNull(_sceneService, $"[{nameof(MainMenuController)}] {nameof(_sceneService)} is null]");
            Assert.IsNotNull(_poolService, $"[{nameof(MainMenuController)}] {nameof(_poolService)} is null]");

            Assert.IsNotNull(backgroundController,
                $"[{nameof(MainMenuController)}] {nameof(backgroundController)} is null]");
            Assert.IsNotNull(panelManager, $"[{nameof(MainMenuController)}] {nameof(panelManager)} is null]");

            _sceneService.SceneLoadCompleted += HandleSceneLoadCompleted;
            _sceneService.SceneLoadStarted += HandleSceneLoadStarted;
        }

        private void InitializeMainMenu()
        {
            _audioService.PlayMusic(AudioClipTag.MainMenuBackgroundMusic);

            backgroundController.Initialize();

            panelManager.Initialize();
        }

        private void HandleSceneLoadCompleted()
        {
            _sceneService.SceneLoadCompleted -= HandleSceneLoadCompleted;
            InitializeMainMenu();
        }

        private void HandleSceneLoadStarted()
        {
            _sceneService.SceneLoadStarted -= HandleSceneLoadStarted;

            backgroundController.DeactivateBackground();

            _poolService.ReleaseAllPools();
        }
    }
}