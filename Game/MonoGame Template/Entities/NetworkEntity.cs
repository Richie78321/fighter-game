using FighterGame.Entities.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Entities
{
    public abstract class NetworkEntity : Entity
    {
        private int clientIndex = -1;
        public int ClientIndex => clientIndex;
        public void SetClientIndex(int clientIndex)
        {
            if (this.clientIndex == -1) this.clientIndex = clientIndex;
        }

        public NetworkEntity() : base(localEntity: false) { }
    }
}
