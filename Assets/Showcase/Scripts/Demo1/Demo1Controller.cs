using Showcase.Scripts.Core.Patterns;
using Showcase.Scripts.Demo1.SpawnObjects;
using Showcase.Scripts.Demo1.UI.Options;
using Showcase.Scripts.Demo1.UI.Stats;
using Showcase.Scripts.SceneFlow;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.Demo1
{
    [DisallowMultipleComponent]
    public sealed class Demo1Controller : MonoBehaviour
    {
        [SerializeField] private OptionsPanel optionsPanel;
        [SerializeField] private StatsPanel statsPanel;
        [SerializeField] private ObjectSpawner spawner;

        private ISceneService _sceneService;

        private void Awake()
        {
            _sceneService = ServiceLocator.Resolve<ISceneService>();

            Assert.IsNotNull(_sceneService, $"[{nameof(Demo1Controller)}] {nameof(_sceneService)} is null");

            Assert.IsNotNull(optionsPanel, $"[{nameof(Demo1Controller)}] {nameof(optionsPanel)} is null");
            Assert.IsNotNull(statsPanel, $"[{nameof(Demo1Controller)}] {nameof(statsPanel)} is null");
            Assert.IsNotNull(spawner, $"[{nameof(Demo1Controller)}] {nameof(spawner)} is null");

            _sceneService.SceneLoadStarted += HandleSceneLoadStarted;

            optionsPanel.UsePoolChanged += HandleUsePoolChanged;
            optionsPanel.CollisionsChanged += HandleCollisionsChanged;
            optionsPanel.ShowStatsChanged += HandleShowStatsChanged;

            optionsPanel.SpawnTypeChanged += HandleSpawnTypeChanged;
            optionsPanel.DespawnPolicyChanged += HandleDespawnPolicyChanged;

            optionsPanel.ObjectLifetimeChanged += HandleObjectLifetimeChanged;
            optionsPanel.SpawnRateChanged += HandleRateChanged;
            optionsPanel.InitialVelocityChanged += HandleInitialVelocityChanged;
            optionsPanel.ObjectSizeChanged += HandleObjectSizeChanged;

            optionsPanel.ClearClicked += HandleClearClicked;

            spawner.OnPooledObjectNumberUpdated += HandlePooledObjectsNumberUpdated;
            spawner.OnInstantiatedObjectNumberUpdated += HandleInstantiatedObjectsNumberUpdated;

            InitializeScene();
        }

        private void OnDestroy()
        {
            if (optionsPanel)
            {
                optionsPanel.UsePoolChanged -= HandleUsePoolChanged;
                optionsPanel.CollisionsChanged -= HandleCollisionsChanged;
                optionsPanel.ShowStatsChanged -= HandleShowStatsChanged;

                optionsPanel.SpawnTypeChanged -= HandleSpawnTypeChanged;
                optionsPanel.DespawnPolicyChanged -= HandleDespawnPolicyChanged;

                optionsPanel.ObjectLifetimeChanged -= HandleObjectLifetimeChanged;
                optionsPanel.SpawnRateChanged -= HandleRateChanged;
                optionsPanel.InitialVelocityChanged -= HandleInitialVelocityChanged;
                optionsPanel.ObjectSizeChanged -= HandleObjectSizeChanged;

                optionsPanel.ClearClicked -= HandleClearClicked;
            }

            if (spawner)
            {
                spawner.OnPooledObjectNumberUpdated -= HandlePooledObjectsNumberUpdated;
                spawner.OnInstantiatedObjectNumberUpdated -= HandleInstantiatedObjectsNumberUpdated;
            }
        }

        private void InitializeScene()
        {
            optionsPanel.Initialize();
            spawner.Initialize();
        }

        #region Handlers

        private void HandleSceneLoadStarted()
        {
            _sceneService.SceneLoadStarted -= HandleSceneLoadStarted;

            spawner.BreakAndClear();
        }

        private void HandleUsePoolChanged(bool usePool)
        {
            spawner.SetUsePoolState(usePool);
        }

        private void HandleCollisionsChanged(bool collisions)
        {
            spawner.SetCollisionsState(collisions);
        }

        private void HandleShowStatsChanged(bool showStats)
        {
            statsPanel.gameObject.SetActive(showStats);
        }

        private void HandleClearClicked()
        {
            spawner.ClearPool();
        }

        private void HandleRateChanged(float spawnRate)
        {
            spawner.SetSpawnRate(spawnRate);
        }

        private void HandleSpawnTypeChanged(int spawnType)
        {
            spawner.SetSpawnType(spawnType);
        }

        private void HandleDespawnPolicyChanged(int despawnPolicyValue)
        {
            spawner.SetDespawnPolicy(despawnPolicyValue);
        }

        private void HandleObjectLifetimeChanged(float lifetime)
        {
            spawner.SetObjectLifetime(lifetime);
        }

        private void HandleInitialVelocityChanged(float velocity)
        {
            spawner.SetInitialVelocity(velocity);
        }

        private void HandleObjectSizeChanged(float size)
        {
            spawner.SetObjectsSize(size);
        }

        private void HandlePooledObjectsNumberUpdated(int activeObjectNumber, int inactiveObjectNumber)
        {
            statsPanel.UpdateActivePooledObjectsNumber(activeObjectNumber);
            statsPanel.UpdateInactivePooledObjectsNumber(inactiveObjectNumber);
        }

        private void HandleInstantiatedObjectsNumberUpdated(int instantiatedObjectNumber)
        {
            statsPanel.UpdateInstantiatedObjectsNumber(instantiatedObjectNumber);
        }

        #endregion
    }
}