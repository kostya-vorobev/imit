using System;
using System.Windows.Controls;

namespace WpfApp4
{
    public class Herbivore : Cell
    {

        public Herbivore(int x, int y, int directionX, int directionY, int speed, int size)
            : base(x, y, directionX, directionY, speed, size)
        {
            IsHerbivore = true;
            maximalSize = 40;
        }

        public override Cell Divide()
        {
            Cell newCell;

            if (random.Next(0, 100) < 50 && Health < StandartHealth * 0.9)
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
            Health += food.Nutrition * 2;
            if (Size < maximalSize)
            {
                Size += 2;
            }
        }

        public new void PrintInfo(Label health, Label speed, Label className)
        {
            health.Content = Health.ToString();
            speed.Content = Speed.ToString();
            className.Content = "Травоядное";
        }
    }
}
