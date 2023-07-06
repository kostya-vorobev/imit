using System;

namespace WpfApp4
{
    public class Food
    {
        public int X { get; }
        public int Y { get; }
        public int Nutrition { get; }
        public int Size { get; }

        public Food(int x, int y, int size)
        {
            X = x;
            Y = y;
            Size = size;
            Nutrition = size * 2;
        }

        public bool Touches(Cell cell)
        {
            if (!cell.IsPredator)
            {
                int distance = (int)Math.Sqrt(Math.Pow(X - cell.X, 2) + Math.Pow(Y - cell.Y, 2));
                return distance < (cell.Size + Size) / 2;
            }
            else
            {
                return false;
            }
        }
    }
}
