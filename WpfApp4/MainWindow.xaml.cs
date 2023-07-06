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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;


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
        private ChartValues<ObservablePoint> herbivoreValues;
        private ChartValues<ObservablePoint> predatorValues;
        private ChartValues<ObservablePoint> cellValues;
        private int countTimer = 0;
        private SeriesCollection seriesCollection;

        public MainWindow()
        {

            InitializeComponent();
            herbivoreValues = new ChartValues<ObservablePoint>();
            predatorValues = new ChartValues<ObservablePoint>();
            cellValues = new ChartValues<ObservablePoint>();

            // Создаем серии и добавляем их в seriesCollection

            var cellSeries = new LineSeries
            {
                Title = "Всеядные",
                Values = cellValues,
                Stroke = Brushes.Blue
            };
            var predatorSeries = new LineSeries
            {
                Title = "Хищники",
                Values = predatorValues,
                Stroke = Brushes.Red
            };
            var herbivoreSeries = new LineSeries
            {
                Title = "Травоядные",
                Values = herbivoreValues,
                Stroke = Brushes.Yellow
            };


            seriesCollection = new SeriesCollection
    {
        herbivoreSeries,
        predatorSeries,
        cellSeries
    };

            // Устанавливаем seriesCollection как источник данных для графика
            chart.Series = seriesCollection;
        }

        private void StartGame()
        {
            if (foods.Count == 0 && cells.Count == 0)
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
                    Food food = new Food(random.Next(10, (int)canvas.ActualWidth-10), random.Next(10, (int)canvas.ActualHeight-10), random.Next(5, 20));
                    foods.Add(food);
                }
            }


        }
        private void StartTimer()
        {


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
            int maxCountCells = 0;
            if (CointFoodTB.Text != "")
            {
                maxCountCells = Convert.ToInt32(MaxCountCellsTB.Text);
            }

            if (cells.Count < maxCountCells)
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
                    Food food = new Food(random.Next(10, (int)canvas.ActualWidth - 10), random.Next(10, (int)canvas.ActualHeight - 10), random.Next(5, 20));
                    foods.Add(food);
                }
            }
            if (countTimer % 10 == 0)
                ConutCells();
            // Обновление отображения
            UpdateDisplay();
            if (countTimer % 10 == 0)
                UpdateChart();
            countTimer++;
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

            }
            HerbivoreL.Content = herbivoreCount.ToString();
            PredatorL.Content = predatorCount.ToString();
            CellL.Content = cellsCount.ToString();
        }
        private void UpdateChart()
        {
            if (seriesCollection == null)
            {
                return;
            }

            // Проверяем наличие серий в коллекции
            if (seriesCollection.Count < 3)
            {
                return;
            }

            // Проверяем наличие значений в каждой серии
            if (seriesCollection[0].Values == null || seriesCollection[1].Values == null || seriesCollection[2].Values == null)
            {
                return;
            }

            seriesCollection[0].Values.Add(new ObservablePoint((double)countTimer, Convert.ToDouble(HerbivoreL.Content)));
            seriesCollection[1].Values.Add(new ObservablePoint((double)countTimer, Convert.ToDouble(PredatorL.Content)));
            seriesCollection[2].Values.Add(new ObservablePoint((double)countTimer, Convert.ToDouble(CellL.Content)));

            // Ограничиваем количество точек в графике, если нужно
            const int maxDataPoints = 15; // Максимальное количество точек в графике
            if (seriesCollection[0].Values.Count > maxDataPoints)
            {
                // Удаляем старые точки, чтобы оставить только последние maxDataPoints точек
                foreach (var series in seriesCollection)
                {
                    while (series.Values.Count > maxDataPoints)
                    {
                        series.Values.RemoveAt(0);
                    }
                }
            }
        }



        private void StopGame()
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
            }

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
                cellEllipse.MouseLeftButtonDown += CellEllipse_MouseLeftButtonDown;
                Canvas.SetLeft(cellEllipse, cell.X - cell.Size / 2);
                Canvas.SetTop(cellEllipse, cell.Y - cell.Size / 2);
                canvas.Children.Add(cellEllipse);
            }
        }

        private void CellEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Получаем нажатый элемент (ellipse)
            Ellipse cellEllipse = (Ellipse)sender;

            // Ищем клетку, соответствующую нажатому элементу
            Cell selectedCell = cells.FirstOrDefault(cell =>
                Canvas.GetLeft(cellEllipse) == cell.X - cell.Size / 2 &&
                Canvas.GetTop(cellEllipse) == cell.Y - cell.Size / 2);

            // Если найдена клетка, выводим информацию о ней
            if (selectedCell != null)
            {
                if (selectedCell.IsHerbivore)
                {
                    ((Herbivore)selectedCell).PrintInfo(HealthL, SpeedL, ClassName);

                }
                else if (selectedCell.IsPredator)
                {
                    ((Predator)selectedCell).PrintInfo(HealthL, SpeedL, ClassName);

                }
                else
                    selectedCell.PrintInfo(HealthL, SpeedL, ClassName);
            }
        }

        private void StartB_Click(object sender, RoutedEventArgs e)
        {
            // Запускаем игру
            StartGame();
            StartTimer();
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

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void AddCellB_Click(object sender, RoutedEventArgs e)
        {

            StopGame();
            int countObjects = Convert.ToInt32(CointCellsTB.Text);
            // Создаем несколько случайных клеток
            for (int i = 0; i < 10; i++)
            {
                Cell cell = new Cell(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(10, 50), random.Next(10, 50), random.Next(10, 20), random.Next(10, 50));
                cells.Add(cell);
            }
            StartTimer();

        }

        private void AddPredatorB_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
            for (int i = 0; i < 10; i++)
            {
                Predator predator = new Predator(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(10, 50), random.Next(10, 50), random.Next(10, 20), random.Next(10, 50));
                cells.Add(predator);
            }
            StartTimer();
        }

        private void AddHerbivoreB_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
            for (int i = 0; i < 10; i++)
            {
                Herbivore herbivore = new Herbivore(random.Next(10, (int)canvas.ActualWidth), random.Next(10, (int)canvas.ActualHeight), random.Next(10, 50), random.Next(10, 50), random.Next(10, 20), random.Next(10, 50));
                cells.Add(herbivore);
            }
            StartTimer();
        }

        private void DeleteCellB_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsPredator == false && cells[i].IsHerbivore == false)
                {
                    cells.RemoveAt(i);
                    if (i > 0)
                        i--;
                }
            }
            StartTimer();
        }

        private void DeletePredatorB_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsPredator)
                {
                    cells.RemoveAt(i);
                    if (i > 0)
                        i--;
                }
            }
            StartTimer();
        }

        private void DeleteHerbivoreB_Click(object sender, RoutedEventArgs e)
        {
            StopGame();
            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].IsHerbivore)
                {
                    cells.RemoveAt(i);
                    if (i > 0)
                        i--;
                }
            }
            StartTimer();
        }
    }
}
