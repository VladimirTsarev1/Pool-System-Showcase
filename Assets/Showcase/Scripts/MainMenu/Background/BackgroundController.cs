using Showcase.Scripts.Boundary;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.MainMenu.Background
{
    [DisallowMultipleComponent]
    public sealed class BackgroundController : MonoBehaviour
    {
        [SerializeField] private BoundariesSpawner boundariesSpawner;
        [SerializeField] private BackgroundObjectSpawner[] spawners;

        [SerializeField] private float distanceFromCamera = 30f;
        [SerializeField] private float boundaryOffsetFromEdge = 10f;
        [SerializeField] private float boundaryZSize = 10f;

        private Camera _camera;

        private void Awake()
        {
            Assert.IsNotNull(boundariesSpawner,
                $"[{nameof(BackgroundController)}] {nameof(boundariesSpawner)} is null");
            Assert.IsTrue(spawners.Length > 0, $"[{nameof(BackgroundController)}] Spawners length is 0");

            for (int i = 0; i < spawners.Length; i++)
            {
                Assert.IsNotNull(spawners[i], $"[{nameof(BackgroundController)}] Spawner is null");
            }
        }

        public void Initialize()
        {
            _camera = Camera.main;

            Assert.IsNotNull(_camera, $"[{nameof(BackgroundController)}] {nameof(_camera)} is null");
            
            boundariesSpawner.SetupBoundary(_camera, distanceFromCamera, boundaryOffsetFromEdge, boundaryZSize);

            StartSpawners();
        }

        public void DeactivateBackground()
        {
            StopSpawners();
        }

        private void StartSpawners()
        {
            foreach (var spawner in spawners)
            {
                if (spawner)
                {
                    spawner.StartSpawn(_camera, distanceFromCamera, boundaryZSize);
                }
            }
        }

        private void StopSpawners()
        {
            foreach (var spawner in spawners)
            {
                spawner?.StopSpawn();
            }
        }
    }
}