using System.Collections;
using Showcase.Scripts.Audio;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Pooling;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Showcase.Scripts.MainMenu.Background
{
    [DisallowMultipleComponent]
    public sealed class BackgroundObjectSpawner : MonoBehaviour
    {
        [SerializeField] private Vector2 startViewportPoint = Vector2.zero;
        [SerializeField] private Vector2 endViewportPoint = Vector2.right;

        [SerializeField] private Vector2 spawnIntervalRange = new(0.5f, 2f);

        [SerializeField] private PoolType[] projectileTypes;
        [SerializeField] private Vector2 forceRange = new(5f, 15f);
        [SerializeField] private Vector3 shootDirection = Vector3.down;

        private IPoolService _poolService;
        private IAudioService _audioService;

        private Vector3 _worldStartPoint;
        private Vector3 _worldEndPoint;
        private float _zOffset;

        private bool _isSpawning;

        private void Awake()
        {
            _poolService = ServiceLocator.Resolve<IPoolService>();
            _audioService = ServiceLocator.Resolve<IAudioService>();

            Assert.IsNotNull(_poolService, $"[{nameof(BackgroundObjectSpawner)}] {nameof(_poolService)} is null");
            Assert.IsNotNull(_audioService, $"[{nameof(BackgroundObjectSpawner)}] {nameof(_audioService)} is null");

            Assert.IsTrue(projectileTypes.Length > 0, $"[{nameof(BackgroundObjectSpawner)}] ProjectileTypes length is 0");
        }

        private void OnDisable()
        {
            StopSpawn();
        }

        public void StartSpawn(Camera cameraComponent, float distanceFromCamera, float zOffset)
        {
            if (_isSpawning)
            {
                Debug.LogWarning($"[{nameof(BackgroundObjectSpawner)}] Already spawning!", this);
                return;
            }

            CalculateWorldPoints(cameraComponent, distanceFromCamera, zOffset);

            _isSpawning = true;
            StartCoroutine(SpawnLoop());
        }

        public void StopSpawn()
        {
            _isSpawning = false;
            StopAllCoroutines();
        }

        private void CalculateWorldPoints(Camera cameraComponent, float distanceFromCamera, float zOffset)
        {
            _zOffset = zOffset / 2;

            _worldStartPoint = cameraComponent.ViewportToWorldPoint(
                new Vector3(startViewportPoint.x, startViewportPoint.y, distanceFromCamera));
            _worldEndPoint = cameraComponent.ViewportToWorldPoint(
                new Vector3(endViewportPoint.x, endViewportPoint.y, distanceFromCamera));
        }

        private IEnumerator SpawnLoop()
        {
            while (_isSpawning)
            {
                SpawnObject();
                yield return new WaitForSeconds(GetRandomInterval());
            }
        }

        private void SpawnObject()
        {
            var spawnPosition = GetRandomSpawnPosition();
            var poolType = GetRandomPoolType();
            var velocity = CalculateVelocity();

            var backgroundObject = _poolService.GetPoolObject<BackgroundObject>(poolType, spawnPosition);

            backgroundObject.Collided += HandleCollided;
            backgroundObject.ReturnedToPool += HandleReturnedToPool;

            backgroundObject.Setup(velocity);
        }

        private void HandleCollided()
        {
            _audioService.PlaySfx(AudioClipTag.BackgroundObjectCollision);
        }

        private void HandleReturnedToPool(PoolObject poolObject)
        {
            poolObject.ReturnedToPool -= HandleReturnedToPool;

            if (poolObject is BackgroundObject backgroundPoolObject)
            {
                backgroundPoolObject.Collided -= HandleCollided;
            }
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var randomZOffset = Random.Range(-_zOffset, _zOffset);

            var startPoint = _worldStartPoint;
            var endPoint = _worldEndPoint;

            startPoint.z += randomZOffset;
            endPoint.z += randomZOffset;

            return Vector3.Lerp(startPoint, endPoint, Random.value);
        }

        private PoolType GetRandomPoolType()
        {
            return projectileTypes[Random.Range(0, projectileTypes.Length)];
        }

        private Vector3 CalculateVelocity()
        {
            var force = Random.Range(forceRange.x, forceRange.y);
            return shootDirection.normalized * force;
        }

        private float GetRandomInterval()
        {
            return Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
        }

        private void HandleSceneLoadStarted()
        {
            StopAllCoroutines();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_worldStartPoint, _worldEndPoint);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_worldStartPoint, 0.5f);
                Gizmos.DrawWireSphere(_worldEndPoint, 0.5f);
            }
        }
#endif
    }
}