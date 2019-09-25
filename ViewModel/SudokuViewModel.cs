using SudokuGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuGame.ViewModel
{
    /// <summary>
    /// This is the ViewModel to the view. Here model data is modified 
    /// as per our view's requirements.
    /// 
    /// @author Aamir Jamal
    /// </summary>
    class SudokuViewModel
    {
        private SudokuModel model;
        public SudokuViewModel(string difficulty)
        {
            model = new SudokuModel(9, difficulty);
        }

        public int[,] Solution { get { return model.Solution; } }
        public int[,] Puzzle { get { return model.Puzzle; } }

    }
}
