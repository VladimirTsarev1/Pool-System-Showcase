#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Showcase.Scripts.Bootstrap
{
    /// <summary>
    /// Forces the game to always start from Bootstrap scene,
    /// even if we hit Play on a different scene in the editor
    /// </summary>
    [InitializeOnLoad]
    public static class BootstrapLoader
    {
        private const string BootstrapPath = "Assets/Showcase/Scenes/0_Bootstrap.unity";

        static BootstrapLoader()
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapPath);
            if (scene == null)
            {
                Debug.LogWarning($"[{nameof(BootstrapLoader)}] Scene not found: {BootstrapPath}");
                return;
            }

            EditorSceneManager.playModeStartScene = scene;
        }
    }
}
#endif