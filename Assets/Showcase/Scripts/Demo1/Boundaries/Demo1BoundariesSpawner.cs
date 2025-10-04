using Showcase.Scripts.Boundary;
using Showcase.Scripts.Core.Utilities;
using Showcase.Scripts.Demo1.SpawnObjects;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.Demo1.Boundaries
{
    [DisallowMultipleComponent]
    public sealed class Demo1BoundariesSpawner : BoundariesSpawner
    {
        [SerializeField] public KillZone killZonePrefab;

        private void Awake()
        {
            Assert.IsNotNull(killZonePrefab, $"[{nameof(Demo1BoundariesSpawner)}] {nameof(killZonePrefab)} is null");
        }

        public override void SetupBoundary(Camera cameraComponent, float distance, float offsetFromEdge,
            float zSize = 1)
        {
            base.SetupBoundary(cameraComponent, distance, offsetFromEdge, zSize);

            SpawnKillZone(cameraComponent, distance);
        }

        private void SpawnKillZone(Camera cameraComponent, float distance)
        {
            var killZone = Instantiate(killZonePrefab);

            killZone.transform.position =
                ViewportCornersUtility.GetWorldPoint(cameraComponent, ViewportCornersUtility.Center, distance);
        }
    }
}