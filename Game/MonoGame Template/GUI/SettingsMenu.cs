using FighterGame.Runtime;
using GameGUI;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.GUI
{
    public class SettingsMenu : Menu
    {
        private const float SETTINGS_SCREEN_PORTION = 0F;

        //Object
        private GameSession gameSession;

        public SettingsMenu(GraphicsDevice graphicsDevice, GameSession gameSession) : base(new Microsoft.Xna.Framework.Rectangle(graphicsDevice.Viewport.Width / 4, 0, graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height))
        {
            this.gameSession = gameSession;
            SetMenu(DeployGameButtonData(), SETTINGS_SCREEN_PORTION);
        }

        private IMenuElementData[] DeployGameButtonData()
        {
            List<IMenuElementData> gameButtonData = new List<IMenuElementData>()
            {
                new GameButtonData("Target Lobby Size: " + gameSession.targetPlayers.ToString(), TargetLobbySize_ClickAction, null),
                new GameButtonData("Server Connection: " + gameSession.NetworkManager.CurrentConnectionType.ToString(), ServerConnection_ClickAction, null),
                new TextBoxData(gameSession.GameInstance.Window, defaultText: "Private Server IP", onTextUpdate: new TextBox.OnTextUpdate((s) => { gameSession.NetworkManager.PrivateServerIP = s; })),
                GameButton.BackButtonData(gameSession)
            };

            return gameButtonData.ToArray();
        }

        private void ServerConnection_ClickAction(GUIButton button)
        {
            gameSession.NetworkManager.CurrentConnectionType = (NetworkManager.ConnectionType)(((int)gameSession.NetworkManager.CurrentConnectionType + 1) % Enum.GetNames(typeof(NetworkManager.ConnectionType)).Length);
            button.UpdateText("Server Connection: " + gameSession.NetworkManager.CurrentConnectionType.ToString());
        }

        private void TargetLobbySize_ClickAction(GUIButton button)
        {
            gameSession.targetPlayers++;
            if (gameSession.targetPlayers > GameSession.MAX_TARGET_PLAYERS) gameSession.targetPlayers = GameSession.MIN_TARGET_PLAYERS;

            button.UpdateText("Target Lobby Size: " + gameSession.targetPlayers); 
        }
    }
}
