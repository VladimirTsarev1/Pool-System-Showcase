using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.SceneFlow;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Showcase.Scripts.UI.Controls
{
    [DisallowMultipleComponent]
    public sealed class LoadSceneButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SceneType sceneType;

        private ISceneService _sceneService;
        
        private void Awake()
        {
            _sceneService = ServiceLocator.Resolve<ISceneService>();

            Assert.IsNotNull(_sceneService, $"[{nameof(LoadSceneButton)}] {nameof(_sceneService)} is null");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _sceneService.LoadScene(sceneType);
        }
    }
}