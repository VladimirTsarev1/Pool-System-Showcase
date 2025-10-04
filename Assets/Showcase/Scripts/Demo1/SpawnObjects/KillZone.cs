using Showcase.Scripts.Boundary;
using UnityEngine;

namespace Showcase.Scripts.Demo1.SpawnObjects
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public sealed class KillZone : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IOutOfBounds>(out var boundaryObject))
            {
                boundaryObject.OnOutOfBounds();
            }
        }
    }
}