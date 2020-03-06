using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sudoku
{
    class FerrisWheelSudoku : StandardSudoku
    {   //摩天轮数独，在标准9*9的数独基础上给定摩天轮规则
        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            base.SetBoundary(boxes);
            foreach (FerrisWheelRuler ruler in ferrisWheels)
            {
                IntVector2 Index1 = IndexSet[ruler.Scope[0]];
                IntVector2 Index2 = IndexSet[ruler.Scope[1]];
                if (Index1 == Index2.Up())
                {
                    boxes[Index1].FerrisNumberTop.Content = ruler.FerrisWheelNumber;
                    boxes[Index1].FerrisNumberTop.Foreground = new SolidColorBrush(Colors.Gold);
                    boxes[Index1].FerrisNumberTop.Visibility = System.Windows.Visibility.Visible;
                }
                if (Index1 == Index2.Down())
                {
                    boxes[Index1].FerrisNumberDown.Content = ruler.FerrisWheelNumber;
                    boxes[Index1].FerrisNumberDown.Foreground = new SolidColorBrush(Colors.Gold);
                    boxes[Index1].FerrisNumberDown.Visibility = System.Windows.Visibility.Visible;
                }
                if (Index1 == Index2.Left())
                {
                    boxes[Index1].FerrisNumberLeft.Content = ruler.FerrisWheelNumber;
                    boxes[Index1].FerrisNumberLeft.Foreground = new SolidColorBrush(Colors.Gold);
                    boxes[Index1].FerrisNumberLeft.Visibility = System.Windows.Visibility.Visible;
                }
                if (Index1 == Index2.Right())
                {
                    boxes[Index1].FerrisNumberRight.Content = ruler.FerrisWheelNumber;
                    boxes[Index1].FerrisNumberRight.Foreground = new SolidColorBrush(Colors.Gold);
                    boxes[Index1].FerrisNumberRight.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        public List<FerrisWheelRuler> ferrisWheels = new List<FerrisWheelRuler>();
        private void AddRuler()
        {
            for (int i = 1; i <= 9; i++)
            {
                List<SolutionNumber> x = new List<SolutionNumber>();
                int max = -1, number = 0;
                for (int j = 1; j <= 9; j++)
                {
                    x.Add(solutionNumbers[new IntVector2(i, j)]);
                    if (max < Result[new IntVector2(i, j)]) { max = Result[new IntVector2(i, j)]; number++; }
                }
                ferrisWheels.Add(new FerrisWheelRuler(x, number));
            }
            for (int i = 1; i <= 9; i++)
            {
                List<SolutionNumber> x = new List<SolutionNumber>();
                int max = -1, number = 0;
                for (int j = 9; j >= 1; j--)
                {
                    x.Add(solutionNumbers[new IntVector2(i, j)]);
                    if (max < Result[new IntVector2(i, j)]) { max = Result[new IntVector2(i, j)]; number++; }
                }
                ferrisWheels.Add(new FerrisWheelRuler(x, number));
            }
            for (int i = 1; i <= 9; i++)
            {
                List<SolutionNumber> x = new List<SolutionNumber>();
                int max = -1, number = 0;
                for (int j = 1; j <= 9; j++)
                {
                    x.Add(solutionNumbers[new IntVector2(j, i)]);
                    if (max < Result[new IntVector2(j, i)]) { max = Result[new IntVector2(j, i)]; number++; }
                }
                ferrisWheels.Add(new FerrisWheelRuler(x, number));
            }
            for (int i = 1; i <= 9; i++)
            {
                List<SolutionNumber> x = new List<SolutionNumber>();
                int max = -1, number = 0;
                for (int j = 9; j >= 1; j--)
                {
                    x.Add(solutionNumbers[new IntVector2(j, i)]);
                    if (max < Result[new IntVector2(j, i)]) { max = Result[new IntVector2(j, i)]; number++; }
                }
                ferrisWheels.Add(new FerrisWheelRuler(x, number));
            }
        }

        public bool IfHaveFerrisUniqueSolution(int Out)
        {
            SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
            for (int i = 0; i < ferrisWheels.Count; i++)
                if (i != Out) solution.AddRuler(ferrisWheels[i]);
            return solution.IfHaveUniqueSolution();
        }
        //摩天轮数独首先同标准数独一样生成终盘，在终盘的基础上添加对应的摩天轮规则
        //然后先同标准数独一样去除数字，待不能再去除时再去除多余的摩天轮规则，最终生成整个数独格局。
        private bool ReduceFerrisWheelRuler()
        {
            List<bool> IfHaveUnique = new List<bool>();
            foreach (FerrisWheelRuler ruler in ferrisWheels)
                IfHaveUnique.Add(true);
            int number = 0;
            while (run)
            {
                number++;
                int Out = -1;
                ContinusRandomSelect x = new ContinusRandomSelect();
                for (int i = 0; i < IfHaveUnique.Count; i++)
                    if (IfHaveUnique[i] && x.SelectThis())
                        Out = i;
                if (Out == -1) break;
                if (IfHaveFerrisUniqueSolution(Out))
                {
                    ferrisWheels.RemoveAt(Out);
                    IfHaveUnique.RemoveAt(Out);
                }
                else
                    IfHaveUnique[Out] = false;
            }
            if (run) return true; else return false;
        }

        public override bool GenerateSudoku()
        {
            if (Initial())
            {
                PrimaryDisk.Clear();
                ferrisWheels.Clear();
                restrictions.Clear();
                foreach (IntVector2 Index in Result.Keys)
                {
                    PrimaryDisk.Add(Index, Result[Index]);
                    restrictions.Add(Index, new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
                }
                AddRuler();
                foreach (Ruler ruler in ferrisWheels)
                    Rulers.Add(ruler);
                ReduceNumbers();
                foreach (Ruler ruler in ferrisWheels)
                    Rulers.RemoveAt(Rulers.Count - 1);
                foreach (IntVector2 Index in PrimaryDisk.Keys)
                    if (PrimaryDisk[Index] != 0) Rulers.Add(restrictions[Index]);
                if (!ReduceFerrisWheelRuler()) return false;
                return true;
            }
            return false;
        }
    }
}
