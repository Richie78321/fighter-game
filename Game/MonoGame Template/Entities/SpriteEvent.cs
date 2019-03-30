using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FighterGame.Textures;
using FighterGame.Entities.Player;
using PolygonalPhysics;

namespace FighterGame.Entities
{
    public class SpriteEvent
    {
        public SpriteSheet spriteSheet;
        public int spriteSheetPos = 0;
        public int FrameTime => spriteSheet.FrameTime;
        public double frameChangeTime;
        public readonly bool stickFrame;

        public SpriteEvent(SpriteSheet spriteSheet, GameTime gameTime, bool stickFrame = false)
        {
            this.spriteSheet = spriteSheet;
            this.stickFrame = stickFrame;
            frameChangeTime = gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}
