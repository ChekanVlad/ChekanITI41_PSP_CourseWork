using SharpDX;
using SharpDX.Direct2D1;
using System.Windows.Input;
using SharpDXLib;
using System;
using System.Collections.Generic;
using Udp;

namespace GameWPF
{
    public class Renderer : Direct2DComponent
    {
        Bitmap[] pl1Sprites = new Bitmap[5];
        Bitmap[] pl2Sprites = new Bitmap[5];
        Bitmap[] wallsSprite = new Bitmap[3];
        Bitmap[] bombSprite = new Bitmap[2];
        Bitmap bg;
        private static int[] area = { 957, 559 };
        Player player1;
        Player player2;
        private Key[] keys1;
        private Key[] keys2;
        private Vector2 position1 = new Vector2(11, (int)area[1] / 2);
        private Vector2 position2 = new Vector2(area[0] - 10, (int)area[1] / 2);
        private Vector2 borders = new Vector2(area[0], area[1]);
        private List<WallProduct> walls;
        private WallFactory wallFactory;
        private bool IsDecorated = false;

        public Renderer(int[] bombs, int playerId, Client client)
        {
            if(playerId == 1)
            {
                player1 = new Player(position1, bombs[0], bombs[1]);
                player2 = new Player(position2, bombs[2], bombs[3]);
            }
            else
            {
                player1 = new Player(position2, bombs[0], bombs[1]);
                player2 = new Player(position1, bombs[2], bombs[3]);
            }
            
        }
        protected override void InternalInitialize()
        {
            base.InternalInitialize();                    
            LoadSprites();
            keys1 = new Key[6] { Key.W, Key.S, Key.A, Key.D, Key.F, Key.G };
            keys2 = new Key[6] { Key.Up, Key.Down, Key.Left, Key.Right, Key.K, Key.L };
            wallFactory = new WallFactory();
            GenerateWalls();
        }

        protected override void InternalUninitialize()
        {
            //Utilities.Dispose(ref color1);
            //Utilities.Dispose(ref color2);

            base.InternalUninitialize();
        }

        /// <summary>
        /// Рендер
        /// </summary>
        protected override void Render()
        {
            RenderTarget2D.Clear(new Color(200, 200, 200));
            RenderTarget2D.DrawBitmap(bg, new RectangleF(0, 0, 1000, 600), 1, BitmapInterpolationMode.NearestNeighbor);
                //RenderTarget2D.FillRectangle(new RectangleF(player1.position.X, player1.position.Y, player1.scale, player1.scale), new SolidColorBrush(RenderTarget2D, player1.color));
            //RenderTarget2D.FillRectangle(new RectangleF(player2.position.X, player2.position.Y, player2.scale, player2.scale), new SolidColorBrush(RenderTarget2D, player2.color));
            RenderTarget2D.DrawBitmap(pl1Sprites[player1.sIndex], new RectangleF(player1.position.X, player1.position.Y, player1.scale, player1.scale), 1, BitmapInterpolationMode.Linear);
            RenderTarget2D.DrawBitmap(pl2Sprites[player2.sIndex], new RectangleF(player2.position.X, player2.position.Y, player2.scale, player2.scale), 1, BitmapInterpolationMode.Linear);
            DrawWalls();
            UpdateAction();
        }

        /// <summary>
        /// Обновление перемещений игрока и бомб.
        /// </summary>
        private void UpdateAction()
        {
            UpdatePlayerPosition(player1, keys1);
            UpdatePlayerPosition(player2, keys2);
            BombPlanting();
        }
        
            /// <summary>
            /// Метод, проверяющий постановку бомб и взрывы.
            /// </summary>
        private void BombPlanting()
        {            
            BombPlayerPlanting(player1, keys1[4], keys1[5]);
            BombPlayerPlanting(player2, keys2[4], keys2[5]);
            BombIsExplode(player1, player2);
            BombIsExplode(player2, player1);
        }

        /// <summary>
        /// Устновка бомбы игроком.
        /// </summary>
        /// <param name="player">Игрок.</param>
        /// <param name="key1">Клавиша, устанавливающая бомбу.</param>
        /// <param name="key2">Клавиша, устанавливающая бомбу.</param>
        private void BombPlayerPlanting(Player player, Key key1, Key key2)
        {
            if (Keyboard.IsKeyDown(key1) && player.expTime == 0 && PlayersIsAlive() && player.smallBombCount != 0)
            {
                player.smallBombCount--;
                if(IsDecorated)
                {
                    IsDecorated = false;
                    player.bomb = new BombConcret(player.bomb.position, player.bomb.scale);
                }
                player.bomb.position = new Vector2(player.position.X + player.scale / 4, player.position.Y + player.scale / 4);
                player.expTime = 100;
                player.expTime = 100;
            }
            else
            {
                if (Keyboard.IsKeyDown(key2) && player.expTime == 0 && PlayersIsAlive() && player.bigBombCount != 0)
                {
                    player.bigBombCount--;
                    if(!IsDecorated)
                    {
                        IsDecorated = true;
                        player.bomb = new BombExpRadiusDecorator(player.bomb);
                    }                   
                    player.bomb.position = new Vector2(player.position.X + player.scale / 4, player.position.Y + player.scale / 4);
                    player.expTime = 100;
                    player.expTime = 100;
                }
            }
        }

        /// <summary>
        /// Коллизия.
        /// </summary>
        /// <param name="v1">Первый вектор (Положение фигуры по левому верхнему углу).</param>
        /// <param name="v2">Второй вектор (Положение фигуры по левому верхнему углу).</param>
        /// <param name="scale1">Размер первого квадрата.</param>
        /// <param name="scale2">Размер второго квадрата.</param>
        /// <returns></returns>
        public static bool IsColliding(Vector2 v1, Vector2 v2, int scale1, int scale2)
        {
             if (v1.X + scale1 >= v2.X &&     
                 v1.X <= v2.X + scale2 &&       
                 v1.Y + scale1 >= v2.Y &&
                 v1.Y <= v2.Y + scale2)
             {       
                 return true;
             } 

             return false;
        }

        /// <summary>
        /// Проверка коллизии игроков.
        /// </summary>
        /// <returns></returns>
        private bool IsCollidingPlayers()
        {
            return IsColliding(player1.position, player2.position, player1.scale, player2.scale);
        }

        /// <summary>
        /// Проверка коллизии игроков и стен.
        /// </summary>
        /// <param name="player"></param>
        private bool IsCollidingWalls(Player player)
        {
            foreach (WallProduct i in walls)
            {
                if (IsColliding(player.position, i.position, player.scale, i.scale))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка, живы ли игроки. Возращает true, если оба игрока живы.
        /// </summary>
        /// <returns></returns>
        private bool PlayersIsAlive()
        {
            return player1.isAlive && player2.isAlive; 
        }

        /// <summary>
        /// Отрисовка стен.
        /// </summary>
        private void DrawWalls()
        {
            foreach(WallProduct i in walls)
            {
                if (i is WallBreakable)
                {
                    if (i.hp > 1)
                    {
                        RenderTarget2D.DrawBitmap(wallsSprite[1], new RectangleF(i.position.X, i.position.Y, i.scale, i.scale), 1, BitmapInterpolationMode.Linear);
                    }
                    else
                    {
                        RenderTarget2D.DrawBitmap(wallsSprite[2], new RectangleF(i.position.X, i.position.Y, i.scale, i.scale), 1, BitmapInterpolationMode.Linear);
                    }
                }
                else
                {
                    RenderTarget2D.DrawBitmap(wallsSprite[0], new RectangleF(i.position.X, i.position.Y, i.scale, i.scale), 1, BitmapInterpolationMode.Linear);
                }
                //RenderTarget2D.FillRectangle(new RectangleF(i.position.X, i.position.Y, i.scale, i.scale), new SolidColorBrush(RenderTarget2D, i.wallColor));        
            }
        }

        /// <summary>
        /// Генерация стен.
        /// </summary>
        private void GenerateWalls()
        {
            walls = new List<WallProduct>();
            Random rand = new Random();          
            int[,] wallsPositions = ReadBMP(rand.Next(5));
            Vector2 position = new Vector2();
            for(int i = 0; i < wallsPositions.GetLength(0); i++)
            {
                for(int j = 0; j < wallsPositions.GetLength(1); j++)
                {
                    if (wallsPositions[i,j] == 1)
                    {
                        walls.Add(wallFactory.CreateWall(rand.Next(3)));
                        position.X = i * walls[walls.Count - 1].scale;
                        position.Y = j * walls[walls.Count - 1].scale;
                        walls[walls.Count - 1].SetPosition(position);
                    }               
                }
            }
        }

        /// <summary>
        /// Загрузка спрайтов
        /// </summary>
        private void LoadSprites()
        {
            bombSprite[0] = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\bomb\\bomb.png");
            bombSprite[1] = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\bomb\\explosion.png");
            pl1Sprites[4] = pl2Sprites[4] = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\rip.png");
            bg = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\bg.png");
            for (int i = 0; i < 4; i++)
            {
                pl1Sprites[i] = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\pl1\\pl1_" + (i + 1) + ".png");
                pl2Sprites[i] = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\pl2\\pl2_" + (i + 1) + ".png");
                if(i < 3)
                {
                    wallsSprite[i] = BitmapWorker.LoadFromFile(RenderTarget2D, "..\\..\\..\\sprites\\walls\\wall" + (i + 1) + ".jpg");
                }
                
            }
        }

        /// <summary>
        /// Чтение BMP-файла.
        /// </summary>
        public static int[,] ReadBMP(int num)
        {
            int[,] pixels;
            System.Drawing.Color[,] pixelColors;
            System.Drawing.Bitmap bmp;
            string filePath = "..\\..\\..\\sprites\\maps\\map" + num + ".bmp";
            bmp = new System.Drawing.Bitmap(filePath);
            pixelColors = new System.Drawing.Color[bmp.Width, bmp.Height];
            pixels = new int[bmp.Width, bmp.Height];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    pixelColors[x, y] = bmp.GetPixel(x, y);
                    pixels[x,y] = (pixelColors[x, y].R == 0) ? 0 : 1;
                }
            }
            return pixels;
        }

        /// <summary>
        /// Обновление позиции игрока.
        /// </summary>
        /// <param name="player">Игрок, для которого обновляется позиция.</param>
        /// <param name="keys">Управление игрока (клавиши).</param>
        private void UpdatePlayerPosition(Player player, Key[] keys)
        {
            if (player.isAlive)
            {
                if (Keyboard.IsKeyDown(keys[0]) && player.position.Y > 0)
                {
                    player.sIndex = 0;
                    player.position.Y -= player.speed;
                    if (IsCollidingPlayers() || IsCollidingWalls(player))
                        player.position.Y += player.speed;
                }

                if (Keyboard.IsKeyDown(keys[1]) && player.position.Y < borders.Y)
                {
                    player.sIndex = 1;
                    player.position.Y += player.speed;
                    if (IsCollidingPlayers() || IsCollidingWalls(player))
                        player.position.Y -= player.speed;
                }

                if (Keyboard.IsKeyDown(keys[2]) && player.position.X > 0)
                {
                    player.sIndex = 2;
                    player.position.X -= player.speed;
                    if (IsCollidingPlayers() || IsCollidingWalls(player))
                        player.position.X += player.speed;
                }

                if (Keyboard.IsKeyDown(keys[3]) && player.position.X < borders.X)
                {
                    player.sIndex = 3;
                    player.position.X += player.speed;
                    if (IsCollidingPlayers() || IsCollidingWalls(player))
                        player.position.X -= player.speed;
                }
            }
            else player.sIndex = 4;
        }

        /// <summary>
        /// Проверка на взрыв (стены и игроки).
        /// </summary>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        private void BombIsExplode(Player player, Player enemy)
        {
            if (player.expTime > 0)
            {
                RenderTarget2D.DrawBitmap(bombSprite[0], new RectangleF(player.bomb.position.X, player.bomb.position.Y, player.bomb.scale, player.bomb.scale), 1, BitmapInterpolationMode.Linear);
                if (player.expTime < 25)
                {
                    //RenderTarget2D.FillRectangle(new RectangleF(player.bomb.position.X - player.bomb.getExpRadius() / 2 + player.bomb.scale / 2, player.bomb.position.Y - player.bomb.getExpRadius() / 2 + player.bomb.scale / 2, player.bomb.getExpRadius(), player.bomb.getExpRadius()), expColor);
                    RenderTarget2D.DrawBitmap(bombSprite[1], new RectangleF(player.bomb.position.X - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2, player.bomb.position.Y - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2, player.bomb.GetExpRadius(), player.bomb.GetExpRadius()), 1, BitmapInterpolationMode.Linear);
                    if (player.isAlive && IsColliding(player.position, new Vector2(player.bomb.position.X - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2, player.bomb.position.Y - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2), player.scale, player.bomb.GetExpRadius()))
                    {
                        player.isAlive = false;
                    }
                    if (enemy.isAlive && IsColliding(enemy.position, new Vector2(player.bomb.position.X - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2, player.bomb.position.Y - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2), enemy.scale, player.bomb.GetExpRadius()))
                    {
                        enemy.isAlive = false;
                    }
                }
                //RenderTarget2D.FillRectangle(new RectangleF(player.bomb.position.X, player.bomb.position.Y, player.bomb.scale, player.bomb.scale), new SolidColorBrush(RenderTarget2D, Bomb.bombColor));               
                player.expTime--;

                if (player.expTime == 24)
                {
                    for (int i = 0; i < walls.Count; i++)
                    {
                        if (IsColliding(walls[i].position, new Vector2(player.bomb.position.X - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2, player.bomb.position.Y - player.bomb.GetExpRadius() / 2 + player.bomb.scale / 2), walls[i].scale, player.bomb.GetExpRadius()))
                        {
                            if (walls[i] is WallBreakable)
                            {
                                walls[i].hp--;
                                if (walls[i].hp == 0)
                                {
                                    walls.Remove(walls[i]);
                                    i--;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}