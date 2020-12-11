using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace GameWPF
{
    public class Player
    {
        public Vector2 position;
        public int scale;
        public int expTime;
        public Bomb bomb;
        public bool isAlive;
        public int speed;
        public int smallBombCount;
        public int bigBombCount;
        public int sIndex;

        /// <summary>
        ///  Конструктор
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="smallBombCount"></param>
        /// <param name="bigBombCount"></param>
        public Player(Vector2 position, int smallBombCount, int bigBombCount)
        {
            this.position = position;
            this.smallBombCount = smallBombCount;
            this.bigBombCount = bigBombCount;
            scale = 40;
            expTime = 0;
            bomb = new BombConcret(new Vector2(0,0), (int)(scale / 2.0));
            isAlive = true;
            speed = 3;
            sIndex = 1;
        }
    }
}
