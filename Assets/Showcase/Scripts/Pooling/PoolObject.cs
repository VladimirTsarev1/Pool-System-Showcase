using System;
using Showcase.Scripts.Boundary;
using UnityEngine;

namespace Showcase.Scripts.Pooling
{
    /// <summary>
    /// Base class for pooled objects with auto-return logic
    /// </summary>
    public class PoolObject : MonoBehaviour, IOutOfBounds
    {
        public event Action<PoolObject> ReturnedToPool;

        private PoolConfig _config;
        private bool _isActiveInPool;
        
        protected virtual void OnDisable()
        {
            if (_isActiveInPool)
            {
                ReturnToPool();
            }
        }

        protected virtual void OnDestroy()
        {
            ReturnedToPool = null;
        }

        public void Initialize(PoolConfig config)
        {
            _config = config;
        }

        public virtual void OnGet(float returnTime = float.NaN)
        {
            _isActiveInPool = true;

            SetupAutoReturn(returnTime);
        }

        public virtual void ForceReturnToPool()
        {
            ReturnToPool();
        }

        protected virtual void ReturnToPool()
        {
            if (!_isActiveInPool)
            {
                return;
            }

            _isActiveInPool = false;
            CancelInvoke(nameof(ReturnToPool));
            StopAllCoroutines();

            ReturnedToPool?.Invoke(this);
        }

        private void SetupAutoReturn(float returnTime = float.NaN)
        {
            if (float.IsNaN(returnTime))
            {
                if (_config?.DespawnPolicy == DespawnPolicy.Timer)
                {
                    Invoke(nameof(ReturnToPool), _config.ReturnTime);
                }
            }
            else
            {
                Invoke(nameof(ReturnToPool), returnTime);
            }
        }

        public virtual void OnOutOfBounds()
        {
            if (_config?.DespawnPolicy == DespawnPolicy.Timer)
            {
                CancelInvoke(nameof(ReturnToPool));
            }

            ReturnToPool();
        }
    }
}