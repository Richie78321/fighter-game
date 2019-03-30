using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameGUI
{
    public class GUIButton : GUIElement
    {
        public delegate void ButtonAction(GUIButton button);

        //Object
        private const int MINIMUM_TILES = 3;

        private int tileSize;

        private int tileCount;

        private GUILabel textLabel;
        public GUILabel TextLabel => textLabel;

        private Texture2D[] modularTextures;

        private bool hovered = false;

        protected event ButtonAction HoverAction;

        protected event ButtonAction UnHoverAction;

        protected event ButtonAction ClickAction;

        public GUIButton(Point centerPosition, Rectangle elementRestriction, string text, SpriteFont font, Color textColor, Texture2D[] tileSequence, ButtonAction ClickAction, bool consumeMouse = true, ButtonAction HoverAction = null, ButtonAction UnHoverAction = null)
        {
            this.ClickAction = ClickAction;
            if (HoverAction != null)
            {
                this.HoverAction = HoverAction;
            }
            else
            {
                this.HoverAction = delegate
                {
                };
            }
            if (UnHoverAction != null)
            {
                this.UnHoverAction = UnHoverAction;
            }
            else
            {
                this.UnHoverAction = new ButtonAction((e) => { });
            }
            if (tileSequence.Length != 6)
            {
                throw new Exception("GUI Button instantiated with incorrect tile sequence");
            }
            modularTextures = tileSequence;
            if (elementRestriction.Height * 3 > elementRestriction.Width)
            {
                tileSize = elementRestriction.Width / 3;
                tileCount = 3;
            }
            else
            {
                tileSize = elementRestriction.Height;
                tileCount = elementRestriction.Width / tileSize;
            }
            elementRectangle = new Rectangle(centerPosition.X - ((tileSize * tileCount) / 2), centerPosition.Y - (tileSize / 2), tileSize * tileCount, tileSize);
            textLabel = new GUILabel(centerPosition, text, font, textColor, elementRectangle);
        }

        public void UpdateText(string text)
        {
            textLabel = new GUILabel(elementRectangle.Center, text, textLabel.Font, textLabel.TextColor, elementRectangle);
        }

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            Color drawColor = new Color(drawSpecifics.Opacity, drawSpecifics.Opacity, drawSpecifics.Opacity, drawSpecifics.Opacity);

            int selectionOffset = 0;
            if (hovered)
            {
                selectionOffset += 3;
            }
            spriteBatch.Draw(modularTextures[selectionOffset], new Rectangle(elementRectangle.Location.X, elementRectangle.Location.Y, tileSize, tileSize), drawColor);
            for (int i = 1; i < tileCount - 1; i++)
            {
                spriteBatch.Draw(modularTextures[selectionOffset + 1], new Rectangle(elementRectangle.Location.X + tileSize * i, elementRectangle.Location.Y, tileSize, tileSize), drawColor);
            }
            spriteBatch.Draw(modularTextures[selectionOffset + 2], new Rectangle(elementRectangle.Right - tileSize, elementRectangle.Location.Y, tileSize, tileSize), drawColor);

            textLabel.Draw(spriteBatch, scissorRectangle, drawSpecifics);
        }

        protected override void HandleUpdate(GameTime gameTime)
        {
        }

        protected override void HandleInteract(GUIInput guiInput)
        {
            if (!guiInput.MouseConsumed)
            {
                if (elementRectangle.Contains(guiInput.MouseState.Position))
                {
                    if (!guiInput.LeftConsumed && guiInput.PastMouseState.LeftButton == ButtonState.Pressed && guiInput.MouseState.LeftButton == ButtonState.Released)
                    {
                        this.ClickAction(this);
                        guiInput.LeftConsumed = true;
                    }
                    else if (!hovered)
                    {
                        hovered = true;
                        this.HoverAction(this);
                    }
                    guiInput.MouseConsumed = true;
                }
                else
                {
                    if (hovered)
                    {
                        hovered = false;
                        this.UnHoverAction(this);
                    }
                }
            }
            else
            {
                if (hovered)
                {
                    hovered = false;
                    this.UnHoverAction(this);
                }
            }
        }
    }
}
