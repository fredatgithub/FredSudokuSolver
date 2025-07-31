using System;

namespace SudokuSolver
{
  internal static class Program
  {
    const int N = 9;

    static void Main()
    {
      int[,] board = new int[9, 9]
        {
            {8, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 3, 6, 0, 0, 0, 0, 0},
            {0, 7, 0, 0, 9, 0, 2, 0, 0},
            {0, 5, 0, 0, 0, 7, 0, 0, 0},
            {0, 0, 0, 0, 4, 5, 7, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 3, 0},
            {0, 0, 1, 0, 0, 0, 0, 6, 8},
            {0, 0, 8, 5, 0, 0, 0, 1, 0},
            {0, 9, 0, 0, 0, 0, 4, 0, 0}
        };

      Console.WriteLine("Sudoku à résoudre :");
      PrintBoard(board);
      Console.WriteLine();
      Console.WriteLine("Résolution du Sudoku...");
      Console.WriteLine();
      // Résoudre le Sudoku
      if (SolveSudoku(board))
      {
        PrintBoard(board);
      }
      else
      {
        Console.WriteLine("Aucune solution trouvée.");
      }

      Console.WriteLine();
      Console.WriteLine("Appuyez sur une touche pour quitter...");
      Console.ReadKey();
    }

    /// <summary>
    /// Resoudre le Sudoku en utilisant la méthode de backtracking.
    /// </summary>
    /// <param name="board">Le tableau de Sudoku</param>
    /// <returns>True si le Sudoku est résolu, sinon False</returns>
    static bool SolveSudoku(int[,] board)
    {
      for (int row = 0; row < N; row++)
      {
        for (int col = 0; col < N; col++)
        {
          if (board[row, col] == 0)
          {
            for (int num = 1; num <= 9; num++)
            {
              if (IsSafe(board, row, col, num))
              {
                board[row, col] = num;

                if (SolveSudoku(board))
                {
                  return true;
                }

                board[row, col] = 0; // backtrack
              }
            }

            return false; // aucun nombre ne convient ici
          }
        }
      }

      // Si nous avons atteint ici, cela signifie que le Sudoku est résolu
      return true; // tout est rempli
    }

    /// <summary>
    /// Est-ce que le nombre peut être placé dans la case (row, col) ?
    /// </summary>
    /// <param name="board">Le tableau de Sudoku</param>
    /// <param name="row">La ligne de la case</param>
    /// <param name="col">La colonne de la case</param>
    /// <param name="num">Le nombre à placer</param>
    /// <returns>True si le nombre peut être placé, sinon False</returns>
    static bool IsSafe(int[,] board, int row, int col, int num)
    {
      for (int x = 0; x < N; x++)
      {
        if (board[row, x] == num || board[x, col] == num)
        {
          return false;
        }
      }

      int startRow = row - row % 3;
      int startCol = col - col % 3;

      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          if (board[i + startRow, j + startCol] == num)
          {
            return false;
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Affiche le tableau de Sudoku dans la console.
    /// </summary>
    /// <param name="board">Le tableau de Sudoku</param>
    static void PrintBoard(int[,] board)
    {
      for (int i = 0; i < N; i++)
      {
        if (i % 3 == 0 && i != 0)
        {
          Console.WriteLine("------+-------+------");
        }

        for (int j = 0; j < N; j++)
        {
          if (j % 3 == 0 && j != 0)
          {
            Console.Write("| ");
          }

          Console.Write(board[i, j] + " ");
        }

        Console.WriteLine();
      }
    }
  }
}
