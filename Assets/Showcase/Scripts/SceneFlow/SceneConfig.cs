using Showcase.Scripts.Core.Constants;
using UnityEngine;

namespace Showcase.Scripts.SceneFlow
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = PathConstants.ScriptableObjects + "/Scene Config")]
    public sealed class SceneConfig : ScriptableObject
    {
        [SerializeField] private SceneType sceneType;
        [SerializeField] private string sceneName;

        public SceneType SceneType => sceneType;
        public string SceneName => sceneName;
    }
}