using GameGUI;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FighterGame.GUI.Health
{
    public class HealthBarManager : GUIPanel
    {
        private const float MAX_WIDTH_PORTION = .2F;
        private const int MAX_PLAYERS_PER_COLUMN = 8;

        //Object
        private Vector2 healthBarDimensions;

        public HealthBarManager(GraphicsDevice graphicsDevice) : base(new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height))
        {
            float heightDimension = Math.Min(graphicsDevice.Viewport.Height / MAX_PLAYERS_PER_COLUMN, graphicsDevice.Viewport.Width * MAX_WIDTH_PORTION / 2);
            healthBarDimensions = new Vector2(heightDimension * 2, heightDimension);
        }

        private int containedHealthBars = 0;
        public HealthBar AddHealthBar(string entityName, float initialHealth, GameTime gameTime, bool incrementInName = false)
        {
            string name = entityName;
            if (incrementInName) name += " " + (containedHealthBars + 1).ToString();

            HealthBar newHealthBar = new HealthBar(name, initialHealth, new Rectangle((int)Math.Ceiling((containedHealthBars / MAX_PLAYERS_PER_COLUMN) * healthBarDimensions.X), (int)Math.Ceiling(healthBarDimensions.Y * (containedHealthBars % MAX_PLAYERS_PER_COLUMN)), (int)Math.Floor(healthBarDimensions.X), (int)Math.Floor(healthBarDimensions.Y)), gameTime);
            AddElement(newHealthBar);
            containedHealthBars++;
            return newHealthBar;
        }

        public void ClearHealthBars()
        {
            containedHealthBars = 0;
            ClearElements();
        }
    }
}
