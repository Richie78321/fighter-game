using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FighterGame.Runtime;

namespace FighterGame.Entities
{
    public abstract class LocalEntity : Entity
    {
        public LocalEntity(bool localEntity = true) : base(localEntity)
        {
        }

        public abstract void SendNetworkEntityData(NetworkManager networkManager);
    }
}
