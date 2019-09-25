using SudokuGame.ViewModel;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml (view)
    /// 
    /// @author Aamir Jamal
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush invalidColor = System.Windows.Media.Brushes.IndianRed;
        Brush backColor = System.Windows.Media.Brushes.Silver;
        private bool _shouldReveal = false;
        private SudokuViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Generates the cells in the grid with appropriate values
        /// taken from the viewmodel puzzle.
        /// </summary>
        /// <param name="puzzle">Puzzle taken from the viewmodel</param>
        private void Init_board(int[,] puzzle)
        {
            if (gameBoard.Children.Count != 0)       // clearing the grid of previous content
                gameBoard.Children.RemoveRange(0, 81);

            int rowIndex = 0;
            foreach (var row in gameBoard.RowDefinitions)
            {
                int colIndex = 0;
                foreach (var col in gameBoard.ColumnDefinitions)
                {
                    Border panel = new Border();
                    Grid.SetColumn(panel, colIndex);
                    Grid.SetRow(panel, rowIndex);
                    panel.Background = backColor;
                    int digit = puzzle[rowIndex, colIndex];
                    if (digit == 0)     // Creating a textbox for blank
                    {
                        TextBox num = new TextBox();
                        num.Uid = "" + rowIndex + colIndex;
                        num.MaxLength = 1;
                        num.Width = 15;
                        num.Margin = new Thickness(0, 3, 0, 3);
                        num.PreviewTextInput += NumberValidationTextBox;
                        num.PreviewMouseLeftButtonDown += MouseUpTextBox;
                        num.Background = backColor;
                        panel.Child = num;
                    }
                    else
                    {
                        Label lbl = new Label();
                        lbl.Content = "" + puzzle[rowIndex, colIndex];
                        lbl.HorizontalAlignment = HorizontalAlignment.Center;
                        lbl.VerticalAlignment = VerticalAlignment.Center;
                        lbl.Background = backColor;
                        panel.Child = lbl;
                    }
                    panel.BorderBrush = System.Windows.Media.Brushes.Black;
                    AddBorderIfNeeded(panel, rowIndex, colIndex);
                    gameBoard.Children.Add(panel);
                    colIndex++;
                }
                rowIndex++;
            }
        }

        /// <summary>
        /// Textbox MouseUp handler. If _shouldReveal flag is on,
        /// reveals the solution of the cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseUpTextBox(object sender, MouseButtonEventArgs e)
        {
            TextBox txtbox = (TextBox)sender;
            if (_shouldReveal)
            {
                int row = Convert.ToInt32(txtbox.Uid.Substring(0, 1));
                int col = Convert.ToInt32(txtbox.Uid.Substring(1));
                txtbox.Text = _viewModel.Solution[row, col].ToString();
                _shouldReveal = false;
                txtReveal.Visibility = Visibility.Hidden;
            }
            txtbox.Background = backColor;
        }

        /// <summary>
        /// Adds border where required for clear presentation.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void AddBorderIfNeeded(Border panel, int row, int col)
        {
            if ((row + 1) % 3 == 0 && (col + 1) % 3 == 0)
                panel.BorderThickness = new Thickness(0, 0, 3, 3);
            else if ((row + 1) % 3 == 0)
                panel.BorderThickness = new Thickness(0, 0, 0, 3);
            else if ((col + 1) % 3 == 0)
                panel.BorderThickness = new Thickness(0, 0, 3, 0);
        }

        /// <summary>
        /// TextComposition event handler to check only 1-9 digits
        /// in the text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^1-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Generates a new puzzle according to the selected difficulty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewPuzzle_Click(object sender, RoutedEventArgs e)
        {
            string difficulty = "easy";
            switch (combo_difficulty.SelectedIndex)
            {
                case 0:
                    difficulty = "easy";
                    break;
                case 1:
                    difficulty = "medium";
                    break;
                case 2:
                    difficulty = "hard";
                    break;
            }
            _viewModel = new SudokuViewModel(difficulty);
            Init_board(_viewModel.Puzzle);
            btnReveal.IsEnabled = true;
            btnValidate.IsEnabled = true;
            btnSave.IsEnabled = true;
            gameGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Turns on the _shouldReveal flag to true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReveal_Click(object sender, RoutedEventArgs e)
        {
            _shouldReveal = true;
            txtReveal.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Validates the inputs against the solution and marks red all 
        /// the invalid inputs. If no invalid input is found, displays
        /// a congratulation message for completing the sudoku successfully.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValidate_Click(object sender, RoutedEventArgs e)
        {
            bool solved = true;
            var soln = _viewModel.Solution;
            var puzzle = _viewModel.Puzzle;
            var size = Math.Sqrt(puzzle.Length);
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (puzzle[row, col] == 0)
                    {
                        var borderPanel = gameBoard.Children.Cast<Border>()
                           .First(i => i.Child.Uid == "" + row + col);
                        var txtbox = (TextBox)borderPanel.Child;
                        var value = txtbox.Text;
                        if (soln[row, col].ToString() != value)
                        {
                            txtbox.Background = invalidColor;
                            solved = false;
                        }
                    }
                }
            }
            if (solved)
                MessageBox.Show("Congratulations!!\nYou've solved the sudoku!!");
        }

        /// <summary>
        /// Opens a save dialog box to save the puzzle in a text file. The base structure
        /// has been taken from MSDN and additional logic for text has been added. Reference:
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.savefiledialog?view=netframework-4.7.2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "puzzle";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";
            // Show save file dialog box
            Nullable<bool> result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dialog.FileName;
                string textToSave = GetPuzzleSaveFormat();
                System.IO.File.WriteAllText(filename, textToSave, Encoding.UTF8);
            }
        }

        /// <summary>
        /// Generates the puzzle in a specific text format as asked
        /// in the problem statement.
        /// </summary>
        /// <returns></returns>
        private string GetPuzzleSaveFormat()
        {
            var puzzle = _viewModel.Puzzle;
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (puzzle[row, col] != 0)
                        sb.Append(puzzle[row, col] + " ");
                    else
                        sb.Append("X ");
                }
                sb.Length--;    // Trimming the extra last space.
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}
