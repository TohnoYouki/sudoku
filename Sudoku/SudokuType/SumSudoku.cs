using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Sudoku
{
    class SumSudoku : StandardSudoku
    {      //杀手数独，在标准9*9数独的基础上制定一些区域内格子的数字和
        private Dictionary<IntVector2, Color> ColorSet = new Dictionary<IntVector2, Color>();
        private bool IsUniqueColor(Color color, List<SolutionNumber> numbers)
        {
            foreach (SolutionNumber number in numbers)
            {
                IntVector2 Index = IndexSet[number];
                if (ColorSet.ContainsKey(Index.Up()) && ColorSet[Index.Up()] == color) return false;
                if (ColorSet.ContainsKey(Index.Down()) && ColorSet[Index.Down()] == color) return false;
                if (ColorSet.ContainsKey(Index.Left()) && ColorSet[Index.Left()] == color) return false;
                if (ColorSet.ContainsKey(Index.Right()) && ColorSet[Index.Right()] == color) return false;
            }
            return true;
        }

        private void SetSumValue(SumRuler ruler, Dictionary<IntVector2, SudokuBox> boxes)
        {
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    if (ruler.Scope.Contains(solutionNumbers[new IntVector2(i, j)]))
                    {
                        boxes[new IntVector2(i, j)].SumValue.Content = ruler.Sum;
                        return;
                    }
        }

        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            base.SetBoundary(boxes);
            ColorSet.Clear();
            foreach (IntVector2 Index in solutionNumbers.Keys)
                ColorSet.Add(Index, Colors.Transparent);
            foreach (SumRuler ruler in sumRulerList)
            {
                SetSumValue(ruler, boxes);
                foreach (Color color in SudokuBox.UnHelightColors)
                    if (IsUniqueColor(color, ruler.Scope))
                    {
                        foreach (SolutionNumber number in ruler.Scope)
                        {
                            boxes[IndexSet[number]].UnHelightColor = color;
                            ColorSet[IndexSet[number]] = color;
                        }
                        break;
                    }
            }
        }

        private Dictionary<IntVector2, SumRuler> sumRulers = new Dictionary<IntVector2, SumRuler>();
        public List<SumRuler> sumRulerList = new List<SumRuler>();
        private SumRulerMachine machine;
        private void ResultToRuler()
        {
            PrimaryDisk.Clear();
            sumRulers.Clear();
            Dictionary<SolutionNumber, int> resultSet = new Dictionary<SolutionNumber, int>();
            foreach (IntVector2 Index in Result.Keys)
            {
                PrimaryDisk.Add(Index, 0);
                sumRulers.Add(Index, new SumRuler(solutionNumbers[Index], Result[Index]));
                resultSet.Add(solutionNumbers[Index], Result[Index]);
            }
            machine = new SumRulerMachine(resultSet);
            machine.AddSingleSumRuler(sumRulers);
        }
        //杀手数独首先同标准数独一样生成终盘，根据终盘为每个单独数字格子生成求和规则
        //把每个数字去掉（因为单个求和规则相当于给定数字）。通过逐渐合并求和规则的方式来生成最终的数独初盘
        //判断两个求和规则是否能够合并有两个标准：1.合并的区域内最终数字不能重复
        //这是为了防止最终单个区域过大2.合并后的数独拥有唯一解。不断合并求和规则直到生成最终的数独初盘。
        private bool ReduceRuler()
        {
            while (run)
            {
                List<SumRuler> x = machine.RandomSelect();
                if (x == null) break;
                SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
                foreach (SumRuler ruler in x)
                {
                    solution.AddRuler(ruler);
                    solution.AddRuler(new NotRepeatRuler(ruler.Scope));
                }
                if (solution.IfHaveUniqueSolution())
                    machine.Combine();
                else
                    machine.Destroy();
            }
            if (run) return true; else return false;
        }

        public override bool GenerateSudoku()
        {
            if (Initial())
            {
                ResultToRuler();
                if (!ReduceRuler()) return false;
                sumRulerList.Clear();
                foreach (CombineSumRuler combineRuler in machine.sumRulers)
                    sumRulerList.Add(combineRuler.mainRuler);
                return true;
            }
            return false;
        }
    }
}
