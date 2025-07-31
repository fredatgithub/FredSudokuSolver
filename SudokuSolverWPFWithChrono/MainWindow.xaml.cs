using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SudokuSolverWPFWithChrono
{
  /// <summary>
  /// Logique d'interaction pour MainWindow.xaml
  /// </summary>
  public partial class MainWindow: Window
  {
    private readonly TextBox[,] cells = new TextBox[9, 9];
    private bool isProgrammaticChange = false;
    private DispatcherTimer chronoTimer;
    private readonly Stopwatch liveStopwatch = new Stopwatch();

    public MainWindow()
    {
      InitializeComponent();
      LoadWindowSizes();
      InitializeGrid();
      StartChrono();
    }

    private void LoadWindowSizes()
    {
      Width = Properties.Settings.Default.WindowWidth;
      Height = Properties.Settings.Default.WindowHeight;
      Top = Properties.Settings.Default.WindowTop;
      Left = Properties.Settings.Default.WindowLeft;
      WindowState = Properties.Settings.Default.WindowState;
    }

    protected void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Properties.Settings.Default.WindowState = WindowState;

      if (WindowState == WindowState.Normal)
      {
        Properties.Settings.Default.WindowWidth = Width;
        Properties.Settings.Default.WindowHeight = Height;
        Properties.Settings.Default.WindowTop = Top;
        Properties.Settings.Default.WindowLeft = Left;
      }
      else
      {
        Properties.Settings.Default.WindowWidth = RestoreBounds.Width;
        Properties.Settings.Default.WindowHeight = RestoreBounds.Height;
        Properties.Settings.Default.WindowTop = RestoreBounds.Top;
        Properties.Settings.Default.WindowLeft = RestoreBounds.Left;
      }

      Properties.Settings.Default.Save();
    }

    private void StartChrono()
    {
      chronoTimer = new DispatcherTimer
      {
        Interval = TimeSpan.FromMilliseconds(200)
      };
      chronoTimer.Tick += (s, e) =>
      {
        TimerText.Text = $"⏱ {liveStopwatch.Elapsed:hh\\:mm\\:ss}";
      };
    }

    private void InitializeGrid()
    {
      SudokuGrid.Children.Clear();
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          var tb = new TextBox
          {
            FontSize = 20,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            MaxLength = 1
          };

          tb.TextChanged += (s, e) => { if (!isProgrammaticChange) { ValidateGrid(); } };
          if ((i / 3 + j / 3) % 2 == 0)
          {
            tb.Background = Brushes.LightGray;
          }

          SudokuGrid.Children.Add(tb);
          cells[i, j] = tb;
        }
      }
    }

    private int[,] ReadGrid()
    {
      var grid = new int[9, 9];
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          string text = cells[i, j].Text;
          grid[i, j] = int.TryParse(text, out int val) ? val : 0;
        }
      }

      return grid;
    }

    private void DisplayGrid(int[,] grid)
    {
      isProgrammaticChange = true;
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          cells[i, j].Text = grid[i, j] == 0 ? "" : grid[i, j].ToString();
        }
      }

      isProgrammaticChange = false;

      ValidateGrid();
    }

    private async void SolveButton_Click(object sender, RoutedEventArgs e)
    {
      var grid = ReadGrid();
      chronoTimer.Stop();
      liveStopwatch.Stop();
      var stopwatch = Stopwatch.StartNew();
      bool success = await Task.Run(() => Solve(grid));
      stopwatch.Stop();

      if (success)
      {
        DisplayGrid(grid);
        TimerText.Text = $"Résolu en {stopwatch.ElapsedMilliseconds} ms";
      }
      else
      {
        MessageBox.Show("Aucune solution trouvée.");
      }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
      isProgrammaticChange = true;
      foreach (var tb in cells)
      {
        tb.Text = string.Empty;
        tb.Foreground = Brushes.Black;
      }
      isProgrammaticChange = false;

      TimerText.Text = string.Empty;
    }

    private void LoadExtremeGrid_Click(object sender, RoutedEventArgs e)
    {
      int[,] extremeGrid = new int[9, 9]
      {
                {8,0,0,0,0,0,0,0,0},
                {0,0,3,6,0,0,0,0,0},
                {0,7,0,0,9,0,2,0,0},
                {0,5,0,0,0,7,0,0,0},
                {0,0,0,0,4,5,7,0,0},
                {0,0,0,1,0,0,0,3,0},
                {0,0,1,0,0,0,0,6,8},
                {0,0,8,5,0,0,0,1,0},
                {0,9,0,0,0,0,4,0,0}
      };
      DisplayGrid(extremeGrid);
      TimerText.Text = "Grille extrême chargée";
    }

    private bool Solve(int[,] board)
    {
      for (int row = 0; row < 9; row++)
      {
        for (int col = 0; col < 9; col++)
        {
          if (board[row, col] == 0)
          {
            for (int num = 1; num <= 9; num++)
            {
              if (IsSafe(board, row, col, num))
              {
                board[row, col] = num;
                if (Solve(board))
                {
                  return true;
                }

                board[row, col] = 0;
              }
            }

            return false;
          }
        }
      }

      return true;
    }

    private static bool IsSafe(int[,] board, int row, int col, int num)
    {
      for (int i = 0; i < 9; i++)
      {
        if (board[row, i] == num || board[i, col] == num)
        {
          return false;
        }
      }

      int boxRow = row - row % 3;
      int boxCol = col - col % 3;
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          if (board[boxRow + i, boxCol + j] == num)
          {
            return false;
          }
        }
      }

      return true;
    }

    private void ValidateGrid()
    {
      isProgrammaticChange = true;
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          cells[i, j].Foreground = Brushes.Black;
          string txt = cells[i, j].Text;
          if (!int.TryParse(txt, out int val) || val < 1 || val > 9)
          {
            continue;
          }

          // Vérifier conflits
          for (int k = 0; k < 9; k++)
          {
            if (k != j && cells[i, k].Text == txt)
            {
              cells[i, j].Foreground = Brushes.Red;
            }

            if (k != i && cells[k, j].Text == txt)
            {
              cells[i, j].Foreground = Brushes.Red;
            }
          }

          int boxRow = i / 3 * 3;
          int boxCol = j / 3 * 3;
          for (int r = boxRow; r < boxRow + 3; r++)
          {
            for (int c = boxCol; c < boxCol + 3; c++)
            {
              if ((r != i || c != j) && cells[r, c].Text == txt)
              {
                cells[i, j].Foreground = Brushes.Red;
              }
            }
          }
        }
      }

      isProgrammaticChange = false;
    }

    private void ExportCsvButton_Click(object sender, RoutedEventArgs e)
    {
      var grid = ReadGrid();

      var dialog = new Microsoft.Win32.SaveFileDialog
      {
        FileName = "sudoku_solution",
        DefaultExt = ".csv",
        Filter = "Fichiers CSV (*.csv)|*.csv"
      };

      bool? result = dialog.ShowDialog();
      if (result == true)
      {
        string path = dialog.FileName;

        try
        {
          using (var writer = new StreamWriter(path))
          {
            for (int i = 0; i < 9; i++)
            {
              string line = string.Join(",", Enumerable.Range(0, 9).Select(j => grid[i, j].ToString()));
              writer.WriteLine(line);
            }
          }

          MessageBox.Show("Export CSV réussi !");
        }
        catch (Exception exception)
        {
          MessageBox.Show($"Erreur lors de l'export : {exception.Message}");
        }
      }
    }

    private void ImportCsvButton_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new Microsoft.Win32.OpenFileDialog
      {
        Filter = "Fichiers CSV (*.csv)|*.csv"
      };

      bool? result = dialog.ShowDialog();
      if (result != true)
      {
        return;
      }

      try
      {
        var lines = System.IO.File.ReadAllLines(dialog.FileName);

        if (lines.Length != 9)
        {
          throw new FormatException("Le fichier doit contenir exactement 9 lignes.");
        }

        var grid = new int[9, 9];

        for (int i = 0; i < 9; i++)
        {
          var parts = lines[i].Split(',');

          if (parts.Length != 9)
          {
            throw new FormatException($"Ligne {i + 1} invalide : elle ne contient pas 9 valeurs séparées par des virgules.");
          }

          for (int j = 0; j < 9; j++)
          {
            string cell = parts[j].Trim();
            if (cell == "0" || string.IsNullOrEmpty(cell))
            {
              grid[i, j] = 0;
            }
            else if (int.TryParse(cell, out int val) && val >= 1 && val <= 9)
            {
              grid[i, j] = val;
            }
            else
            {
              throw new FormatException($"Valeur invalide à la ligne {i + 1}, colonne {j + 1} : \"{cell}\". Utilise uniquement des nombres entre 1 et 9 ou 0.");
            }
          }
        }

        DisplayGrid(grid);
        TimerText.Text = "Grille CSV chargée avec succès.";
      }
      catch (Exception exception)
      {
        MessageBox.Show($"Erreur d'import :\n{exception.Message}", "Import CSV", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void StartChrono_Click(object sender, RoutedEventArgs e)
    {
      if (!liveStopwatch.IsRunning)
      {
        liveStopwatch.Start();
        chronoTimer.Start();
      }
    }

    private void ResetChrono_Click(object sender, RoutedEventArgs e)
    {
      chronoTimer.Stop();
      liveStopwatch.Reset();
      TimerText.Text = "⏱ 00:00:00";
    }

    private void ResetWindowSizeButton_Click(object sender, RoutedEventArgs e)
    {
      // Définir la taille par défaut
      // Assure que la fenêtre n'est pas maximisée ou minimisée
      WindowState = WindowState.Normal; 

      Width = 1024;
      Height = 900;

      // Centrer la fenêtre sur l'écran principal
      Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
      Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

      // Optionnel : sauvegarder immédiatement dans les Settings
      Properties.Settings.Default.WindowWidth = Width;
      Properties.Settings.Default.WindowHeight = Height;
      Properties.Settings.Default.WindowTop = Top;
      Properties.Settings.Default.WindowLeft = Left;
      Properties.Settings.Default.WindowState = WindowState;
      Properties.Settings.Default.Save();
    }
  }
}
