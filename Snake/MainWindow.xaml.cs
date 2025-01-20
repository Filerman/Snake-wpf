using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
{
    { GridValue.Empty, Images.Empty },
    { GridValue.Snake, Images.SnakeColors[Images.SelectedColor].Body },
    { GridValue.Food, Images.Food },
};


        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int snakeSpeed = 200; // Domyślna szybkość w milisekundach (dla Easy)


        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);

            // Inicjalizacja timera
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1); // Aktualizuje się co sekundę
            gameTimer.Tick += GameTimer_Tick; // Zdarzenie aktualizacji timera
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            TimerText.Text = $"TIME: {elapsedTime}s";
        }



        private async Task RunGame()
        {
            elapsedTime = 0; // Reset czasu
            UpdateTimerText();
            Draw();
            await ShowCountDown(); // Poczekaj na zakończenie odliczania 3, 2, 1
            Overlay.Visibility = Visibility.Hidden;
            gameTimer.Start(); // Uruchom timer po odliczaniu
            await GameLoop();
            gameTimer.Stop(); // Zatrzymaj timer po zakończeniu gry
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }



        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(snakeSpeed); // Użyj prędkości zależnej od poziomu trudności
                gameState.Move();
                Draw();
            }
        }


        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridValue = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridValue];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.SnakeColors[Images.SelectedColor].Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }


        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.SnakeColors[Images.SelectedColor].DeadHead : Images.SnakeColors[Images.SelectedColor].DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }


        private async Task ShowCountDown()
        {
            for (int i = 3; i > 0; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "Press any key to start";
        }

        private void ColorSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                Images.SelectedColor = selectedItem.Content.ToString();
                UpdateSnakeColors();
            }
        }

        private void UpdateSnakeColors()
        {
            gridValToImage[GridValue.Snake] = Images.SnakeColors[Images.SelectedColor].Body;
        }

        private DispatcherTimer gameTimer;
        private int elapsedTime; // W sekundach


        private void DifficultySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DifficultySelector.SelectedItem is ComboBoxItem selectedItem)
            {
                string difficulty = selectedItem.Content.ToString();
                switch (difficulty)
                {
                    case "Easy":
                        snakeSpeed = 200;
                        break;
                    case "Medium":
                        snakeSpeed = 150;
                        break;
                    case "Hard":
                        snakeSpeed = 100;
                        break;
                    case "Extreme":
                        snakeSpeed = 50;
                        break;
                }
            }
        }



    }
}