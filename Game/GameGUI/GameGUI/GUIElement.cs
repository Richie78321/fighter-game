using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameGUI
{
    public abstract class GUIElement
    {
        protected Rectangle elementRectangle;

        private bool _Visible = true;

        private bool _Enabled = true;

        private KeyboardState pastKeyboardState = Keyboard.GetState();
        protected KeyboardState PastKeyboardState => pastKeyboardState;

        private MouseState pastMouseState = Mouse.GetState();
        protected MouseState PastMouseState => pastMouseState;

        public Rectangle ElementRectangle => elementRectangle;

        public Vector2 Location => new Vector2((float)elementRectangle.Location.X, (float)elementRectangle.Location.Y);

        public virtual bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                _Visible = value;
            }
        }

        public virtual bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                _Enabled = value;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                HandleUpdate(gameTime);
            }
        }

        protected abstract void HandleUpdate(GameTime gameTime);

        public void Interact(GUIInput GUIInput)
        {
            if (Enabled)
            {
                HandleInteract(GUIInput);
            }

            pastKeyboardState = GUIInput.KeyboardState;
            pastMouseState = GUIInput.MouseState;
        }

        protected abstract void HandleInteract(GUIInput GUIInput);

        public void Draw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            if (Visible)
            {
                spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                HandleDraw(spriteBatch, scissorRectangle, drawSpecifics);
            }
        }

        protected abstract void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics);
    }
}
