using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Map.Background;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.Map
{
    public class MapTexturePack
    {
        public readonly PlatformTexturePack PlatformTexturePack;
        public readonly BackgroundImage[] BackgroundImages;

        public MapTexturePack(PlatformTexturePack PlatformTexturePack, BackgroundImage[] BackgroundImages)
        {
            this.PlatformTexturePack = PlatformTexturePack;
            this.BackgroundImages = BackgroundImages;
        }
    }

    public class PlatformTexturePack
    {
        public enum PlatformTexture
        {
            Left,
            Middle,
            Right
        }

        public readonly Texture2D[] PlatformTop;
        public readonly Texture2D[] PlatformBottom;

        public PlatformTexturePack(Texture2D[] PlatformTop, Texture2D[] PlatformBottom)
        {
            this.PlatformBottom = PlatformBottom;
            this.PlatformTop = PlatformTop;
        }
    }
}
