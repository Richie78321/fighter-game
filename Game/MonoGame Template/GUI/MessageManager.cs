using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameGUI;
using Microsoft.Xna.Framework;
using FighterGame.Runtime;

namespace FighterGame.GUI
{
    public class MessageManager : GUIPanel
    {
        private GameSession gameSession;
        public MessageManager(Rectangle elementRectangle, GameSession gameSession) : base(elementRectangle)
        {
            this.gameSession = gameSession;
        }

        public void AddMessage(string message)
        {
            AddElement(new MessageBox(message, gameSession.LatestGameTime, RemoveMessage));
        }

        public void AddMessage(MessageBox message)
        {
            AddElement(message);
        }

        public void RemoveMessage(MessageBox message)
        {
            RemoveElement(message);
        }

        public void ClearMessages() { ClearElements(); }
    }
}
