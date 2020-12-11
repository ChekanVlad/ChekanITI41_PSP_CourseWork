using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GameWPF
{

    public class BombConcret : Bomb
    {
        public BombConcret(Vector2 position, int scale) : base(position, scale)
        {
        }

        public override int GetExpRadius()
        {
            return expRadius;
        }
    }
}
