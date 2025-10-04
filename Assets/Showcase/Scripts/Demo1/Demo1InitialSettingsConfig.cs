using Showcase.Scripts.Core.Constants;
using Showcase.Scripts.Demo1.SpawnObjects;
using Showcase.Scripts.Pooling;
using UnityEngine;

namespace Showcase.Scripts.Demo1
{
    [CreateAssetMenu(fileName = "Demo1 Initial Settings Config",
        menuName = PathConstants.ScriptableObjects + "/Demo1 Initial Settings")]
    public sealed class Demo1InitialSettingsConfig : ScriptableObject
    {
        [SerializeField] private bool usePool = true;
        [SerializeField] private bool collisions = true;
        [SerializeField] private bool showStats = true;

        [SerializeField] private PoolType poolType = PoolType.Demo1Sphere;
        [SerializeField] private SpawnType spawnType = SpawnType.Spray;
        [SerializeField] private DespawnPolicy despawnPolicy = DespawnPolicy.Disable;

        [SerializeField] private float spawnRate = 10f;
        [SerializeField] private float objectLifetime = 5f;
        [SerializeField] private float initialVelocity = 5f;
        [SerializeField] private float objectSize = 1f;

        public bool UsePool => usePool;
        public bool Collisions => collisions;
        public bool ShowStats => showStats;
        public PoolType PoolType => poolType;
        public SpawnType SpawnType => spawnType;
        public DespawnPolicy DespawnPolicy => despawnPolicy;
        public float SpawnRate => spawnRate;
        public float ObjectLifetime => objectLifetime;
        public float InitialVelocity => initialVelocity;
        public float ObjectSize => objectSize;
    }
}