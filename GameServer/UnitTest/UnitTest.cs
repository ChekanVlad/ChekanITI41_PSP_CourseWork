using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Input;
using SharpDXLib;
using System.Collections.Generic;
using GameWPF;
using Vector2 = SharpDX.Vector2;
using SharpDX.Direct2D1;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        /// <summary>
        /// Check collision with 2 players
        /// </summary>
        [TestMethod]
        public void CollisionTest()
        {
            Player player1 = new Player(new Vector2(0, 0), 1, 1);
            Player player2 = new Player(new Vector2(10, 10), 1, 1);
            Player player3 = new Player(new Vector2(80, 10), 1, 1);
            Assert.IsTrue(Renderer.IsColliding(player1.position, player2.position, player1.scale, player2.scale));
            Assert.IsFalse(Renderer.IsColliding(player1.position, player3.position, player3.scale, player3.scale));
            
        }

        /// <summary>
        /// test for readBMP (using map5.bmp in sprites folder)
        /// </summary>
        [TestMethod]
        public void ReadBmpTest()
        {
            int[,] readArray = Renderer.ReadBMP(5);
            int[,] actualArray = new int[readArray.GetLength(0),readArray.GetLength(1)];
            for(int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 12; j++)
                    actualArray[i, j] = j < 6 ? 0 : 1;
            }
            CollectionAssert.AreEqual(readArray, actualArray);
        }

        /// <summary>
        /// test for factory method
        /// </summary>
        [TestMethod]
        public void FactoryMethodTest()
        {
            WallFactory factory = new WallFactory();
            WallProduct wall1 = factory.CreateWall(0);
            WallProduct wall2 = factory.CreateWall(1);
            WallProduct wall3 = factory.CreateWall(2);
            Assert.IsInstanceOfType(wall1, typeof(WallBreakable));
            Assert.IsInstanceOfType(wall2, typeof(WallBreakable));
            Assert.IsInstanceOfType(wall3, typeof(WallStrong));
        }


        /// <summary>
        /// test for decorator
        /// </summary>
        [TestMethod]
        public void DecoratorTest()
        {
            Bomb bomb = new BombConcret(new Vector2(0,0), 10);
            int bombExp = bomb.GetExpRadius();
            bomb = new BombExpRadiusDecorator(bomb);
            Assert.IsInstanceOfType(bomb, typeof(BombExpRadiusDecorator));
            int bombExp2 = bomb.GetExpRadius();
            Assert.IsTrue(bombExp2 == bombExp + 104);
            
        }
    }
}
