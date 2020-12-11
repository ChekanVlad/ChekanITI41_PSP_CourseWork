using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace GameWPF
{
    class Wall
    {
        public Vector2 position;
        public int hp;
        public int scale;
        public Color wallColor;

        public Wall(Vector2 position)
        {
            this.position = position;
            hp = 1;
            scale = 50;
            wallColor = new Color(48, 48, 48);
        }
    }
}
