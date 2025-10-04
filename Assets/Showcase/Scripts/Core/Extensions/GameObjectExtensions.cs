using UnityEngine;

namespace Showcase.Scripts.Core.Extensions
{
    public static class GameObjectExtensions
    {
        public static void Activate(this GameObject target)
        {
            if (!target)
            {
                return;
            }

            target.SetActive(true);
        }

        public static void Deactivate(this GameObject target)
        {
            if (!target)
            {
                return;
            }

            target.SetActive(false);
        }
    }
}