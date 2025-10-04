using System;
using System.Collections;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Providers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Showcase.Scripts.SceneFlow
{
    [DisallowMultipleComponent]
    public sealed class SceneManager : BaseSingleton<SceneManager>, ISceneService
    {
        public event Action SceneLoadStarted;
        public event Action SceneLoadCompleted;

        [SerializeField] private SceneOverlay sceneOverlay;

        private AsyncOperation _loadingOperation;

        protected override void OnSingletonAwake()
        {
            base.OnSingletonAwake();

            Assert.IsNotNull(sceneOverlay, $"{nameof(SceneManager)} {nameof(sceneOverlay)} is null");
        }

        public void LoadScene(SceneType sceneType, bool useOverlay = true)
        {
            StartCoroutine(LoadSceneAsync(sceneType, useOverlay));
        }

        private IEnumerator LoadSceneAsync(SceneType sceneType, bool useOverlay)
        {
            var sceneName = ConfigProvider.Scene.GetSceneName(sceneType);

            SceneLoadStarted?.Invoke();

            if (useOverlay)
            {
                sceneOverlay.Show();

                sceneOverlay.HideStarted += HandleOverlayHideStarted;
            }

            _loadingOperation =
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            _loadingOperation.allowSceneActivation = false;

            while (!_loadingOperation.isDone)
            {
                float progress = Mathf.Clamp01(_loadingOperation.progress / 0.9f);

                if (useOverlay)
                {
                    sceneOverlay.UpdateProgress(progress);
                }

                if (_loadingOperation.progress >= 0.9f)
                {
                    if (useOverlay)
                    {
                        sceneOverlay.Hide();
                    }
                    else
                    {
                        _loadingOperation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            SceneLoadCompleted?.Invoke();
        }

        private void HandleOverlayHideStarted()
        {
            sceneOverlay.HideStarted -= HandleOverlayHideStarted;

            _loadingOperation.allowSceneActivation = true;
        }
    }
}