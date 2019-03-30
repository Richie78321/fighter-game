using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace FighterGame.Network.Client
{
    [ProtoContract(SkipConstructor = true)]
    public class PeerClientInfo
    {
        [ProtoMember(1)]
        public readonly string Username;

        [ProtoMember(2)]
        public readonly int ClientIndex;

        public PeerClientInfo(int ClientIndex, string Username)
        {
            this.Username = Username;
            this.ClientIndex = ClientIndex;
        }
    }
}
