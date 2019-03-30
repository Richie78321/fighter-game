using FighterGame.Map;
using FighterGame.Map.Background;
using FighterGame.Runtime;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.GUI
{
    public class BackgroundPanel : GUIPanel
    {
        private GameSession gameSession;

        public BackgroundPanel(Rectangle elementRectangle, GameSession gameSession) : base(elementRectangle)
        {
            this.gameSession = gameSession;

            Random random = new Random();
            MapTexturePack texturePack = GameMap.MapTexturePacks[random.Next(0, GameMap.MapTexturePacks.Length)].Deploy();
            parallaxBackground = new ParallaxBackground(texturePack.BackgroundImages);
        }

        private ParallaxBackground parallaxBackground;

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            spriteBatch.End();
            float focusPointScale = (float)MapStandards.MAP_SIZE / Math.Min(scissorRectangle.Width, scissorRectangle.Height);
            parallaxBackground.Draw(spriteBatch, new Vector2(PastMouseState.Position.X * focusPointScale, PastMouseState.Position.Y * focusPointScale), MapStandards.MAP_SIZE / Math.Min(scissorRectangle.Width, scissorRectangle.Height), gameSession.graphicsDevice);
            spriteBatch.Begin();

            base.HandleDraw(spriteBatch, scissorRectangle, drawSpecifics);
        }
    }
}
