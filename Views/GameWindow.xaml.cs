using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TheGameOfLife;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TheGameOfLife.Views
{
    public partial class GameWindow : Window
    {
        private static int Rows { get; set; }
        private static int Columns { get; set; }
        private Rectangle[,] cells;
        private bool autoplay = false;
        private DispatcherTimer timer;
        private bool[,] cellStates;
        private Stack<GameState> gameHistory = new Stack<GameState>();
        private int cellsDiedNumber = 0;
        private int cellsBornNumber = 0;
        private bool fromFile = false;
        bool[,] cellsDied = new bool[Rows, Columns];
        bool[,] cellsBorn = new bool[Rows, Columns];

        public GameWindow(int rows, int cols)
        {
            InitializeComponent();

            Rows = rows;
            Columns = cols;

            InitializeGameBoard();
        }

        public GameWindow(string filePath)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                LoadGameFromFile(filePath);
            }
            else
            {
                MessageBox.Show("Selected file is not valid.");
            }
        }

        private void LoadGameFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length >= 2)
                {
                    if (int.TryParse(lines[0], out int loadedRows) && int.TryParse(lines[1], out int loadedCols))
                    {
                        Rows = loadedRows;
                        Columns = loadedCols;
                        fromFile = true;
                        if (lines.Length > Rows + 4) 
                        {
                            UndoNextState.IsEnabled = true;
                        }
                        InitializeGameBoard();

                        int lineIndex = 2;

                        if (lines.Length >= lineIndex + Rows)
                        {
                            for (int row = 0; row < Rows; row++)
                            {
                                if (lines[lineIndex].Length == Columns)
                                {
                                    for (int col = 0; col < Columns; col++)
                                    {
                                        cellStates[row, col] = lines[lineIndex][col] == '1';
                                        cells[row, col].Fill = cellStates[row, col] ? Brushes.Black : Brushes.White;
                                    }
                                    lineIndex++;
                                }
                                else
                                {
                                    MessageBox.Show("Invalid format in the loaded file.");
                                    return;
                                }
                            }

                            if (lines.Length > lineIndex)
                            {
                                cellsDiedNumber = int.Parse(lines[lineIndex]);
                                lineIndex++;
                                cellsBornNumber = int.Parse(lines[lineIndex]);
                                lineIndex++;
                                CellsDiedLabel.Content = cellsDiedNumber;
                                CellsBornLabel.Content = cellsBornNumber;
                            }
                            else
                            {
                                MessageBox.Show("Not enough data in the loaded file.");
                            }
                            Stack<GameState> reversedStack = new Stack<GameState>();
                            while (lineIndex + Rows + 2 <= lines.Length)
                            {
                                GameState gameState = new GameState(new bool[Rows, Columns], 0, 0);

                                for (int row = 0; row < Rows; row++)
                                {
                                    if (lines[lineIndex].Length == Columns)
                                    {
                                        for (int col = 0; col < Columns; col++)
                                        {
                                            gameState.CellStates[row, col] = lines[lineIndex][col] == '1';
                                        }
                                        lineIndex++;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Invalid format in the loaded file.");
                                        return;
                                    }
                                }

                                gameState.CellsDiedNumber = int.Parse(lines[lineIndex]);
                                gameState.CellsBornNumber = int.Parse(lines[lineIndex + 1]);

                                reversedStack.Push(gameState);

                                lineIndex += 2; 
                            }

                            while (reversedStack.Count > 0)
                            {
                                gameHistory.Push(reversedStack.Pop());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Not enough data in the loaded file.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid row and column values in the loaded file.");
                    }
                }
                else
                {
                    MessageBox.Show("Not enough data in the loaded file.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading the game from file: " + ex.Message);
            }
        }

        private void InitializeGameBoard()
        {
            cells = new Rectangle[Rows, Columns];
            cellStates = new bool[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var cell = new Rectangle
                    {
                        Width = 20, 
                        Height = 20, 
                        Fill = Brushes.White, 
                        Stroke = Brushes.Black 
                    };

                    Canvas.SetLeft(cell, col * 20);
                    Canvas.SetTop(cell, row * 20);

                    GameBoard.Children.Add(cell);
                    if (!fromFile)
                    {
                        cell.MouseLeftButtonDown += Cell_MouseDown;
                    }
                    cells[row, col] = cell;
                }
            }

            timer = new DispatcherTimer();
            timer.Tick += (sender, e) => UpdateGame();
            timer.Interval = TimeSpan.FromMilliseconds(500);

        }

        private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!autoplay)
            {
                var cell = (Rectangle)sender;
                int row = (int)Canvas.GetTop(cell) / 20;
                int col = (int)Canvas.GetLeft(cell) / 20;

                cellStates[row, col] = !cellStates[row, col];
                cell.Fill = cellStates[row, col] ? Brushes.Black : Brushes.White;
            }
        }

        private void UpdateGame()
        {
            gameHistory.Push(new GameState(cellStates, cellsDiedNumber, cellsBornNumber));

            bool[,] nextGeneration = new bool[Rows, Columns];
            bool[,] currentCellsDied = new bool[Rows, Columns];
            bool[,] currentCellsBorn = new bool[Rows, Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int liveNeighbors = GetLiveNeighbors(row, col);

                    bool isAlive = cellStates[row, col];
                    bool willBeAlive = isAlive;

                    if (isAlive)
                    {
                        willBeAlive = liveNeighbors == 2 || liveNeighbors == 3;
                    }
                    else
                    {
                        willBeAlive = liveNeighbors == 3;
                    }

                    if (isAlive && !willBeAlive)
                    {
                        cellsDiedNumber++;
                        currentCellsDied[row, col] = true;
                    }
                    else if (!isAlive && willBeAlive)
                    {
                        cellsBornNumber++;
                        currentCellsBorn[row, col] = true;
                    }

                    nextGeneration[row, col] = willBeAlive;
                }
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    cellStates[row, col] = nextGeneration[row, col];
                    cells[row, col].Fill = cellStates[row, col] ? Brushes.Black : Brushes.White;
                }
            }

            GenerationLabel.Content = gameHistory.Count + 1;
            CellsDiedLabel.Content = cellsDiedNumber;
            CellsBornLabel.Content = cellsBornNumber;
            cellsDied = currentCellsDied;
            cellsBorn = currentCellsBorn;

        }

        private int GetLiveNeighbors(int row, int col)
        {
            int liveNeighbors = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;

                    int neighborRow = row + i;
                    int neighborCol = col + j;

                    if (neighborRow >= 0 && neighborRow < Rows && neighborCol >= 0 && neighborCol < Columns)
                    {
                        if (cellStates[neighborRow, neighborCol])
                            liveNeighbors++;
                    }
                }
            }

            return liveNeighbors;
        }

        private void Autoplay_Click(object sender, RoutedEventArgs e)
        {
            autoplay = true;
            timer.Start();
            BornCells.IsEnabled = true;
            DeadCells.IsEnabled = true;
            UndoNextState.IsEnabled = true;
            RestartGame.IsEnabled = true;
            Autoplay.Visibility = Visibility.Collapsed;
            AutoplayStop.Visibility = Visibility.Visible;
        }

        private void AutoplayStop_Click(object sender, RoutedEventArgs e)
        {
            autoplay = false;
            timer.Stop();
            AutoplayStop.Visibility = Visibility.Collapsed;
            Autoplay.Visibility = Visibility.Visible;
        }

        private void NextState_Click(object sender, RoutedEventArgs e)
        {
            if (!autoplay)
            {
                UpdateGame();
                BornCells.IsEnabled = true;
                DeadCells.IsEnabled = true;
                UndoNextState.IsEnabled = true;
                RestartGame.IsEnabled = true;
            }
        }

        private void UndoNextState_Click(object sender, RoutedEventArgs e)
        {
            if (!autoplay && gameHistory.Count > 0)
            {
                GameState previousState = gameHistory.Peek();
                gameHistory.Pop();

                cellStates = previousState.CellStates;
                CellsDiedLabel.Content = previousState.CellsDiedNumber;
                CellsBornLabel.Content = previousState.CellsBornNumber;
                cellsDiedNumber = previousState.CellsDiedNumber;
                cellsBornNumber = previousState.CellsBornNumber;

                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Columns; col++)
                    {
                        cells[row, col].Fill = cellStates[row, col] ? Brushes.Black : Brushes.White;
                    }
                }

                GenerationLabel.Content = gameHistory.Count + 1;

                if (gameHistory.Count == 0)
                {
                    UndoNextState.IsEnabled = false;
                    BornCells.IsEnabled = false;
                    DeadCells.IsEnabled = false;
                    RestartGame.IsEnabled = false;
                }
            }
        }

        private void ShowBornCells_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (cellsBorn[row, col])
                    {
                        cells[row, col].Fill = Brushes.Green;
                    }
                }
            }
            BornCells.Visibility = Visibility.Collapsed;
            HideBornCells.Visibility = Visibility.Visible;
        }

        private void HideBornCells_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (cellsBorn[row, col])
                    {
                        cells[row, col].Fill = Brushes.Black;
                    }
                }
            }
            HideBornCells.Visibility = Visibility.Collapsed;
            BornCells.Visibility = Visibility.Visible;
        }

        private void ShowDeadCells_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (cellsDied[row, col])
                    {
                        cells[row, col].Fill = Brushes.Red;
                    }
                }
            }
            DeadCells.Visibility = Visibility.Collapsed;
            HideDeadCells.Visibility = Visibility.Visible;
        }

        private void HideDeadCells_Click(object sender, RoutedEventArgs e)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (cellsDied[row, col])
                    {
                        cells[row, col].Fill = Brushes.White;
                    }
                }
            }
            HideDeadCells.Visibility = Visibility.Collapsed;
            DeadCells.Visibility = Visibility.Visible;
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            string currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string goalDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName).FullName;
            string savedGamesFolderPath = System.IO.Path.Combine(goalDirectory, "SavedGames");

            if (!System.IO.Directory.Exists(savedGamesFolderPath))
            {
                System.IO.Directory.CreateDirectory(savedGamesFolderPath);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileName = "TheGameOfLifeSave_" + timestamp + ".txt";

            string filePath = System.IO.Path.Combine(savedGamesFolderPath, fileName);

            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath))
                {
                    file.WriteLine(Rows);
                    file.WriteLine(Columns);

                    for (int row = 0; row < Rows; row++)
                    {
                        for (int col = 0; col < Columns; col++)
                        {
                            file.Write(cellStates[row, col] ? "1" : "0");
                        }
                        file.WriteLine(); 
                    }

                    file.WriteLine(cellsDiedNumber);
                    file.WriteLine(cellsBornNumber);

                    foreach (GameState gameState in gameHistory)
                    {
                        for (int row = 0; row < Rows; row++)
                        {
                            for (int col = 0; col < Columns; col++)
                            {
                                file.Write(gameState.CellStates[row, col] ? "1" : "0");
                            }
                            file.WriteLine(); 
                        }

                        file.WriteLine(gameState.CellsDiedNumber);
                        file.WriteLine(gameState.CellsBornNumber);
                    }

                    MessageBox.Show("Game state saved successfully to the 'SavedGames' folder.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving game state: " + ex.Message);
            }
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            if (gameHistory.Count > 0)
            {
                Stack<GameState> gameHistoryCopy = new Stack<GameState>(gameHistory);
                foreach (GameState gameState in gameHistoryCopy)
                {
                    GameState initialState = gameHistory.Peek();
                    gameHistory.Pop();
                    if(gameHistory.Count == 0)
                    {
                        for (int row = 0; row < Rows; row++)
                        {
                            for (int col = 0; col < Columns; col++)
                            {
                                cellStates[row, col] = initialState.CellStates[row, col];
                                cells[row, col].Fill = cellStates[row, col] ? Brushes.Black : Brushes.White;
                            }
                        }
                    }
                }

                cellsDiedNumber = 0;
                cellsBornNumber = 0;
            }

            gameHistory.Clear();

            GenerationLabel.Content = "1";
            CellsDiedLabel.Content = cellsDiedNumber.ToString();
            CellsBornLabel.Content = cellsBornNumber.ToString();

            BornCells.IsEnabled = false;
            DeadCells.IsEnabled = false;
            UndoNextState.IsEnabled = false;
            RestartGame.IsEnabled = false;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

    }
}
