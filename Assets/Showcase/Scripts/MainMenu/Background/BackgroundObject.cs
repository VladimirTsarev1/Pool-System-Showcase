using System;
using Showcase.Scripts.Core.Colors;
using Showcase.Scripts.Core.Utilities;
using Showcase.Scripts.Pooling;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.MainMenu.Background
{
    [DisallowMultipleComponent]
    public sealed class BackgroundObject : PoolObject
    {
        public event Action Collided;

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody rigidbodyComponent;

        private void Awake()
        {
            Assert.IsNotNull(meshRenderer, $"[{nameof(BackgroundObject)}] {nameof(meshRenderer)} is null");
            Assert.IsNotNull(rigidbodyComponent, $"[{nameof(BackgroundObject)}] {nameof(rigidbodyComponent)} is null");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude > 1.5f
                && GetInstanceID() < collision.gameObject.GetInstanceID()
                && collision.gameObject.GetComponent<BackgroundObject>())
            {
                Collided?.Invoke();
            }
        }

        public void Setup(Vector3 newVelocity)
        {
            RendererColorUtility.SetRandomColor(meshRenderer, ColorPaletteType.MainMenuBackgroundObjects);

            rigidbodyComponent.angularVelocity = Vector3.zero;

            rigidbodyComponent.velocity = newVelocity;
        }

        protected override void ReturnToPool()
        {
            base.ReturnToPool();

            meshRenderer.SetPropertyBlock(null);
        }
    }
}