using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Sudoku
{
    class OddEvenSudoku : StandardSudoku
    {   //奇偶数独，在标准9*9数独的基础上指定某些格子为奇或为偶
        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            base.SetBoundary(boxes);
            foreach (OddEvenRuler ruler in oddEvenRestrictions.Values)
                if (ruler.remainder == 0)
                    boxes[IndexSet[ruler.Scope[0]]].UnHelightColor = SudokuBox.UnHelightColors[1];
                else
                    boxes[IndexSet[ruler.Scope[0]]].UnHelightColor = SudokuBox.UnHelightColors[2];
        }

        public Dictionary<IntVector2, OddEvenRuler> oddEvenRestrictions = new Dictionary<IntVector2, OddEvenRuler>();

        public bool IfHaveOddEvenUniqueSolution(IntVector2 Out)
        {
            SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
            foreach (IntVector2 Index in oddEvenRestrictions.Keys)
                if (Index != Out)
                    solution.AddRuler(oddEvenRestrictions[Index]);
                else
                    solution.AddRuler(new OddEvenRuler(solutionNumbers[Index], Result[Index] % 2 == 0));
            return !solution.Solution();
        }

        private void ReduceOddEvenRuler()
        {
            Dictionary<IntVector2, bool> IfHaveUnique = new Dictionary<IntVector2, bool>();
            foreach (IntVector2 Index in oddEvenRestrictions.Keys)
                IfHaveUnique.Add(Index, true);
            while (true)
            {
                IntVector2 Out = new IntVector2(0, 0);
                ContinusRandomSelect x = new ContinusRandomSelect();
                foreach (IntVector2 index in oddEvenRestrictions.Keys)
                    if (IfHaveUnique[index] && x.SelectThis())
                        Out = index;
                if (Out == new IntVector2(0, 0)) break;
                if (IfHaveOddEvenUniqueSolution(Out))
                    oddEvenRestrictions.Remove(Out);
                IfHaveUnique[Out] = false;
            }
        }
        //奇偶数独的终盘生成同标准数独一样，终盘生成完毕后
        //按照终盘的数字为每个格子都添加奇偶规则，然后同标准数独一样尽可能地去除数字直到不能再去除为止
        //最后开始去除多余的奇偶规则，从可选队列中随机选择一个奇偶数独
        //判断去除后是否能保证唯一解，如果能保证则去除。两种情况都从可选队列中去除该规则
        //重复上述过程直到不能再去除规则为止，或者达到制定的策略的阈值。
        public override bool GenerateSudoku()
        {
            if (Initial())
            {
                PrimaryDisk.Clear();
                restrictions.Clear();
                oddEvenRestrictions.Clear();
                foreach (IntVector2 Index in Result.Keys)
                {
                    PrimaryDisk.Add(Index, Result[Index]);
                    restrictions.Add(Index, new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
                    oddEvenRestrictions.Add(Index, new OddEvenRuler(solutionNumbers[Index], Result[Index] % 2 == 1));
                }
                foreach (Ruler ruler in oddEvenRestrictions.Values)
                    Rulers.Add(ruler);
                ReduceNumbers();
                foreach (Ruler ruler in oddEvenRestrictions.Values)
                    Rulers.RemoveAt(Rulers.Count - 1);
                foreach (IntVector2 Index in PrimaryDisk.Keys)
                    if (PrimaryDisk[Index] != 0)
                        Rulers.Add(restrictions[Index]);
                ReduceOddEvenRuler();
                return true;
            }
            return false;
        }
    }
}
