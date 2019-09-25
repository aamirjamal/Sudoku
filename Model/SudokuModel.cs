using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame.Model
{
    /// <summary>
    /// A model for sudoku. Contains business Logic
    /// for a unique puzzle generation.
    /// Exposes two properties: Solution and Puzzle.
    /// 
    /// @author   Aamir Jamal
    /// </summary>
    class SudokuModel
    {
        private int size;
        private string level;
        private int[,] board;
        private int[,] solution;

        public int[,] Puzzle { get { return board; } }
        public int[,] Solution { get { return solution; } }

        /// <summary>
        /// Constructor for the SudokuModel.
        /// </summary>
        /// <param name="num">Size of the puzzle</param>
        /// <param name="difficulty">Difficulty of the puzzle</param>
        public SudokuModel(int num, string difficulty)
        {
            size = num;
            level = difficulty;
            board = new int[size, size];
            solution = new int[size, size];
            if (level != "hard")            // Generating unique solution for a hard sudoku
                GenerateUniqueSolnBoard();  // takes a lot of computation time.
            else
                GenerateBoardAndSoln(); 
        }

        /// <summary>
        /// Fills the puzzle board with numbers
        /// using backtracking.
        /// </summary>
        public bool FillBoard()
        {
            var empty = GetFirstEmpty();
            if (!empty.Item1)       // Board is filled successfully
                return true;
            var row = empty.Item2;
            var col = empty.Item3;
            var randArray = GetRandomArray();
            for (int i = 1; i <= size; i++)
            {
                if (IsValidEntry(row, col, randArray[i - 1]))
                {
                    board[row, col] = randArray[i - 1];
                    if (FillBoard())
                        return true;
                    board[row, col] = 0;    // Backtracking here
                }
            }
            return false;
        }

        /// <summary>
        /// Adds blank spaces to the puzzle.
        /// </summary>
        private void PutBlanks()
        {
            var blanksToPut = GetNumOfBlanks();
            var rnd = new Random();
            while (blanksToPut != 0)
            {
                var row = rnd.Next(size);
                var col = rnd.Next(size);
                if (board[row, col] == 0)
                    continue;
                else
                {
                    board[row, col] = 0;
                    blanksToPut--;
                }
            }
        }

        /// <summary>
        /// Calculates the number of blanks required according
        /// to the difficulty level set.
        /// </summary>
        /// <returns>Number of blanks to set.</returns>
        private int GetNumOfBlanks()
        {
            switch (level)
            {
                case "easy": return (int)(size * size * 0.4);
                case "medium": return (int)(size * size * 0.55);
                case "hard": return (int)(size * size * 0.75);
                default: return (int)(size * size * 0.4);
            }
        }

        /// <summary>
        /// Generates an array filled with random unique number
        /// of the size of the board.
        /// </summary>
        /// <returns>Array of random unique numbers.</returns>
        private int[] GetRandomArray()
        {
            var nums = Enumerable.Range(1, size).ToArray();
            var rnd = new Random();

            for (var i = 0; i < nums.Length; ++i)
            {
                var randomIndex = rnd.Next(nums.Length);
                var temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }
            return nums;
        }

        /// <summary>
        /// Looks at the board and returns first empty position.
        /// </summary>
        /// <returns>A tuple containing a booleon value for 
        /// showing if there exists an empty value or not, and
        /// rest two integers showing the row and column coordinates
        /// of the empty board location.</returns>
        private Tuple<bool, int, int> GetFirstEmpty()
        {
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if (board[row, col] == 0)
                        return new Tuple<bool, int, int>(true, row, col);
                }
            }
            return new Tuple<bool, int, int>(false, -1, -1);    // Value not found.
        }

        /// <summary>
        /// Checks if the input is valid for the given location
        /// or not.
        /// </summary>
        /// <param name="row">Row coordinate</param>
        /// <param name="col">Column coordinate</param>
        /// <param name="value">Value to be entered</param>
        /// <returns>True id valid, False if invalid.</returns>
        private bool IsValidEntry(int row, int col, int value)
        {
            return IsValidColEntry(col, value) &&
                IsValidRowEntry(row, value) &&
                IsValidInRegion(row, col, value);
        }

        /// <summary>
        /// Checks if entry is valid for the column.
        /// </summary>
        /// <param name="col">Column to check validity.</param>
        /// <param name="value">Value to be entered</param>
        /// <returns>True id valid, False if invalid.</returns>
        private bool IsValidColEntry(int col, int value)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                if (board[i, col] == value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if entry is valid for the row.
        /// </summary>
        /// <param name="row">Row for which validity needs to be checked</param>
        /// <param name="value">Value to be entered.</param>
        /// <returns>True id valid, False if invalid.</returns>
        private bool IsValidRowEntry(int row, int value)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                if (board[row, i] == value)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if entry is valid for the region.
        /// </summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="col">Column coordinate.</param>
        /// <param name="value">Value to be entered.</param>
        /// <returns>True id valid, False if invalid.</returns>
        private bool IsValidInRegion(int row, int col, int value)
        {
            int sqrt = (int)Math.Sqrt(board.GetLength(0));
            int x = row / sqrt;
            int y = col / sqrt;

            for (int i = x * sqrt; i < x * sqrt + sqrt; i++)
            {
                for (int j = y * sqrt; j < y * sqrt + sqrt; j++)
                {
                    if (board[i, j] == value)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Generates a puzzle along with its solution.
        /// </summary>
        private void GenerateBoardAndSoln()
        {
            FillBoard(); 
            solution = board.Clone() as int[,];
            PutBlanks();
        }

        /// <summary>
        /// Keeps on generating puzzles until a puzzle with
        /// unique solution is not found.
        /// </summary>
        private void GenerateUniqueSolnBoard()
        {
            GenerateBoardAndSoln();
            while (GetNumOfSolutions() != 1)
                GenerateBoardAndSoln();
        }


        /// <summary>
        /// Returns the number of solutions possible from a sudoku puzzle. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="count"></param>
        /// <returns> 0 for no solution, 1 if only 1 solution 
        /// and 2 if more than one solution is possible.
        /// https://stackoverflow.com/questions/24343214/determine-whether-a-sudoku-has-a-unique-solution
        /// </returns>
        private int GetNumOfSolutions(int row = 0, int col = 0, int count = 0)
        {
            if (row == 9)
            {
                row = 0;
                if (++col == 9)
                    return 1 + count;
            }
            if (board[row, col] != 0)  // skip filled cells
                return GetNumOfSolutions(row + 1, col, count);
            for (int val = 1; val <= 9 && count < 2; ++val)
            {
                if (IsValidEntry(row, col, val))
                {
                    board[row, col] = val;
                    count = GetNumOfSolutions(row + 1, col, count);
                }
            }
            board[row, col] = 0; // backtrack
            return count;
        }

    }
}
