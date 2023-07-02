using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class Predator : Cell
    {
        public Predator(int x, int y, int directionX, int directionY, int speed, int size)
            : base(x, y, directionX, directionY, speed, size)
        {
            IsPredator = true;
        }

        protected new int HealthLossRate = 1;


        public new Cell Divide()
        {
            Cell newCell;

            // Шанс, что новая клетка станет травоядной или хищником
            if (random.Next(0, 2) == 0)
            {
                newCell = new Cell(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(Size - 5, Size + 5));
            }
            else
            {
                newCell = new Predator(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(Size - 5, Size + 5));

            }
            newCell.Health = Health / 2;
            Health = Health / 2;

            return newCell; 
        }

        public void UpdatePosition(List<Food> foods, int actualWidht, int actualHeight, List<Cell> cells)
        {
            IsPosition(actualWidht, actualHeight);

            // поиск ближайшей жертвы (клетки)
            double closestCellDistance = double.MaxValue;
            Cell closestCell = null;
            foreach (Cell cell in cells)
            {
                if (!cell.IsPredator)
                {
                    double distance = Math.Sqrt(Math.Pow(X - cell.X, 2) + Math.Pow(Y - cell.Y, 2));
                    if (distance < closestCellDistance && this.Size > cell.Size)
                    {
                        closestCellDistance = distance;
                        closestCell = cell;
                    }
                }
            }

            // перемещение к цели (еда или клетка)
            if (closestCell != null)
            {
                double angle = Math.Atan2((closestCell.Y - Y), (closestCell.X - X));
                DirectionX = (int)Math.Round(Math.Cos(angle) * Speed);
                DirectionY = (int)Math.Round(Math.Sin(angle) * Speed);
            }

            // потеря здоровья
            Health -= HealthLossRate;
        }

        public void Eat(Cell cell)
        {
            // При съедании другой клетки, хищник получает ее здоровье
            Health += cell.Health;
            cell.Health = 0;
            if (this.Size < maximalSize)
            {
                this.Size++;
            }
        }


    }
}
