using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.Boundary
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public sealed class BoundaryZone : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;

        private void Awake()
        {
            Assert.IsNotNull(boxCollider, $"[{nameof(BoundaryZone)}] {nameof(boxCollider)} is null");
        }

        private void OnTriggerEnter(Collider other)
        {
            OutOfBound(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OutOfBound(other);
        }

        private void OutOfBound(Collider other)
        {
            if (other.TryGetComponent<IOutOfBounds>(out var boundaryObject))
            {
                boundaryObject.OnOutOfBounds();
            }
        }

        public void SetBoxColliderSize(Vector3 size)
        {
            boxCollider.size = size;
        }

        public void SetBoxColliderSize(float xSize = 1f, float ySize = 1f, float zSize = 1f)
        {
            var newSize = new Vector3(xSize, ySize, zSize);

            SetBoxColliderSize(newSize);
        }
    }
}