using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameWPF
{
    public class BombExpRadiusDecorator : BombDecorator
    {
        public BombExpRadiusDecorator(Bomb bomb) : base(bomb.position, bomb.scale, bomb)
        {
        }

        public override int GetExpRadius()
        {
            return bomb.GetExpRadius() + 104;
        }
    }
}
