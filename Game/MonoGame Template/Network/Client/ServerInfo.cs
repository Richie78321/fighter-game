using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace FighterGame.Network.Client
{
    [Serializable]
    [ProtoContract(SkipConstructor = true)]
    public struct ServerInfo
    {
        [ProtoMember(1)]
        public int RandomSeed;
        [ProtoMember(2)]
        public byte ClientIndex;

        public ServerInfo(int RandomSeed, byte ClientIndex)
        {
            this.RandomSeed = RandomSeed;
            this.ClientIndex = ClientIndex;
        }
    }
}
