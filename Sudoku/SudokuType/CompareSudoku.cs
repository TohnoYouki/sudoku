using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class CompareSudoku : StandardSudoku
    {    //不等号数独，在标准9*9数独的基础上指定一些相邻格子间的大小关系
        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            base.SetBoundary(boxes);
            foreach (SingleCompareRuler ruler in compareRulers)
            {
                IntVector2 Index1 = IndexSet[ruler.Scope[0]];
                IntVector2 Index2 = IndexSet[ruler.Scope[1]];
                if (Index2 == Index1.Up()) boxes[Index1].TopLarger.Visibility = System.Windows.Visibility.Visible;
                if (Index2 == Index1.Down()) boxes[Index1].DownLarger.Visibility = System.Windows.Visibility.Visible;
                if (Index2 == Index1.Left()) boxes[Index1].LeftLarger.Visibility = System.Windows.Visibility.Visible;
                if (Index2 == Index1.Right()) boxes[Index1].RightLarger.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public List<SingleCompareRuler> compareRulers = new List<SingleCompareRuler>();

        private bool IfHaveCompareUniqueSolution(int Index)
        {
            SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
            for (int i = 0; i < compareRulers.Count; i++)
                if (i != Index)
                    solution.AddRuler(compareRulers[i]);
            return solution.IfHaveUniqueSolution();
        }

        private bool ReduceCompareRuler()
        {
            int Index;
            List<bool> IfHaveUnique = new List<bool>();
            for (int i = 0; i < compareRulers.Count; i++) IfHaveUnique.Add(true);
            while (run)
            {
                Index = -1;
                ContinusRandomSelect x = new ContinusRandomSelect();
                //Console.WriteLine(compareRulers.Count);
                for (int i = 0; i < compareRulers.Count; i++)
                    if (IfHaveUnique[i] && x.SelectThis())
                        Index = i;
                if (Index == -1) break;
                if (IfHaveCompareUniqueSolution(Index))
                {
                    compareRulers.RemoveAt(Index);
                    IfHaveUnique.RemoveAt(Index);
                }
                else
                    IfHaveUnique[Index] = false;
            }
            if (run) return true; else return false;
        }

        private void GenerateCompareRulers()
        {
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                {
                    IntVector2 Index1 = new IntVector2(i, j);
                    IntVector2 Index2 = new IntVector2(i + 1, j);
                    IntVector2 Index3 = new IntVector2(i, j + 1);
                    if (i + 1 < 10)
                        if (Result[Index1] > Result[Index2])
                            compareRulers.Add(new SingleCompareRuler(solutionNumbers[Index1], solutionNumbers[Index2]));
                        else if (Result[Index1] < Result[Index2])
                            compareRulers.Add(new SingleCompareRuler(solutionNumbers[Index2], solutionNumbers[Index1]));
                    if (j + 1 < 10)
                        if (Result[Index1] > Result[Index3])
                            compareRulers.Add(new SingleCompareRuler(solutionNumbers[Index1], solutionNumbers[Index3]));
                        else if (Result[Index1] < Result[Index3])
                            compareRulers.Add(new SingleCompareRuler(solutionNumbers[Index3], solutionNumbers[Index1]));
                }
        }
        //不等号数独首先同标准数独一样生成终盘，在终盘的基础上添加对应的不等号规则
        //然后先同标准数独一样去除数字，待不能再去除时再去除多余的不等号规则，最终生成整个数独格局。
        public override bool GenerateSudoku()
        {
            if (Initial())
            {
                PrimaryDisk.Clear();
                restrictions.Clear();
                foreach (IntVector2 Index in Result.Keys)
                {
                    PrimaryDisk.Add(Index, Result[Index]);
                    restrictions.Add(Index, new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
                }
                GenerateCompareRulers();
                foreach (Ruler ruler in compareRulers)
                    Rulers.Add(ruler);
                ReduceNumbers();
                foreach (Ruler ruler in compareRulers)
                    Rulers.RemoveAt(Rulers.Count - 1);
                foreach (IntVector2 Index in PrimaryDisk.Keys)
                    if (PrimaryDisk[Index] != 0)
                        Rulers.Add(restrictions[Index]);
                if (!ReduceCompareRuler()) return false;
                return true;
            }
            return false;
        }
    }
}
