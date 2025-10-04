using UnityEngine;

namespace Showcase.Scripts.Core.Extensions
{
    public static class ComponentExtensions
    {
        public static void Activate(this Component target)
        {
            if (!target || !target.gameObject)
            {
                return;
            }

            target.gameObject.SetActive(true);
        }

        public static void Deactivate(this Component target)
        {
            if (!target || !target.gameObject)
            {
                return;
            }

            target.gameObject.SetActive(false);
        }
    }
}