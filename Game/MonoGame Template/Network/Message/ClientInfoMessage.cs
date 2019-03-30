using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using FighterGame.Network.Client;

namespace FighterGame.Network.Message
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class ClientInfoMessage : INetworkMessage
    {
        [ProtoMember(1)]
        public ClientInfo ClientInfo;

        public ClientInfoMessage(ClientInfo ClientInfo)
        {
            this.ClientInfo = ClientInfo;
        }
    }
}
