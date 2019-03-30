using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Textures;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.Entities
{
    public class EntityTexturePack
    {
        public readonly SpriteSheet[] LoopSheets;
        public readonly SpriteEvent[][] EventSheets;

        public EntityTexturePack(SpriteSheet[] loopSheets, SpriteEvent[][] eventSheets)
        {
            this.EventSheets = eventSheets;
            this.LoopSheets = loopSheets;
        }
    }
}
