using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace GameWPF
{
    public abstract class WallProduct
    {
        public Vector2 position { get; protected set; }
        public int scale;
        public int hp;

        public WallProduct()
        {
            scale = 50;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }
    }

    public class WallBreakable : WallProduct
    {
        public WallBreakable(int hp) : base()
        {
            this.hp = hp;
        }
    }

    public class WallStrong : WallProduct
    {
        public WallStrong() : base()
        {
        }
    }

    /// <summary>
    /// Фабрика для создания объектов типа WallProduct.
    /// </summary>
    public class WallFactory
    {
        /// <summary>
        /// Возвращает объект WallProduct.
        /// </summary>
        /// <param name="num">0 - ломается(1hp) 1 - ломается(3 hp) 2 - не ломается</param>
        /// <returns></returns>
        public WallProduct CreateWall(int num)
        {
            WallProduct wall = null;
            switch(num)
            {
                case 1: wall = new WallBreakable(1);
                    break;
                case 2: wall = new WallBreakable(3);
                    break;
                case 3: wall = new WallStrong();
                    break;
            }
            return wall;
        }
    }
}
