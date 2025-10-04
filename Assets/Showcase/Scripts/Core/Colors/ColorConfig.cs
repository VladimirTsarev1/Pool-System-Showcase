using Showcase.Scripts.Core.Constants;
using UnityEngine;

namespace Showcase.Scripts.Core.Colors
{
    [CreateAssetMenu(fileName = "New Color Config", menuName = PathConstants.ScriptableObjects + "/Color Config")]
    public sealed class ColorConfig : ScriptableObject
    {
        [SerializeField] private ColorPaletteType colorPaletteType;
        [SerializeField] private Color[] colors;

        public ColorPaletteType ColorPaletteType => colorPaletteType;
        public Color[] Colors => colors;
    }
}