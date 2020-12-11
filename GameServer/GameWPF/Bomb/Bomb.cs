using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace GameWPF
{
    public abstract class Bomb
    {
        public Vector2 position;
        public int expRadius;
        public int scale;

        /// <summary>
        /// Объект Bomb.
        /// </summary>
        /// <param name="position">Позиция бомбы.</param>
        /// <param name="scale">Размер бомбы.</param>
        public Bomb(Vector2 position, int scale)
        {
            this.position = position;
            this.scale = scale;
            expRadius = 150;           
        }

        /// <summary>
        /// Возвращает радиус взрыва бомбы.
        /// </summary>
        /// <returns></returns>
        abstract public int GetExpRadius();
    }
}
