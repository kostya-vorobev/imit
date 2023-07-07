using System;

namespace WpfApp4
{
    public class Food
    {
        private int x;
        private int y;
        private int nutrition;
        private int size;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Size { get => size; set => size = value; }
        public int Nutrition { get => nutrition; set => nutrition = value; }

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
