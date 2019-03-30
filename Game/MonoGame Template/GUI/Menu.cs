using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FighterGame.GUI
{
    public class Menu : GUIPanel
    {
        protected const float BUTTON_PADDING_PORTION = .3F;

        //Object
        public Menu(IMenuElementData[] elementData, Rectangle elementRectangle, float screenHeightOffset = 0F) : base(elementRectangle)
        {
            SetMenu(elementData, screenHeightOffset);
        }

        protected Menu(Rectangle elementRectangle) : base(elementRectangle)
        {
        }

        public void SetMenu(IMenuElementData[] elementData, float screenHeightOffset = 0F)
        {
            ClearElements();

            //Find button height
            int buttonHeight = (int)Math.Floor((elementRectangle.Height * ((1F - screenHeightOffset) - (BUTTON_PADDING_PORTION * (1F - screenHeightOffset)))) / elementData.Length);
            int buttonPadding = (int)Math.Floor(((elementRectangle.Height * (1F - screenHeightOffset)) * BUTTON_PADDING_PORTION) / (elementData.Length + 1));

            //Add buttons
            for (int i = 0; i < elementData.Length; i++) AddElement(elementData[i].DeployElement(new Point(elementRectangle.X + (elementRectangle.Width / 2), elementRectangle.Y + (int)((elementRectangle.Height * screenHeightOffset) + ((i + 1) * buttonPadding) + ((i + .5F) * buttonHeight))), new Rectangle(0, 0, elementRectangle.Width - (buttonPadding * 2), buttonHeight)));
        }
    }
}
