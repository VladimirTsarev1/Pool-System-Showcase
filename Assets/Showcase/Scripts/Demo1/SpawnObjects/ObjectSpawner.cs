using System;
using System.Collections.Generic;
using System.Linq;
using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Demo1.Boundaries;
using Showcase.Scripts.Demo1.SpawnObjects.Projectile;
using Showcase.Scripts.Pooling;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Showcase.Scripts.Demo1.SpawnObjects
{
    [DisallowMultipleComponent]
    public sealed class ObjectSpawner : MonoBehaviour
    {
        public event Action<int, int> OnPooledObjectNumberUpdated;
        public event Action<int> OnInstantiatedObjectNumberUpdated;

        [SerializeField] private Demo1InitialSettingsConfig initialSettings;
        [SerializeField] private Demo1BoundariesSpawner boundariesSpawner;

        [SerializeField] private float distanceFromCamera = 30f;
        [SerializeField] private float boundaryOffset = 2f;

        [Header("Steady Spawn")]
        [SerializeField] private Vector2 startViewportPoint = Vector2.zero;

        [SerializeField] private Vector2 endViewportPoint = Vector2.right;

        public int PooledActiveObjectsNumber => _currentPool.ActiveObjectsNumber;
        public int PooledInactiveObjectsNumber => _currentPool.InactiveObjectsNumber;
        public int InstantiatedObjectsNumber => _instantiatedObjects.Count;

        private Camera _camera;
        private IPoolService _poolService;
        private EventSystem _eventSystem;
        private Pool _currentPool;

        private readonly HashSet<Demo1Projectile> _allSpawnedObjects = new HashSet<Demo1Projectile>();
        private readonly HashSet<Demo1Projectile> _instantiatedObjects = new HashSet<Demo1Projectile>();

        private SpawnType _spawnType;
        private DespawnPolicy _despawnPolicy;

        private bool _usePool;
        private bool _collisions;

        private float _spawnRate;
        private float _objectLifetime;
        private float _initialVelocity;

        private Vector3 _worldStartPoint;
        private Vector3 _worldEndPoint;
        private Vector3 _objectSize;

        private float _spawnInterval = 1f;
        private float _accumulator;

        private bool _spawnIsAllowed = true;

        private void Awake()
        {
            _poolService = ServiceLocator.Resolve<IPoolService>();

            Assert.IsNotNull(_poolService, $"[{nameof(ObjectSpawner)}] {nameof(_poolService)} is null");
            Assert.IsNotNull(initialSettings, $"[{nameof(ObjectSpawner)}] {nameof(initialSettings)} is null");
            Assert.IsNotNull(boundariesSpawner, $"[{nameof(ObjectSpawner)}] {nameof(boundariesSpawner)} is null");
        }

        private void Update()
        {
            if (!_spawnIsAllowed)
            {
                return;
            }

            HandleSpawning();
        }

        public void Initialize()
        {
            _eventSystem = EventSystem.current;
            Assert.IsNotNull(_eventSystem, $"[{nameof(ObjectSpawner)}] {nameof(_eventSystem)} is null");

            _camera = Camera.main;
            Assert.IsNotNull(_camera, $"[{nameof(ObjectSpawner)}] {nameof(_camera)} is null");

            InitializeValues();

            boundariesSpawner.SetupBoundary(_camera, distanceFromCamera, boundaryOffset);

            _worldStartPoint = _camera.ViewportToWorldPoint(
                new Vector3(startViewportPoint.x, startViewportPoint.y, distanceFromCamera));
            _worldEndPoint = _camera.ViewportToWorldPoint(
                new Vector3(endViewportPoint.x, endViewportPoint.y, distanceFromCamera));
        }

        private void InitializeValues()
        {
            _usePool = initialSettings.UsePool;
            _collisions = initialSettings.Collisions;

            _spawnType = initialSettings.SpawnType;
            _despawnPolicy = initialSettings.DespawnPolicy;

            _objectLifetime = initialSettings.ObjectLifetime;
            _spawnRate = initialSettings.SpawnRate;
            _initialVelocity = initialSettings.InitialVelocity;
            _objectSize = Vector3.one * initialSettings.ObjectSize;

            _currentPool = _poolService.GetPool(initialSettings.PoolType);

            _spawnInterval = 1f / _spawnRate;

            OnPooledObjectNumberUpdated?.Invoke(PooledActiveObjectsNumber, PooledInactiveObjectsNumber);
            OnInstantiatedObjectNumberUpdated?.Invoke(InstantiatedObjectsNumber);
        }

        public void BreakAndClear()
        {
            _spawnIsAllowed = false;

            _poolService.ReleaseAllPools();
        }

        public void SetUsePoolState(bool usePool)
        {
            _usePool = usePool;
        }

        public void SetCollisionsState(bool collisions)
        {
            _collisions = collisions;

            foreach (var spawnedObject in _allSpawnedObjects)
            {
                spawnedObject.SetCollisionsState(_collisions);
            }
        }

        public void SetSpawnType(int spawnType)
        {
            _accumulator = 0;
            _spawnType = (SpawnType)spawnType;
        }

        public void SetDespawnPolicy(int despawnPolicyValue)
        {
            _despawnPolicy = (DespawnPolicy)despawnPolicyValue;
        }

        public void SetObjectLifetime(float lifetime)
        {
            _objectLifetime = lifetime;
        }

        public void SetSpawnRate(float spawnRate)
        {
            _spawnRate = (int)spawnRate;

            _spawnInterval = 1f / spawnRate;
        }

        public void SetInitialVelocity(float velocity)
        {
            _initialVelocity = velocity;
        }

        public void SetObjectsSize(float size)
        {
            _objectSize = Vector3.one * size;

            foreach (var spawnedObject in _allSpawnedObjects)
            {
                spawnedObject.SetSize(_objectSize);
            }
        }

        public void ClearPool()
        {
            var allSpawnedObjectsArray = _allSpawnedObjects.ToArray();

            for (int i = 0; i < allSpawnedObjectsArray.Length; i++)
            {
                allSpawnedObjectsArray[i].Release();
            }

            _allSpawnedObjects.Clear();
        }

        private void HandleSpawning()
        {
            switch (_spawnType)
            {
                case SpawnType.Steady:
                    HandleSteady();
                    break;
                case SpawnType.Spray:
                    HandleSpray();
                    break;
                case SpawnType.Burst:
                    HandleBurst();
                    break;
            }
        }

        private void HandleSteady()
        {
            _accumulator += Time.deltaTime;

            while (_accumulator >= _spawnInterval)
            {
                _accumulator -= _spawnInterval;
                SpawnProjectileSteady();
            }
        }

        private void HandleSpray()
        {
            if (_eventSystem.IsPointerOverGameObject() || !Input.GetMouseButton(0))
            {
                return;
            }

            _accumulator += Time.deltaTime;

            while (_accumulator >= _spawnInterval)
            {
                _accumulator -= _spawnInterval;
                SpawnProjectileAtMousePosition();
            }
        }

        private void HandleBurst()
        {
            if (_eventSystem.IsPointerOverGameObject() || !Input.GetMouseButtonDown(0))
            {
                return;
            }

            for (int i = 0; i < _spawnRate; i++)
            {
                SpawnProjectileAtMousePosition();
            }
        }

        private void SpawnProjectileAtMousePosition()
        {
            var velocity = (Vector3)Random.insideUnitCircle;
            velocity = velocity.normalized * _initialVelocity;

            Demo1Projectile projectile = CreateProjectile(GetMousePosition());

            SetupProjectile(projectile, velocity);
        }

        private void SpawnProjectileSteady()
        {
            var spawnPoint = Vector3.Lerp(_worldStartPoint, _worldEndPoint, Random.value);

            var velocity = Vector3.down * (Random.Range(Mathf.Epsilon, 1f));
            velocity = velocity.normalized * _initialVelocity;

            Demo1Projectile projectile = CreateProjectile(spawnPoint);

            SetupProjectile(projectile, velocity);
        }

        private Demo1Projectile CreateProjectile(Vector3 position)
        {
            var lifetime = _despawnPolicy == DespawnPolicy.Timer ? _objectLifetime : float.NaN;

            if (_usePool)
            {
                var projectile = _poolService.GetPoolObject<Demo1Projectile>(PoolType.Demo1Sphere, position,
                    Quaternion.identity,
                    lifetime);

                projectile.ReturnedToPool += HandleObjectReturnedToPool;

                return projectile;
            }
            else
            {
                var projectileAsset = _poolService.GetPrefab(PoolType.Demo1Sphere);
                var instance = Instantiate(projectileAsset);

                if (instance.TryGetComponent(out Demo1Projectile projectile))
                {
                    projectile.transform.position = position;
                    projectile.Destroyed += HandleNonPoolObjectDestroyed;

                    projectile.OnGet(lifetime);

                    return projectile;
                }

                return null;
            }
        }

        private void SetupProjectile(Demo1Projectile projectile, Vector3 velocity)
        {
            if (projectile != null)
            {
                _allSpawnedObjects.Add(projectile);

                if (_usePool)
                {
                    OnPooledObjectNumberUpdated?.Invoke(PooledActiveObjectsNumber, PooledInactiveObjectsNumber);
                }
                else
                {
                    _instantiatedObjects.Add(projectile);
                    OnInstantiatedObjectNumberUpdated?.Invoke(InstantiatedObjectsNumber);
                }

                projectile.Setup(_objectSize, velocity, _usePool, _collisions);
            }
        }

        private void HandleObjectReturnedToPool(PoolObject poolObject)
        {
            poolObject.ReturnedToPool -= HandleObjectReturnedToPool;

            if (poolObject is Demo1Projectile projectile)
            {
                _allSpawnedObjects.Remove(projectile);
                OnPooledObjectNumberUpdated?.Invoke(PooledActiveObjectsNumber, PooledInactiveObjectsNumber);
            }
        }

        private void HandleNonPoolObjectDestroyed(Demo1Projectile nonProjectile)
        {
            nonProjectile.Destroyed -= HandleNonPoolObjectDestroyed;

            _instantiatedObjects.Remove(nonProjectile);
            _allSpawnedObjects.Remove(nonProjectile);

            OnInstantiatedObjectNumberUpdated?.Invoke(InstantiatedObjectsNumber);
        }

        private Vector3 GetMousePosition()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = distanceFromCamera;

            mousePosition = _camera.ScreenToWorldPoint(mousePosition);

            return mousePosition;
        }
    }
}