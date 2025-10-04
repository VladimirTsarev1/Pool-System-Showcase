using Showcase.Scripts.Core.Constants;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Showcase.Scripts.Pooling
{
    [CreateAssetMenu(fileName = "New Pools Config", menuName = PathConstants.ScriptableObjects + "/Pools Config")]
    public sealed class PoolConfig : ScriptableObject
    {
        [SerializeField] private PoolType poolType;
        [SerializeField] private AssetReferenceGameObject prefabReference;
        [SerializeField] private int initialSize;

        [SerializeField] private DespawnPolicy despawnPolicy = DespawnPolicy.Timer;
        [SerializeField] private float returnTime;

        public PoolType PoolType => poolType;
        public AssetReferenceGameObject PrefabReference => prefabReference;
        public int InitialSize => initialSize;
        public DespawnPolicy DespawnPolicy => despawnPolicy;
        public float ReturnTime => returnTime;
    }
}