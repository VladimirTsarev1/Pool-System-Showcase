using UnityEngine;

namespace Showcase.Scripts.Pooling
{
    public interface IPoolService
    {
        public T GetPoolObject<T>(PoolType type, Vector3 position, Quaternion rotation = default,
            float returnTime = float.NaN)
            where T : Component;
        
        public Pool GetPool(PoolType type);
        public GameObject GetPrefab(PoolType type);
        public void ReleaseAllPools();
    }
}