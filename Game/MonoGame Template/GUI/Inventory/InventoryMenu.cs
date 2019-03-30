using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using GameGUI;
using Microsoft.Xna.Framework;

namespace FighterGame.GUI.Inventory
{
    public class InventoryMenu : Menu
    {
        private const float SCREEN_HEIGHT_PORTION = .8F;

        //Object
        private GameSession gameSession;

        public InventoryMenu(Rectangle elementRectangle, GameSession gameSession) : base(elementRectangle)
        {
            this.gameSession = gameSession;

            SetMenu(new IMenuElementData[] { GameButton.BackButtonData(gameSession) }, SCREEN_HEIGHT_PORTION);
            AddElement(new InventoryPanel(new Rectangle(elementRectangle.X, elementRectangle.Y, elementRectangle.Width, (int)Math.Floor(elementRectangle.Height * SCREEN_HEIGHT_PORTION))));
        }
    }
}
