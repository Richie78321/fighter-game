using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;
using ProtoBuf;

namespace FighterGame.Network.Message
{
    [ProtoContract(SkipConstructor = true)]
    public class TestPolygon : IClientMessage
    {
        [ProtoMember(1)]
        private PolygonalPhysics.PortablePolygon portablePolygon;

        public TestPolygon(PolygonalPhysics.Polygon polygon)
        {
            portablePolygon = new PolygonalPhysics.PortablePolygon(polygon);
        }

        public void ClientAction(GameSession gameSession)
        {
            FighterGame.Map.GameMap.SERVERCP = portablePolygon.DeployPolygon();
        }
    }
}
