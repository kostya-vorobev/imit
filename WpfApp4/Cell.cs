using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp4
{
    public class Cell 
    {

        public  int X { get; set; }
        public  int Y { get; set; }
        public  int DirectionX { get; set; }
        public  int DirectionY { get; set; }
        public  int Speed { get; set; }
        public  int Size { get; set; }
        public  int Health { get; set; }
        public  int Strength { get; set; }
        public bool IsPredator { get; set; }
        public bool IsHerbivore { get; set; }
        protected Random random = new Random();
        protected int HealthLossRate = 1;
        protected int minimalSize = 10;
        protected int minimalSpeed = 10;
        public int StandartHealth  { get; }
        protected int maximalSize = 50;
        private int PredatorShanse = 0;
        private int HerbivoreShanse = 0;

        public Cell(int x, int y, int directionX, int directionY, int speed, int size)
        {
            X = x;
            Y = y;
            DirectionX = directionX;
            DirectionY = directionY;
            Speed = speed;
            if (speed > minimalSpeed)
                Speed = size;
            else Speed = minimalSpeed;
            if (size > minimalSize)
                Size = size;
            else Size = minimalSize;
            Health = 3000;
            Strength = 10;
            StandartHealth = Health;
        }
        public void IsPosition(int actualWidht, int actualHeight)
        {
            // новые кооржинаты клетки
            X += DirectionX;
            Y += DirectionY;

            // проверка выхода за границы
            if (X - Size / 2 < 10)
            {
                X = Size / 2;
                DirectionX = -DirectionX;
            }
            else if (X + Size / 2 > actualWidht)
            {
                X = actualWidht - Size / 2;
                DirectionX = -DirectionX;
            }
            if (Y - Size / 2 < 10)
            {
                Y = Size / 2;
                DirectionY = -DirectionY;
            }
            else if (Y + Size / 2 > actualHeight)
            {
                Y = actualHeight - Size / 2;
                DirectionY = -DirectionY;
            }
        }

        public void UpdatePosition(List<Food> foods, int actualWidht, int actualHeight)
        {
            IsPosition(actualWidht, actualHeight);

            // происк еды
            double closestDistance = double.MaxValue;
            Food closestFood = null;
            foreach (Food food in foods)
            {
                double distance = Math.Sqrt(Math.Pow(X - food.X, 2) + Math.Pow(Y - food.Y, 2));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
            }

            // перемещение к пище
            if (closestFood != null)
            {
                double angle = Math.Atan2((closestFood.Y - Y), (closestFood.X - X));
                DirectionX = (int)Math.Round(Math.Cos(angle) * Speed);
                DirectionY = (int)Math.Round(Math.Sin(angle) * Speed);
            }

            // потеря здоровья
            Health -= HealthLossRate;
        }
       
        public  bool Touches(Cell cell)
        {
            if (this.Size < cell.Size)
            {
                int distance = (int)Math.Sqrt(Math.Pow(X - cell.X, 2) + Math.Pow(Y - cell.Y, 2));
                return distance < (cell.Size + Size) / 2;
            }
            else
            {
                return false;
            }
        }
        public void Eat(Food food)
        {
            Health += food.Nutrition;
            if (this.Size < maximalSize)
            {
                this.Size++;
            }
            if (HerbivoreShanse < 45)
                HerbivoreShanse++;
        }

        public void Eat(Cell cell)
        {
            // При съедании другой клетки, хищник получает ее здоровье
            Health += cell.Health / 2;
            cell.Health = 0;
            if (this.Size < maximalSize)
            {
                this.Size++;
            }
            if (PredatorShanse < 60)
                PredatorShanse++;
        }

        public virtual Cell Divide()
        {
            Cell newCell;

            // Шанс, что новая клетка станет травоядной или хищником
            if (random.Next(0, 50) < HerbivoreShanse)
            {
                newCell = new Herbivore(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(minimalSize, Size+1));
                newCell.Health = Health;
                Health = Health / 2;
            }
            else if (random.Next(0, 100) < PredatorShanse)
            {
                newCell = new Predator(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed-1, Speed + 10), minimalSize+5);
                newCell.Health = Health;
                Health = Health / 2;
            }
            else
            {
                newCell = new Cell(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(Size - 5, Size + 5));
                newCell.Health = Health;
                Health = Health / 2;
            }



            return newCell;

        }

    }
}
