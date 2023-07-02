using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class Herbivore : Cell
    {

        public Herbivore(int x, int y, int directionX, int directionY, int speed, int size) : base(x, y, directionX, directionY, speed, size)
        {
            IsHerbivore = true;
        }

        public new Cell Divide()
        {
            Cell newCell;

            // Шанс, что новая клетка станет травоядной или хищником
            if (random.Next(0, 3) != 0)
            {
                newCell = new Cell(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(Size - 5, Size + 5));
            }
            else
            {
                newCell = new Herbivore(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(Size - 5, Size + 5));

            }
            newCell.Health = Health / 2;
            Health = Health / 2;

            return newCell;
        }

        public new void Eat(Food food)
        {
            Health += food.Nutrition*2;
            if (this.Size < maximalSize)
            {
                this.Size+=2;
            }
        }

    }
}
