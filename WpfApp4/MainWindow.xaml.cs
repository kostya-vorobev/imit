using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Cell> cells = new List<Cell>();
        private List<Food> foods = new List<Food>(); 
        private Random random = new Random(); 
        private DispatcherTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();

            // Создаем несколько случайных клеток
            for (int i = 0; i < 10; i++)
            {
                Cell cell = new Cell(random.Next(10, 50), random.Next(10, 50), random.Next(10, 50), random.Next(10, 50), random.Next(10, 50), random.Next(10, 50));
                cells.Add(cell);
            }

            // Создаем несколько объектов пищи
            for (int i = 0; i < 50; i++)
            {
                Food food = new Food(random.Next(0, (int)canvas.ActualWidth), random.Next(0, (int)canvas.ActualHeight), random.Next(5, 20));
                foods.Add(food);
            }
        }

        private void StartGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(50);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Обновление положения клеток
            foreach (Cell cell in cells)
            {
                cell.UpdatePosition(foods, (int)canvas.ActualWidth, (int)canvas.ActualHeight);
            }

            // Проверка столкновений пищи и клеток
            for (int i = 0; i < foods.Count; i++)
            {
                for (int j = 0; j < cells.Count; j++)
                {
                    if (foods[i].Touches(cells[j]))
                    {
                        cells[j].Eat(foods[i]);
                        foods.RemoveAt(i);
                        if (i > 0)
                            i--;
                    }
                }
            }

            // Удаление мертвых клеток
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].Health <= 0)
                {
                    cells.RemoveAt(i);
                    i--;
                }
            }


            // Добавление новых клеток
            for (int i = 0; i < cells.Count; i++)
            {
                if (random.Next(0, 100) < 5)
                {
                    Cell newCell = cells[i].Divide();
                    cells.Add(newCell);
                }
            }

            // Добавление новой еды
            if (foods.Count < 100)
            {
                Food food = new Food(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(5, 20));
                foods.Add(food);
            }

            // Обновление отображения
            UpdateDisplay();
        }

        private void StopGame()
        {
            gameTimer.Stop();
        }

        private void UpdateDisplay()
        {
            // Очистка отображения
            canvas.Children.Clear();

            // Отображение объектов пищи
            foreach (Food food in foods)
            {
                Rectangle foodRect = new Rectangle();
                foodRect.Width = food.Size;
                foodRect.Height = food.Size;
                foodRect.Fill = Brushes.Green;
                Canvas.SetLeft(foodRect, food.X - food.Size / 2);
                Canvas.SetTop(foodRect, food.Y - food.Size / 2);
                canvas.Children.Add(foodRect);
            }

            // Отображение клеток
            foreach (Cell cell in cells)
            {
                Ellipse cellEllipse = new Ellipse();
                cellEllipse.Width = cell.Size;
                cellEllipse.Height = cell.Size;
                cellEllipse.Fill = cell.IsCarnivore ? Brushes.Red : cell.IsHerbivore ? Brushes.Yellow : Brushes.Blue;
                Canvas.SetLeft(cellEllipse, cell.X - cell.Size / 2);
                Canvas.SetTop(cellEllipse, cell.Y - cell.Size / 2);
                canvas.Children.Add(cellEllipse);
            }
        }

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            // Запускаем игру
            StartGame();
        }

        private void StopB_Click(object sender, RoutedEventArgs e)
        {
            // Останавливаем игру
            StopGame();
        }
    }
}


public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public int DirectionX { get; set; }
    public int DirectionY { get; set; }
    public int Speed { get; set; }
    public int Size { get; set; }
    public int Health { get; set; }
    public int Strength { get; set; }
    public bool IsCarnivore { get; set; }
    public bool IsHerbivore { get; set; }
    public Color Color { get; set; }
    private int foodCounter = 0; private const int HealthLossRate = 1; private Random random = new Random();

    public Cell(int x, int y, int directionX, int directionY, int speed, int size, Color color)
    {
        X = x;
        Y = y;
        DirectionX = directionX;
        DirectionY = directionY;
        Speed = speed;
        Size = size;
        Color = color;
        Health = 100;
        Strength = 10;
        IsCarnivore = false;
        IsHerbivore = false;
    }

    public void UpdatePosition(List<Food> foods, int actualWidth, int actualHeight)
    {
        // Move the cell
        X += DirectionX;
        Y += DirectionY;

        // Check if the cell is going outside of the canvas
        if (X - Size / 2 < 10)
        {
            X = Size / 2;
            DirectionX = -DirectionX;
        }
        else if (X + Size / 2 > actualWidth)
        {
            X = actualWidth - Size / 2;
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

        // Find the closest food
        double closestDistance = double.MaxValue;
        Food closestFood = null;
        foreach (Food food in foods)
        {
            double distance = Math.Sqrt(Math.Pow(X - food.X, 2) + Math.Pow(Y - food.Y, 2));
            if (distance < closestDistance)
            {
                if (IsCarnivore && food is Cell && ((Cell)food).IsHerbivore)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
                else if (!IsCarnivore)
                {
                    closestDistance = distance;
                    closestFood = food;
                }
            }
        }

        // Move towards the closest food
        if (closestFood != null)
        {
            double angle = Math.Atan2((closestFood.Y - Y), (closestFood.X - X));
            DirectionX = (int)Math.Round(Math.Cos(angle) * Speed);
            DirectionY = (int)Math.Round(Math.Sin(angle) * Speed);
        }

        // Lose health over time
        Health -= HealthLossRate;

        if (foodCounter >= 5)
        {
            Divide();
            foodCounter = 0;
        }
    }

    public void Eat(Food food)
    {
        Health += food.Nutrition;
        foodCounter++;
        if (food is Cell)
        {
            Cell cell = (Cell)food;
            if (cell.IsHerbivore && !IsCarnivore)
            {
                IsHerbivore = true;
            }
            else if (cell.IsCarnivore && !IsHerbivore)
            {
                IsCarnivore = true;
            }
        }
    }

    public Cell Divide()
    {
        Cell newCell = new Cell(X, Y, DirectionX, DirectionY, Speed, Size, MutateColor(Color));
        newCell.Health = Health / 2;
        Health = Health / 2;
        newCell.Speed = Speed;
        newCell.Size = Size;
        newCell.Health = Health;
        newCell.Strength = Strength;
        Mutate(newCell);
        return newCell;
    }

    private void Mutate(Cell cell)
    {
        int mutationChance = random.Next(0, 101);
        if (mutationChance >= 95)
        {
            cell.Speed += random.Next(-1, 2);
        }
        mutationChance = random.Next(0, 101);
        if (mutationChance >= 95)
        {
            cell.Size += random.Next(-1, 2);
        }
        mutationChance = random.Next(0, 101);
        if (mutationChance >= 95)
        {
            cell.Health += random.Next(-1, 2);
        }
        mutationChance = random.Next(0, 101);
        if (mutationChance >= 95)
        {
            cell.Strength += random.Next(-1, 2);
        }
    }

    private Color MutateColor(Color color)
    {
        int mutationChance = random.Next(0, 101);
        if (mutationChance >= 95)
        {
            int red = color.R + random.Next(-20, 20);
            int green = color.G + random.Next(-20, 20);
            int blue = color.B + random.Next(-20, 20);
            return Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
        }
        return color;
    }
}

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
        Nutrition = size;
    }

    public bool Touches(Cell cell)
    {
        int distance = (int)Math.Sqrt(Math.Pow(X - cell.X, 2) + Math.Pow(Y - cell.Y, 2));
        return distance < (cell.Size + Size) / 2;
    }
}

