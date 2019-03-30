using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public interface INetworkMessage
    {
    }
}
