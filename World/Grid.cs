using System;

namespace AiGame1.World
{
    public class Grid
    {
        public int Width { get; }
        public int Height { get; }
        public Cell[,] Cells { get; }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width, height];
            CreateHardcodedMap();
        }

        private void CreateHardcodedMap()
        {
            // Create a border of walls and a floor inside
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                    {
                        Cells[x, y] = new Cell(TileType.Wall);
                    }
                    else
                    {
                        Cells[x, y] = new Cell(TileType.Floor);
                    }
                }
            }

            //Add a few random pillars
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                int x  = rnd.Next(1, Width);
                int y  = rnd.Next(1, Height);
                Cells[x, y] = new Cell(TileType.Wall);
            }

        }
    }
}
