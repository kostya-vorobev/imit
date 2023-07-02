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
using Syncfusion.SfSkinManager;

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

        }

        private void StartGame()
        {
            if (foods.Count == 0)
            {
                int countObjects = Convert.ToInt32(CointCellsTB.Text);
                // Создаем несколько случайных клеток
                for (int i = 0; i < countObjects; i++)
                {
                    Cell cell = new Cell(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(10, 50), random.Next(10, 50), random.Next(10, 20), random.Next(10, 50));
                    cells.Add(cell);
                }
                countObjects = Convert.ToInt32(CointFoodTB.Text);
                // Создаем несколько объектов пищи
                for (int i = 0; i < countObjects; i++)
                {
                    Food food = new Food(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(5, 20));
                    foods.Add(food);
                }
            }
            int speedTimer = Convert.ToInt32(SpeedTimerTB.Text);
            if (SpeedTimerTB.Text == null)
            {
                speedTimer = 50;
            }
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(speedTimer);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Обновление положения клеток
            foreach (Cell cell in cells)
            {
                if (cell.IsPredator)
                {
                    ((Predator)cell).UpdatePosition(foods, (int)canvas.ActualWidth, (int)canvas.ActualHeight, cells);
                }
                else
                {
                    cell.UpdatePosition(foods, (int)canvas.ActualWidth, (int)canvas.ActualHeight);
                }
            }

            for (int i = 0; i < foods.Count; i++)
            {
                for (int j = 0; j < cells.Count; j++)
                {
                    if (foods[i].Touches(cells[j]))
                    {
                        if (cells[j].IsHerbivore)
                        {
                            ((Herbivore)cells[j]).Eat(foods[i]);
                            foods.RemoveAt(i);
                            i--;
                            break;
                        }
                        else
                        {
                            cells[j].Eat(foods[i]);
                            foods.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsPredator == false && cells[i].Health < cells[i].StandartHealth / 5 && cells[i].IsHerbivore == false)
                {
                    for (int j = 0; j < cells.Count; j++)
                    {
                        if (cells[i].Touches(cells[j]) && i != j && !cells[j].IsPredator && !cells[j].IsHerbivore)
                        {
                            cells[i].Eat(cells[j]);
                            cells.RemoveAt(j);
                            if (j < i)
                                i--;
                            if (j > 0)
                            {
                                j--;
                            }


                        }
                    }
                }
            }

            // Удаление съеденных клеток
            for (int i = 0; i < cells.Count; i++)
            {
                for (int j = i + 1; j < cells.Count; j++)
                {
                    if (cells[i].Touches(cells[j]))
                    {
                        if (cells[i].IsPredator)
                        {
                            ((Predator)cells[i]).Eat(cells[j]);
                            cells.RemoveAt(j);
                            if (j > 0)
                                j--;
                        }
                        else if (cells[j].IsPredator)
                        {
                            ((Predator)cells[j]).Eat(cells[i]);
                            cells.RemoveAt(i);
                            if (i > 0)
                                i--;
                        }
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
                if (random.Next(0, 100) < 3)
                {
                    if (cells[i].IsHerbivore)
                    {
                        Cell newCell = ((Herbivore)cells[i]).Divide();
                        cells.Add(newCell);
                    }
                    else if (cells[i].IsPredator)
                    {
                        Cell newCell = ((Predator)cells[i]).Divide();
                        cells.Add(newCell);
                    }
                    else
                    {
                        Cell newCell = cells[i].Divide();
                        cells.Add(newCell);
                    }
                }
            }
            int countFood = 0;
            if (CointFoodTB.Text != "")
            {
                countFood = Convert.ToInt32(CointFoodTB.Text);
            }

            // Добавление новой еды
            if (foods.Count < countFood)
            {
                int countAddFood = random.Next(0, countFood - foods.Count);
                for (int i = 0; i < countAddFood / 4; i++)
                {
                    Food food = new Food(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(5, 20));
                    foods.Add(food);
                }
            }
                ConutCells();
            // Обновление отображения
            UpdateDisplay();
        }
        private void ConutCells()
        {
            int predatorCount = 0, herbivoreCount = 0, cellsCount = 0;
            if (cells.Count != 0)
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsPredator)
                {
                    predatorCount++;
                }else if(cells[i].IsHerbivore)
                {
                    herbivoreCount++;
                }else
                {
                    cellsCount++;
                }
                HerbivoreL.Content = herbivoreCount.ToString();
                PredatorL.Content = predatorCount.ToString();
                CellL.Content = cellsCount.ToString();
            }
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
                cellEllipse.Fill = cell.IsPredator ? Brushes.Red : cell.IsHerbivore ? Brushes.Yellow : Brushes.Blue;
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

        private void RestartB_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            cells.Clear();
            foods.Clear();
            int countObjects = Convert.ToInt32(CointCellsTB.Text);
            // Создаем несколько случайных клеток
            for (int i = 0; i < countObjects; i++)
            {
                Cell cell = new Cell(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(10, 50), random.Next(10, 50), random.Next(10, 20), random.Next(10, 50));
                cells.Add(cell);
            }
            countObjects = Convert.ToInt32(CointFoodTB.Text);
            // Создаем несколько объектов пищи
            for (int i = 0; i < countObjects; i++)
            {
                Food food = new Food(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(5, 20));
                foods.Add(food);
            }
        }

        private void CointFoodTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SpeedTimerTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SfSkinManager.ApplyStylesOnApplication = true;

        }
    }
}











