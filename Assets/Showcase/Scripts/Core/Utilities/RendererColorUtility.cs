using Showcase.Scripts.Core.Colors;
using Showcase.Scripts.Core.Constants;
using Showcase.Scripts.Providers;
using UnityEngine;

namespace Showcase.Scripts.Core.Utilities
{
    public static class RendererColorUtility
    {
        private static readonly MaterialPropertyBlock PropertyBlock = new MaterialPropertyBlock();

        public static void SetRandomColor(Renderer renderer, ColorPaletteType colorPaletteType)
        {
            if (!renderer)
            {
                Debug.LogError($"{nameof(RendererColorUtility)} {nameof(renderer)} is null)");
                return;
            }

            var randomColor = ConfigProvider.Colors.GetRandomColor(colorPaletteType);

            PropertyBlock.SetColor(MaterialConstants.Color, randomColor);

            renderer.SetPropertyBlock(PropertyBlock, 0);
        }
    }
}