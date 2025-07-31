using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSolverWPF
{
  /// <summary>
  /// Logique d'interaction pour MainWindow.xaml
  /// </summary>
  public partial class MainWindow: Window
  {
    private readonly TextBox[,] cells = new TextBox[9, 9];

    public MainWindow()
    {
      InitializeComponent();
      InitializeGrid();
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
      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          cells[i, j].Text = grid[i, j] == 0 ? "" : grid[i, j].ToString();
        }
      }
    }

    private async void SolveButton_Click(object sender, RoutedEventArgs e)
    {
      var grid = ReadGrid();
      bool success = await Task.Run(() => Solve(grid));
      if (success)
      {
        DisplayGrid(grid);
      }
      else
      {
        MessageBox.Show("Aucune solution trouvée.");
      }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
      foreach (var tb in cells)
      {
        tb.Text = string.Empty;
      }
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
  }
}
