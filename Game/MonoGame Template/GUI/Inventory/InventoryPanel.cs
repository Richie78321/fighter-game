using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.GUI.Inventory
{
    public class InventoryPanel : GUIPanel
    {
        private const int MIN_ITEM_TILES = 5;

        private enum EdgeTexture
        {
            UpLeft,
            UpMiddle,
            UpTextLeft,
            UpTextRight,
            UpRight,
            DownLeft,
            DownTextLeft,
            DownTextRight,
            DownMiddle,
            DownRight,
            LeftMiddle,
            RightMiddle,
        }
        private static Texture2D inventorySlot;
        private static Texture2D[] edgeTextures;
        public static void LoadContent(ContentManager Content)
        {
            inventorySlot = Content.Load<Texture2D>("GUI/Inventory/inventorySlot");

            string[] enumNames = Enum.GetNames(typeof(EdgeTexture));
            edgeTextures = new Texture2D[enumNames.Length];
            for (int i = 0; i < edgeTextures.Length; i++) edgeTextures[i] = Content.Load<Texture2D>("GUI/Inventory/" + enumNames[i]);
        }

        //Object
        private int tileWidth, tileHeight;
        private int tileSize;
        private int xOffset, yOffset;

        public InventoryPanel(Rectangle elementRectangle) : base(elementRectangle, true, false)
        {
            tileSize = Math.Min(ElementRectangle.Width, ElementRectangle.Height) / MIN_ITEM_TILES;
            tileWidth = ElementRectangle.Width / tileSize;
            tileHeight = ElementRectangle.Height / tileSize;
            xOffset = (ElementRectangle.Width - (tileWidth * tileSize)) / 2;
            yOffset = (ElementRectangle.Height - (tileHeight * tileSize)) / 2;
        }

        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            //Draw tiles
            spriteBatch.End();
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            for (int i = 0; i < tileWidth; i++)
            {
                for (int j = 0; j < tileHeight; j++)
                {
                    bool isEdge = false;
                    EdgeTexture edgeTexture = EdgeTexture.DownLeft;
                    if (i == 0)
                    {
                        isEdge = true;
                        if (j == 0) edgeTexture = EdgeTexture.UpLeft;
                        else if (j == tileHeight - 1) edgeTexture = EdgeTexture.DownLeft;
                        else edgeTexture = EdgeTexture.LeftMiddle;
                    }
                    else if (i == tileWidth - 1)
                    {
                        isEdge = true;
                        if (j == 0) edgeTexture = EdgeTexture.UpRight;
                        else if (j == tileHeight - 1) edgeTexture = EdgeTexture.DownRight;
                        else edgeTexture = EdgeTexture.RightMiddle;
                    }
                    else if (j == 0)
                    {
                        isEdge = true;
                        if (i == 1) edgeTexture = EdgeTexture.UpTextLeft;
                        else if (i == tileWidth - 2) edgeTexture = EdgeTexture.UpTextRight;
                        else edgeTexture = EdgeTexture.UpMiddle;
                    }
                    else if (j == tileHeight - 1)
                    {
                        isEdge = true;
                        if (i == 1) edgeTexture = EdgeTexture.DownTextLeft;
                        else if (i == tileWidth - 2) edgeTexture = EdgeTexture.DownTextRight;
                        else edgeTexture = EdgeTexture.DownMiddle;
                    }

                    if (isEdge) spriteBatch.Draw(edgeTextures[(int)edgeTexture], new Vector2(xOffset + (tileSize * (i + .5F)), yOffset + (tileSize * (j + .5F))), null, Color.White, 0, new Vector2(edgeTextures[(int)edgeTexture].Width / 2, edgeTextures[(int)edgeTexture].Height / 2), (float)tileSize / edgeTextures[(int)edgeTexture].Width, SpriteEffects.None, 0);
                    else spriteBatch.Draw(inventorySlot, new Vector2(xOffset + (tileSize * (i + .5F)), yOffset + (tileSize * (j + .5F))), null, Color.White, 0, new Vector2(edgeTextures[(int)edgeTexture].Width / 2, edgeTextures[(int)edgeTexture].Height / 2), (float)tileSize / edgeTextures[(int)edgeTexture].Width, SpriteEffects.None, 0);
                }
            }

            base.HandleDraw(spriteBatch, scissorRectangle, drawSpecifics);
        }
    }
}
