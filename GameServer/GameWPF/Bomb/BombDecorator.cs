using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameWPF
{
    public abstract class BombDecorator : Bomb
    {
        protected Bomb bomb;
        public BombDecorator(Vector2 position, int scale, Bomb bomb) : base(position, scale)
        {
            this.bomb = bomb;
        }
    }
}