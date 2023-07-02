using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class Food
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Nutrition { get; set; }
        public int Size { get; set; }

        public Food(int x, int y, int size)
        {
            X = x;
            Y = y;
            Size = size;
            Nutrition = size*2;
        }

        public bool Touches(Cell cell)
        {
            if (cell.IsPredator == false)
            {
                int distance = (int)Math.Sqrt(Math.Pow(X - cell.X, 2) + Math.Pow(Y - cell.Y, 2));
                return distance < (cell.Size + Size) / 2;
            }else
            {
                return false;
            }
        }
    }
}
