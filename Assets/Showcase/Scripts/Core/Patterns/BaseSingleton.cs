using UnityEngine;

namespace Showcase.Scripts.Core.Patterns
{
    /// <summary>
    /// Simple singleton base class for MonoBehaviours.
    /// Automatically handles duplicate instances and provides safe access.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class</typeparam>
    public abstract class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static bool IsInitialized => _instance != null;

        public static T Instance
        {
            get
            {
                if (_instance)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<T>();

                if (!_instance)
                {
                    Debug.LogError($"[{nameof(BaseSingleton<T>)}] No instance of {typeof(T)} found in scene!");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            InitializeSingleton();

            if (_instance != this)
            {
                return;
            }

            DontDestroyOnLoad(gameObject);

            OnSingletonAwake();
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        protected virtual void OnSingletonAwake()
        {
        }

        private void InitializeSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"Duplicate {typeof(T)} found. Destroying this instance.");
                Destroy(gameObject);
            }

            if (_instance == this)
            {
                return;
            }

            _instance = this as T;
        }
    }
}