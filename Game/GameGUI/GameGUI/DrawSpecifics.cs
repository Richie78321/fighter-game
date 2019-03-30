using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGUI
{
    public struct DrawSpecifics
    {
        public float Scale;
        public float Opacity;

        public DrawSpecifics(float Scale, float Opacity = 1F)
        {
            this.Scale = Scale;
            this.Opacity = Opacity;
        }
    }
}
