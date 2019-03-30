using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using GameGUI;
using Microsoft.Xna.Framework;

namespace FighterGame.GUI
{
    public class LoginMenu : Menu
    {
        private const float SCREEN_HEIGHT_OFFSET = .6F;
        private const float TEXTBOX_PADDING_PORTION = .5F;

        //Object
        private GameSession gameSession;

        public LoginMenu(Rectangle elementRectangle, GameSession gameSession) : base(new Microsoft.Xna.Framework.Rectangle(elementRectangle.Width / 4, 0, elementRectangle.Width / 2, elementRectangle.Height))
        {
            this.gameSession = gameSession;

            SetMenu(DeployGameButtonData(), SCREEN_HEIGHT_OFFSET);

            Rectangle textBoxBounds = new Rectangle(this.elementRectangle.X, this.elementRectangle.Y, this.elementRectangle.Width, (int)Math.Floor(this.elementRectangle.Height * SCREEN_HEIGHT_OFFSET));
            usernameTextbox = new TextBox(new Rectangle(textBoxBounds.X, (int)((textBoxBounds.Height * TEXTBOX_PADDING_PORTION) / 3), textBoxBounds.Width, (int)((textBoxBounds.Height * (1F - TEXTBOX_PADDING_PORTION)) / 2)), gameSession.GameInstance.Window, "Username");
            passwordTextbox = new TextBox(new Rectangle(textBoxBounds.X, (int)((2 * textBoxBounds.Height * TEXTBOX_PADDING_PORTION) / 3) + (int)((textBoxBounds.Height * (1F - TEXTBOX_PADDING_PORTION)) / 2), textBoxBounds.Width, (int)((textBoxBounds.Height * (1F - TEXTBOX_PADDING_PORTION)) / 2)), gameSession.GameInstance.Window, "Password", censoredInput: true);
            AddElement(usernameTextbox);
            AddElement(passwordTextbox);
        }

        private TextBox usernameTextbox;
        private TextBox passwordTextbox;

        private IMenuElementData[] DeployGameButtonData()
        {
            List<IMenuElementData> gameButtonData = new List<IMenuElementData>()
            {
                new GameButtonData("Login", Login_ClickAction, null),
                new GameButtonData("Create Account", CreateAccount_ClickAction, null),
                new GameButtonData("Skip (DEBUG ONLY)", Skip_ClickAction, null)
            };

            return gameButtonData.ToArray();
        }

        private void Skip_ClickAction(GUIButton button)
        {
            gameSession.graphicsUI.SetCurrentMenu(gameSession.graphicsUI.MainMenu);
        }

        private async void CreateAccount_ClickAction(GUIButton button)
        {
            Enabled = false;
            if (!await gameSession.DatabaseManager.CreateAccount(usernameTextbox.Text, passwordTextbox.Text))
            {
                gameSession.graphicsUI.MessageManager.AddMessage("Failed to create account! '" + usernameTextbox.Text + "' may already be in use.");
                Enabled = true;
            }
            else Login_ClickAction(button);
        }

        private async void Login_ClickAction(GUIButton button)
        {
            Enabled = false;
            if (!await gameSession.DatabaseManager.AttemptLogin(usernameTextbox.Text, passwordTextbox.Text))
            {
                //Failed
                Enabled = true;
                gameSession.graphicsUI.MessageManager.AddMessage("Failed to Login! Please ensure your username and password are correct.");
            }
            else
            {
                //Go to main menu
                gameSession.graphicsUI.SetCurrentMenu(gameSession.graphicsUI.MainMenu);
            }
        }
    }
}
