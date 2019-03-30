using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameGUI;

namespace FighterGame.GUI
{
    public class MessageBox : GamePanel
    {
        private const float PADDING_SCALE = .5F;
        public static GraphicsDevice GraphicsDevice;
        private const int FADE_TIME = 50;

        public delegate void RemoveMessage(MessageBox messageBox);

        //Object
        private RemoveMessage removeMessage;

        public MessageBox(string message, GameTime gameTime, RemoveMessage removeMessage, string dismissButtonText = "Dismiss", GUIButton.ButtonAction OnDismiss = null) : base(new Rectangle((int)((GraphicsDevice.Viewport.Width * PADDING_SCALE) / 2), (int)((GraphicsDevice.Viewport.Height * PADDING_SCALE) / 2), (int)(GraphicsDevice.Viewport.Width * (1F - PADDING_SCALE)), (int)(GraphicsDevice.Viewport.Height * (1F - PADDING_SCALE))))
        {
            this.removeMessage = removeMessage;
            if (OnDismiss == null) OnDismiss = new GUIButton.ButtonAction((e) => { });

            AddElement(new GUILabel(new Point(elementRectangle.Center.X, elementRectangle.Center.Y - (elementRectangle.Height / 4)), message, GameButton.TextFont, Color.White, new Rectangle(0, 0, elementRectangle.Width, elementRectangle.Height / 2), wordWrap: true));
            AddElement(new GameButton(new Point(elementRectangle.Center.X, elementRectangle.Center.Y + (elementRectangle.Height / 4)), new Rectangle(0, 0, (int)(elementRectangle.Width * PADDING_SCALE), (int)((elementRectangle.Height / 2) * PADDING_SCALE)), new GameButtonData(dismissButtonText, Dismiss_Click + OnDismiss, null)));
        }

        private float FadeOpacity = 0F;
        private bool remove = false;
        protected override void HandleDraw(SpriteBatch spriteBatch, Rectangle scissorRectangle, DrawSpecifics drawSpecifics)
        {
            drawSpecifics.Opacity = FadeOpacity;
            base.HandleDraw(spriteBatch, scissorRectangle, drawSpecifics);
        }

        protected override void HandleUpdate(GameTime gameTime)
        {
            base.HandleUpdate(gameTime);
            if (!remove && FadeOpacity < 1F) FadeOpacity = MathHelper.Clamp(FadeOpacity + ((1F / FADE_TIME) * (float)gameTime.ElapsedGameTime.TotalMilliseconds), 0, 1F);
            else if (remove)
            {
                FadeOpacity = MathHelper.Clamp(FadeOpacity - ((1F / FADE_TIME) * (float)gameTime.ElapsedGameTime.TotalMilliseconds), 0, 1F);
                if (FadeOpacity == 0) removeMessage.Invoke(this);
            }
        }

        public void Dismiss_Click(GUIButton button)
        {
            remove = true;
        }
    }
}
