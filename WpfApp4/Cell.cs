using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp4
{
    public class Cell
    {
        protected int x;
        protected int y;
        protected int directionX;
        protected int directionY;
        protected int speed;
        protected int size;
        protected int health;
        protected int strength;
        protected bool isPredator;
        protected bool isHerbivore;
        protected int standartHealth;
        protected Random random = new Random();
        protected int healthLossRate = 10;
        protected int minimalSize = 10;
        protected int minimalSpeed = 10;
        protected int maximalSize = 50;
        private int predatorСhanсe = 0;
        private int herbivoreСhanсe = 0;


        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int DirectionX { get => directionX; set => directionX = value; }
        public int DirectionY { get => directionY; set => directionY = value; }
        public int Speed { get => speed; set => speed = value; }
        public int Size { get => size; set => size = value; }
        public int Health { get => health; set => health = value; }
        public int Strength { get => strength; set => strength = value; }
        public bool IsPredator { get => isPredator; set => isPredator = value; }
        public bool IsHerbivore { get => isHerbivore; set => isHerbivore = value; }
        public int StandartHealth { get => standartHealth;}



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
            Health = 500;
            Strength = 10;
            this.standartHealth = Health;
        }
        protected void IsPosition(int actualWidht, int actualHeight)
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
            Health -= this.healthLossRate;
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
            if (Health < StandartHealth)
            Health += food.Nutrition;
            if (this.Size < maximalSize)
            {
                this.Size++;
            }
            if (herbivoreСhanсe < 30)
                herbivoreСhanсe++;
            if (predatorСhanсe > 0)
                predatorСhanсe--;
        }
        public void PrintInfo(Label health, Label speed, Label className)
        {
            health.Content = this.Health.ToString();
            speed.Content = this.Speed.ToString();
            className.Content = "Всеядное";
        }
        public void Eat(Cell cell)
        {
            // При съедании другой клетки, хищник получает ее здоровье
            if (Health < StandartHealth)
                Health += cell.Health / 2;
            cell.Health = 0;
            if (this.Size < maximalSize)
            {
                this.Size++;
            }
            if (predatorСhanсe < 60)
                predatorСhanсe+=10;
            if (herbivoreСhanсe > 0)
                herbivoreСhanсe--;
        }

        public virtual Cell Divide()
        {
            Cell newCell;

            // Шанс, что новая клетка станет травоядной или хищником
            if (random.Next(0, 100) < herbivoreСhanсe)
            {
                newCell = new Herbivore(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed - 5, Speed + 5), random.Next(minimalSize, Size+1));
                newCell.Health = Health / 2;
                Health = Health / 2;
            }
            else if (random.Next(0, 100) < predatorСhanсe)
            {
                newCell = new Predator(X + Size, Y + Size, DirectionX, DirectionY, random.Next(Speed-1, Speed + 10), minimalSize+5);
                newCell.Health = Health/2;
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
