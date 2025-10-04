using System;
using Showcase.Scripts.Core.Colors;
using Showcase.Scripts.Core.Utilities;
using Showcase.Scripts.Pooling;
using UnityEngine;
using UnityEngine.Assertions;

namespace Showcase.Scripts.Demo1.SpawnObjects.Projectile
{
    [DisallowMultipleComponent]
    public sealed class Demo1Projectile : PoolObject
    {
        public event Action<Demo1Projectile> Destroyed;

        [SerializeField] private Demo1ProjectileConfig config;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody rigidbodyComponent;
        [SerializeField] private Collider colliderComponent;

        private bool _usePool;

        private void Awake()
        {
            Assert.IsNotNull(config, $"[{nameof(Demo1Projectile)}] {nameof(config)} is null");
            Assert.IsNotNull(meshRenderer, $"[{nameof(Demo1Projectile)}] {nameof(meshRenderer)} is null");
            Assert.IsNotNull(rigidbodyComponent, $"[{nameof(Demo1Projectile)}] {nameof(rigidbodyComponent)} is null");
            Assert.IsNotNull(colliderComponent, $"[{nameof(Demo1Projectile)}] {nameof(colliderComponent)} is null");

            rigidbodyComponent.maxDepenetrationVelocity = config.MaxDepenetrationVelocity;
        }

        protected override void OnDisable()
        {
            if (!_usePool)
            {
                return;
            }

            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            if (!_usePool)
            {
                Destroyed?.Invoke(this);
            }

            base.OnDestroy();
        }

        public void Setup(Vector3 size, Vector3 velocity, bool usePool, bool collisions)
        {
            RendererColorUtility.SetRandomColor(meshRenderer, ColorPaletteType.Demo1Objects);

            _usePool = usePool;

            SetSize(size);

            SetCollisionsState(collisions);

            rigidbodyComponent.angularVelocity = Vector3.zero;

            rigidbodyComponent.velocity = velocity;
        }

        public void SetCollisionsState(bool collisions)
        {
            colliderComponent.isTrigger = !collisions;
        }

        public void SetSize(Vector3 size)
        {
            transform.localScale = size;
        }

        public void Release()
        {
            ReturnToPool();
        }

        protected override void ReturnToPool()
        {
            if (_usePool)
            {
                base.ReturnToPool();

                meshRenderer.SetPropertyBlock(null);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}