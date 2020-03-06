using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class StandardSudoku : SudokuInstance
    {   ////标准9*9数独，每一行每一列每个九宫格中的数字不重复
        public override void ClearDraftLaber(Dictionary<IntVector2, SudokuBox> Boxs, SudokuBox nowSelectedBox)
        {
            bool Find = false;
            IntVector2 TargetIndex = new IntVector2();
            foreach (IntVector2 index in Boxs.Keys)
                if (Boxs[index]==nowSelectedBox) { TargetIndex = index; Find = true; break; }
            if (!Find) return;
            int clearDraft = nowSelectedBox.BoxNumber;
            foreach (IntVector2 Index in Boxs.Keys)
                if (Index.X == TargetIndex.X || Index.Y == TargetIndex.Y || ((Index.X - 1) / 3 == (TargetIndex.X - 1) / 3 && (Index.Y - 1) / 3 == (TargetIndex.Y - 1) / 3))
                    Boxs[Index].SetDraftLabel(clearDraft, System.Windows.Visibility.Hidden);
        }

        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            Dictionary<IntVector2, SudokuBox> smallArray = new Dictionary<IntVector2, SudokuBox>();
            for (int i = 1; i < 10; i = i + 3)
                for (int j = 1; j < 10; j = j + 3)
                {
                    smallArray.Clear();
                    for (int m = 0; m < 3; m++)
                        for (int n = 0; n < 3; n++) smallArray.Add(new IntVector2(i + m, j + n), boxes[new IntVector2(i + m, j + n)]);
                    BoundaryDraw.SetBoundary(smallArray);
                }
        }

        public StandardSudoku()
        {
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                    solutionNumbers.Add(new IntVector2(i, j), new SolutionNumber(9));

            for (int i = 1; i < 10; i++)
            {
                List<SolutionNumber> y = new List<SolutionNumber>();
                for (int j = 1; j < 10; j++)
                    y.Add(solutionNumbers[new IntVector2(i, j)]);
                Rulers.Add(new NotRepeatFullRuler(y));
            }

            for (int i = 1; i < 10; i++)
            {
                List<SolutionNumber> y = new List<SolutionNumber>();
                for (int j = 1; j < 10; j++)
                    y.Add(solutionNumbers[new IntVector2(j, i)]);
                Rulers.Add(new NotRepeatFullRuler(y));
            }

            for (int i = 1; i < 10; i = i + 3)
                for (int j = 1; j < 10; j = j + 3)
                {
                    List<SolutionNumber> y = new List<SolutionNumber>();
                    for (int m = 0; m < 3; m++)
                        for (int n = 0; n < 3; n++)
                            y.Add(solutionNumbers[new IntVector2(i + m, j + n)]);
                    Rulers.Add(new NotRepeatFullRuler(y));
                }

            foreach (SolutionNumber y in solutionNumbers.Values)
                numbersInList.Add(y);
            AddIndexSet();
        }

        //标准数独生成算法首先专注于数独终盘的生成。
        //利用数独求解器运行在一个没有初始数字的数独上，添加标准数独包含的规则
        //因为每次选取猜测格子时是随机在候选值中挑出猜测值，所以求解器在解空间中随机搜寻
        //直到找到解，易知生成的数独解满足标准数独的规则并且具有随机性，所以可以作为标准数独的终盘。
        public override bool Initial(int maxDelay = -1)
        {
            SudokuSolution solution = new SudokuSolution(numbersInList, Rulers, maxDelay);
            if (!solution.Solution()) return false;
            foreach (IntVector2 Index in solutionNumbers.Keys)
                Result[Index] = solutionNumbers[Index].RandomSelect();
            return true;
        }

        protected Dictionary<IntVector2, OneNumberRuler> restrictions = new Dictionary<IntVector2, OneNumberRuler>();
        protected void ResultToNumber()
        {
            PrimaryDisk.Clear();
            restrictions.Clear();
            foreach (IntVector2 Index in Result.Keys)
            {
                PrimaryDisk.Add(Index, Result[Index]);
                restrictions.Add(Index, new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
            }
        }

        protected bool IfHaveUniqueSolution(IntVector2 Out)
        {
            SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
            foreach (IntVector2 Index in PrimaryDisk.Keys)
                if (PrimaryDisk[Index] != 0)
                    if (Index != Out) solution.AddRuler(restrictions[Index]); else solution.AddRuler(new OneNumberRuler(solutionNumbers[Index], Result[Index], false));
            return !solution.Solution();
        }

        protected void ReduceNumbers()
        {
            Dictionary<IntVector2, bool> IfHaveUnique = new Dictionary<IntVector2, bool>();
            foreach (IntVector2 Index in PrimaryDisk.Keys)
                IfHaveUnique.Add(Index, true);
            while (true)
            {
                IntVector2 Out = new IntVector2(0, 0);
                ContinusRandomSelect x = new ContinusRandomSelect();
                foreach (IntVector2 index in PrimaryDisk.Keys)
                    if (IfHaveUnique[index] && x.SelectThis())
                        Out = index;
                if (Out == new IntVector2(0, 0)) break;
                if (IfHaveUniqueSolution(Out))
                    PrimaryDisk[Out] = 0;
                IfHaveUnique[Out] = false;
            }
        }
        //而初盘的生成基于在终盘上去数字。从现有可选的数字中随机选择一个去掉
        //用数独求解器判断是否存在唯一解。如果没有唯一解表示去掉这个数字会破坏数独解的唯一性
        //所以标记该数字不能去掉，如果存在唯一解则直接去掉该数字。不管如何都将这个数字从可选队列中去除
        //重复上述过程直到可选队列为空，到此为止生成数独的初盘。
        public override bool GenerateSudoku()
        {
            if (Initial())
            {
                ResultToNumber();
                ReduceNumbers();
                return true;
            }
            return false;
        }
    }
}
