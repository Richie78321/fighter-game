using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.GUI
{
    public class GamePanel : GUIPanel
    {
        public static void LoadContent(ContentManager Content)
        {
            Texture2D topCorner = Content.Load<Texture2D>("GUI/Panel/windowCornerTop");
            Texture2D bottomCorner = Content.Load<Texture2D>("GUI/Panel/windowCornerBottom");
            PanelCorners = new GUITexture[] { new GUITexture(topCorner), new GUITexture(topCorner, SpriteEffects.FlipHorizontally), new GUITexture(bottomCorner), new GUITexture(bottomCorner, SpriteEffects.FlipHorizontally) };
            Texture2D windowSide = Content.Load<Texture2D>("GUI/Panel/windowSide");
            Texture2D windowSideTop = Content.Load<Texture2D>("GUI/Panel/windowSideTop");
            Texture2D windowSideBottom = Content.Load<Texture2D>("GUI/Panel/windowSideBottom");
            PanelSides = new GUITexture[] { new GUITexture(windowSide), new GUITexture(windowSide, SpriteEffects.FlipHorizontally), new GUITexture(windowSideTop), new GUITexture(windowSideBottom) };
            PanelMiddle = new GUITexture(Content.Load<Texture2D>("GUI/Panel/windowMiddle"));
        }

        private static GUITexture[] PanelCorners;
        private static GUITexture[] PanelSides;
        private static GUITexture PanelMiddle;

        //Object
        private int panelTileSize;
        private Point panelDimensions;

        public GamePanel(Rectangle elementRectangle) : base(elementRectangle, consumeMouse: true)
        {
            panelTileSize = Math.Min(elementRectangle.Height / 3, elementRectangle.Width / 3);
            panelDimensions = new Point(elementRectangle.Width / panelTileSize, elementRectangle.Height / panelTileSize);
            this.elementRectangle = new Rectangle(elementRectangle.X + (int)((elementRectangle.Width - (panelDimensions.X * panelTileSize)) / 2F), elementRectangle.Y + (int)((elementRectangle.Height - (panelDimensions.Y * panelTileSize)) / 2F), panelDimensions.X * panelTileSize, panelDimensions.Y * panelTileSize);
        }

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            Color drawColor = new Color(drawSpecifics.Opacity, drawSpecifics.Opacity, drawSpecifics.Opacity, drawSpecifics.Opacity);

            //Draw panel
            for (int i = 0; i < panelDimensions.X; i++)
            {
                for (int j = 0; j < panelDimensions.Y; j++)
                {
                    bool top = j == 0, bottom = j == panelDimensions.Y - 1, left = i == 0, right = i == panelDimensions.X - 1;
                    if (top)
                    {
                        if (left)
                        {
                            spriteBatch.Draw(PanelCorners[0].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelCorners[0].SpriteEffect, 0);
                        }
                        else if (right)
                        {
                            spriteBatch.Draw(PanelCorners[1].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelCorners[1].SpriteEffect, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(PanelSides[2].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelSides[2].SpriteEffect, 0);
                        }
                    }
                    else if (bottom)
                    {
                        if (left)
                        {
                            spriteBatch.Draw(PanelCorners[2].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelCorners[2].SpriteEffect, 0);
                        }
                        else if (right)
                        {
                            spriteBatch.Draw(PanelCorners[3].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelCorners[3].SpriteEffect, 0);
                        }
                        else
                        {
                            spriteBatch.Draw(PanelSides[3].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelSides[3].SpriteEffect, 0);
                        }
                    }
                    else if (left)
                    {
                        spriteBatch.Draw(PanelSides[0].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelSides[0].SpriteEffect, 0);
                    }
                    else if (right)
                    {
                        spriteBatch.Draw(PanelSides[1].Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelSides[1].SpriteEffect, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(PanelMiddle.Texture, new Rectangle(elementRectangle.X + (panelTileSize * i), elementRectangle.Y + (panelTileSize * j), panelTileSize, panelTileSize), null, drawColor, 0, Vector2.Zero, PanelMiddle.SpriteEffect, 0);
                    }
                }
            }

            base.HandleDraw(spriteBatch, scissorRectangle, drawSpecifics);
        }
    }
}
