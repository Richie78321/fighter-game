using FighterGame.Entities;
using FighterGame.Textures;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.GUI.Health
{
    public class HealthBar : GUIPanel
    {
        private static SpriteSheet[] HealthIndicators;
        private const int HEALTH_INDICATOR_COUNT = 7;
        public static void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
        {
            HealthIndicators = new SpriteSheet[HEALTH_INDICATOR_COUNT];
            for (int i = 0; i < HEALTH_INDICATOR_COUNT; i++)
            {
                Texture2D texture = Content.Load<Texture2D>("GUI/Health/healthBar" + i.ToString());
                if (i != 5) HealthIndicators[i] = new SpriteSheet(texture, 1, 1, int.MaxValue, graphicsDevice);
                else HealthIndicators[i] = new SpriteSheet(texture, 2, 1, 100, graphicsDevice);
            }
        }

        //Object
        private readonly float initialHealth;

        private float currentHealth;
        public void SetCurrentHealth(float newHealth, GameTime gameTime)
        {
            currentHealth = newHealth;
            if (currentHealth <= 0)
            {
                healthIndicator.SetSpriteSheet(gameTime, HealthIndicators[HealthIndicators.Length - 1]);
            }
            else healthIndicator.SetSpriteSheet(gameTime, HealthIndicators[(int)MathHelper.Clamp(HealthIndicators.Length - 1 - (currentHealth / (initialHealth / (HealthIndicators.Length - 1))), 0, HealthIndicators.Length - 2)]);
        }

        private BasicSpriteManager healthIndicator;
        private float indicatorScale;

        public HealthBar(string entityName, float initialHealth, Rectangle elementRectangle, GameTime gameTime) : base(elementRectangle)
        {
            this.initialHealth = initialHealth;
            AddElement(new GUILabel(new Point(elementRectangle.X + ((2 * elementRectangle.Width) / 3), elementRectangle.Center.Y), entityName, GameButton.TextFont, Color.Black, new Rectangle(elementRectangle.X + (elementRectangle.Width / 3), elementRectangle.Y, (elementRectangle.Width * 2) / 3, elementRectangle.Height)));
            indicatorScale = Math.Min(elementRectangle.Height / HealthIndicators[0].textures[0].Height, elementRectangle.Width / HealthIndicators[0].textures[0].Width);

            healthIndicator = new BasicSpriteManager(gameTime, HealthIndicators[0]);
        }

        protected override void HandleUpdate(GameTime gameTime)
        {
            base.HandleUpdate(gameTime);

            healthIndicator.Update(gameTime);
        }

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            //Draw health indicator
            Texture2D texture = healthIndicator.CurrentTexture;
            spriteBatch.Draw(texture, new Vector2(elementRectangle.Center.X, elementRectangle.Center.Y), null, Color.White, 0, new Vector2(texture.Width / 2, texture.Height / 2), indicatorScale, SpriteEffects.None, 0);

            base.HandleDraw(spriteBatch, scissorRectangle, drawSpecifics);
        }
    }
}
