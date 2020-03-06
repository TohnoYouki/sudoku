using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Sudoku
{
    class BoundaryDraw
    { //绘制边界
        public static void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            Color East, West, South, North;
            foreach (IntVector2 Index in boxes.Keys)
            {
                if (boxes.ContainsKey(Index.Up())) North = Colors.Transparent; else North = Colors.Black;
                if (boxes.ContainsKey(Index.Down())) South = Colors.Transparent; else South = Colors.Black;
                if (boxes.ContainsKey(Index.Left())) West = Colors.Transparent; else West = Colors.Black;
                if (boxes.ContainsKey(Index.Right())) East = Colors.Transparent; else East = Colors.Black;
                boxes[Index].BoundaryOrientation = new SudokuBox.Orientation(North, South, West, East);
            }
        }
    }
}
