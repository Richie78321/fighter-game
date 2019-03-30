using FighterGame.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Entities
{
    class BasicSpriteManager
    {
        private SpriteSheet spriteSheet;
        public SpriteSheet SpriteSheet => spriteSheet;

        private int currentIndex = 0;

        public void SetSpriteSheet(GameTime gameTime, SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            currentIndex = 0;
            timeOfLastFrameChange = gameTime.TotalGameTime.TotalMilliseconds;
        }

        public BasicSpriteManager(GameTime gameTime, SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            timeOfLastFrameChange = gameTime.TotalGameTime.TotalMilliseconds;
        }

        private double timeOfLastFrameChange;
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds - timeOfLastFrameChange >= spriteSheet.FrameTime)
            {
                timeOfLastFrameChange = gameTime.TotalGameTime.TotalMilliseconds;
                currentIndex++;
                if (currentIndex >= spriteSheet.textures.Length)
                {
                    OnSpriteEnd.Invoke(this, null);
                    currentIndex %= spriteSheet.textures.Length;
                }
            }
        }

        public Texture2D CurrentTexture => spriteSheet.textures[currentIndex];
        public event EventHandler OnSpriteEnd = new EventHandler((e, s) => { });
    }
}
