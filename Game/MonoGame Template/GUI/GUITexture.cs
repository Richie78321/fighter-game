using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.GUI
{
    class GUITexture
    {
        public readonly Texture2D Texture;
        public readonly SpriteEffects SpriteEffect;

        public GUITexture(Texture2D Texture, SpriteEffects SpriteEffect = SpriteEffects.None)
        {
            this.Texture = Texture;
            this.SpriteEffect = SpriteEffect;
        }
    }
}
