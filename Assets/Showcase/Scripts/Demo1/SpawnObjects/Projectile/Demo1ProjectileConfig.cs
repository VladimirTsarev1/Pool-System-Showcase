using Showcase.Scripts.Core.Constants;
using Showcase.Scripts.Projectile;
using UnityEngine;

namespace Showcase.Scripts.Demo1.SpawnObjects.Projectile
{
    [CreateAssetMenu(fileName = "Demo1 Projectile Config",
        menuName = PathConstants.ScriptableObjects + "/Demo1 Projectile")]
    public class Demo1ProjectileConfig : ProjectileConfig
    {
        [SerializeField] private float maxDepenetrationVelocity;
        
        public  float MaxDepenetrationVelocity => maxDepenetrationVelocity;
    }
}