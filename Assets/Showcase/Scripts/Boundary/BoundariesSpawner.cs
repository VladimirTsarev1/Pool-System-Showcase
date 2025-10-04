using System;
using Showcase.Scripts.Core.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.Boundary
{
    [DisallowMultipleComponent]
    public class BoundariesSpawner : MonoBehaviour
    {
        [SerializeField] public BoundaryZone boundaryPrefab;

        private void Awake()
        {
            Assert.IsNotNull(boundaryPrefab, $"[{nameof(BoundaryZone)}] {nameof(boundaryPrefab)} is null");
        }

        public virtual void SetupBoundary(Camera cameraComponent, float distance, float offsetFromEdge,
            float zSize = 1f)
        {
            if (!cameraComponent)
            {
                Debug.LogError($"[{nameof(BoundaryZone)}] {nameof(cameraComponent)} is null");
                return;
            }

            var bottom =
                SpawnBoundary(cameraComponent, ViewportCornersUtility.BottomCenter, Vector3.down * offsetFromEdge,
                    distance);
            var top =
                SpawnBoundary(cameraComponent, ViewportCornersUtility.TopCenter, Vector3.up * offsetFromEdge,
                    distance);
            var left =
                SpawnBoundary(cameraComponent, ViewportCornersUtility.LeftCenter, Vector3.left * offsetFromEdge,
                    distance);
            var right =
                SpawnBoundary(cameraComponent, ViewportCornersUtility.RightCenter, Vector3.right * offsetFromEdge,
                    distance);

            if (!bottom || !top || !left || !right)
            {
                return;
            }

            var horizontalSize = Vector3.Distance(left.transform.position, right.transform.position);
            var verticalSize = Vector3.Distance(bottom.transform.position, top.transform.position);

            bottom.SetBoxColliderSize(xSize: horizontalSize, zSize: zSize);
            top.SetBoxColliderSize(xSize: horizontalSize, zSize: zSize);
            left.SetBoxColliderSize(ySize: verticalSize, zSize: zSize);
            right.SetBoxColliderSize(ySize: verticalSize, zSize: zSize);
        }

        private BoundaryZone SpawnBoundary(Camera cameraComponent, Vector2 viewportPoint,
            Vector3 offset, float distance)
        {
            var worldPosition = ViewportCornersUtility.GetWorldPoint(cameraComponent, viewportPoint, distance);
            return Instantiate(boundaryPrefab, worldPosition + offset, Quaternion.identity, transform);
        }
    }
}