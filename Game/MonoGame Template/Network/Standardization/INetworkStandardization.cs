using FighterGame.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Network.Standardization
{
    interface INetworkStandardization
    {
        Task<object> ReadObject();
        Task<bool> SendMessage(INetworkMessage messageToSend);
        void Disable();
    }
}
