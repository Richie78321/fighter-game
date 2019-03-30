using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace GameGUI
{
    public class GUIInput
    {
        public readonly MouseState MouseState, PastMouseState;
        public bool LeftConsumed = false;
        public bool RightConsumed = false;
        public bool MouseConsumed = false;

        public readonly KeyboardState KeyboardState, PastKeyboardState;

        public GUIInput(KeyboardState KeyboardState, KeyboardState PastKeyboardState, MouseState MouseState, MouseState PastMouseState)
        {
            this.KeyboardState = KeyboardState;
            this.PastKeyboardState = PastKeyboardState;
            this.MouseState = MouseState;
            this.PastMouseState = PastMouseState;
        }
    }
}
