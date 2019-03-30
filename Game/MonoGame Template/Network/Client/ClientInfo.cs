using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Network.Client
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public class ClientInfo
    {
        [ProtoMember(1)]
        public readonly LobbyInfo LobbyInfo;

        [ProtoMember(2)]
        public readonly string Username;

        public ClientInfo(LobbyInfo lobbyInfo, string username)
        {
            LobbyInfo = lobbyInfo;
            Username = username;
        }
    }
}
