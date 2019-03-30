using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FighterGame.Inventory
{
    public class Item
    {
        private int itemSeed;
        private int itemLevel;

        public Item(int itemSeed, int itemLevel)
        {
            this.itemLevel = itemLevel;
            this.itemSeed = itemSeed;
        }
    }
}
