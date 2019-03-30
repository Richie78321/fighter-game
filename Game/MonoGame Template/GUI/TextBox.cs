using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameGUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FighterGame.GUI
{
    public class TextBox : GamePanel
    {
        public delegate void OnTextUpdate(string newText);

        //Object
        private bool censoredInput;

        public event OnTextUpdate OnTextUpdateEvent = new OnTextUpdate((s) => { });

        public TextBox(Rectangle elementRectangle, GameWindow gameWindow, string defaultText = "", bool censoredInput = false, OnTextUpdate onTextUpdateEvent = null) : base(elementRectangle)
        {
            this.censoredInput = censoredInput;
            if (onTextUpdateEvent != null) OnTextUpdateEvent += onTextUpdateEvent;

            if (!string.IsNullOrEmpty(defaultText))
            {
                this.defaultText = true;
                textLabel = new GUILabel(elementRectangle.Center, defaultText, GameButton.TextFont, Color.LightGray, elementRectangle);
                AddElement(textLabel);
            }

            gameWindow.TextInput += GameWindow_TextInput;
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                if (!value) selected = false;
                base.Enabled = value;
            }
        }

        private string characterQueue = "";
        private int backspace = 0;
        private void GameWindow_TextInput(object sender, TextInputEventArgs e)
        {
            if (selected)
            {
                if (e.Character == '\b') backspace++;
                else if (GameButton.TextFont.Characters.Contains(e.Character)) characterQueue += e.Character;
            }
        }

        public string Text => textLabel.Text;
        private GUILabel textLabel = null;
        private bool defaultText = false;

        private bool selected = false;
        protected override void HandleInteract(GUIInput guiInput)
        {
            base.HandleInteract(guiInput);

            if (guiInput.PastMouseState.LeftButton == ButtonState.Pressed && guiInput.MouseState.LeftButton == ButtonState.Released)
            {
                //Has clicked
                if (elementRectangle.Contains(guiInput.MouseState.Position) && !guiInput.LeftConsumed)
                {
                    selected = true;
                    if (textLabel != null) textLabel.TextColor = Color.White;
                }
                else
                {
                    selected = false;
                    if (textLabel != null) textLabel.TextColor = Color.LightGray;
                }
            }
        }

        protected override void HandleUpdate(GameTime gameTime)
        {
            base.HandleUpdate(gameTime);

            if (selected && (characterQueue.Length > 0 || backspace > 0))
            {
                //Update GUI Label
                string newString = "";
                if (textLabel != null && !defaultText)
                {
                    newString = textLabel.Text;
                }
                RemoveElement(textLabel);
                newString += characterQueue;
                characterQueue = "";

                newString = newString.Substring(0, MathHelper.Clamp(newString.Length - backspace, 0, newString.Length));
                defaultText = false;
                backspace = 0;
                if (censoredInput) textLabel = new GUILabel(elementRectangle.Center, newString, GameButton.TextFont, Color.White, elementRectangle, GUILabel.DEFAULT_CENSORED_CHAR);
                else textLabel = new GUILabel(elementRectangle.Center, newString, GameButton.TextFont, Color.White, elementRectangle);
                AddElement(textLabel);

                OnTextUpdateEvent.Invoke(newString);
            }
        }
    }

    public class TextBoxData : IMenuElementData
    {
        private GameWindow gameWindow;
        private string defaultText;
        private bool censoredInput;
        private TextBox.OnTextUpdate onTextUpdate;

        public TextBoxData(GameWindow gameWindow, string defaultText = "", bool censoredInput = false, TextBox.OnTextUpdate onTextUpdate = null)
        {
            this.onTextUpdate = onTextUpdate;
            this.censoredInput = censoredInput;
            this.defaultText = defaultText;
            this.gameWindow = gameWindow;
        }

        public GUIElement DeployElement(Point centerPoint, Rectangle elementRestrictions)
        {
            return new TextBox(new Rectangle(centerPoint.X - (elementRestrictions.Width / 2), centerPoint.Y - (elementRestrictions.Height / 2), elementRestrictions.Width, elementRestrictions.Height), gameWindow, defaultText, censoredInput, onTextUpdate);
        }
    }
}
