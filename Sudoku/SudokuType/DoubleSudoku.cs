using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class DoubleSudoku : SudokuInstance
    {   //双重数独，两个标准数独的合并
        public override void ClearDraftLaber(Dictionary<IntVector2, SudokuBox> Boxs, SudokuBox nowSelectedBox)
        {
            bool Find = false;
            IntVector2 TargetIndex = new IntVector2();
            foreach (IntVector2 index in Boxs.Keys)
                if (Boxs[index] == nowSelectedBox) { TargetIndex = index; Find = true; break; }
            if (!Find) return;
            int clearDraft = nowSelectedBox.BoxNumber;

            foreach (IntVector2 Index in Boxs.Keys)
                if ((Index.X >= 1 && Index.X <= 9 && Index.Y >= 1 && Index.Y <= 9 && TargetIndex.X >= 1 && TargetIndex.X <= 9 && TargetIndex.Y >= 1 && TargetIndex.Y <= 9) || ((Index.X >= 7 && Index.X <= 15 && Index.Y >= 4 && Index.Y <= 12 && TargetIndex.X >= 7 && TargetIndex.X <= 15 && TargetIndex.Y >= 4 && TargetIndex.Y <= 12)))
                    if (Index.X == TargetIndex.X || Index.Y == TargetIndex.Y || ((Index.X - 1) / 3 == (TargetIndex.X - 1) / 3 && (Index.Y - 1) / 3 == (TargetIndex.Y - 1) / 3))
                        Boxs[Index].SetDraftLabel(clearDraft, System.Windows.Visibility.Hidden);
        }

        private List<Ruler> downRulers = new List<Ruler>();
        private List<SolutionNumber> downNumberInList = new List<SolutionNumber>();

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
            for (int i = 7; i < 16; i = i + 3)
                for (int j = 4; j < 13; j = j + 3)
                {
                    smallArray.Clear();
                    for (int m = 0; m < 3; m++)
                        for (int n = 0; n < 3; n++) smallArray.Add(new IntVector2(i + m, j + n), boxes[new IntVector2(i + m, j + n)]);
                    BoundaryDraw.SetBoundary(smallArray);
                }
        }

        public DoubleSudoku()
        {
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                {
                    SolutionNumber x = new SolutionNumber(9);
                    solutionNumbers.Add(new IntVector2(i, j), x);
                    numbersInList.Add(x);
                }

            for (int i = 7; i < 16; i++)
                for (int j = 4; j < 13; j++)
                {
                    if (!solutionNumbers.ContainsKey(new IntVector2(i, j)))
                        solutionNumbers.Add(new IntVector2(i, j), new SolutionNumber(9));
                    downNumberInList.Add(solutionNumbers[new IntVector2(i, j)]);
                }

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

            for (int i = 7; i < 16; i++)
            {
                List<SolutionNumber> y = new List<SolutionNumber>();
                for (int j = 4; j < 13; j++)
                    y.Add(solutionNumbers[new IntVector2(i, j)]);
                downRulers.Add(new NotRepeatFullRuler(y));
            }

            for (int i = 4; i < 13; i++)
            {
                List<SolutionNumber> y = new List<SolutionNumber>();
                for (int j = 7; j < 16; j++)
                    y.Add(solutionNumbers[new IntVector2(j, i)]);
                downRulers.Add(new NotRepeatFullRuler(y));
            }

            for (int i = 7; i < 16; i = i + 3)
                for (int j = 4; j < 13; j = j + 3)
                {
                    List<SolutionNumber> y = new List<SolutionNumber>();
                    for (int m = 0; m < 3; m++)
                        for (int n = 0; n < 3; n++)
                            y.Add(solutionNumbers[new IntVector2(i + m, j + n)]);
                    downRulers.Add(new NotRepeatFullRuler(y));
                }
            AddIndexSet();
        }
        private Dictionary<IntVector2, OneNumberRuler> restrictions = new Dictionary<IntVector2, OneNumberRuler>();
        private Dictionary<IntVector2, OneNumberRuler> downRestrictions = new Dictionary<IntVector2, OneNumberRuler>();

        public override bool Initial(int maxDelay = -1)
        {
            SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
            if (!solution.Solution()) return false;
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                {
                    IntVector2 Index = new IntVector2(i, j);
                    Result[Index] = solutionNumbers[Index].RandomSelect();
                }

            solution = new SudokuSolution(downNumberInList, downRulers);
            for (int i = 7; i < 10; i++)
                for (int j = 4; j < 10; j++)
                {
                    IntVector2 Index = new IntVector2(i, j);
                    solution.AddRuler(new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
                }
            if (!solution.Solution()) return false;
            for (int i = 7; i < 16; i++)
                for (int j = 4; j < 13; j++)
                {
                    IntVector2 Index = new IntVector2(i, j);
                    Result[Index] = solutionNumbers[Index].RandomSelect();
                }
            return true;
        }

        private void ResultToNumber()
        {
            PrimaryDisk.Clear();
            restrictions.Clear();
            downRestrictions.Clear();
            foreach (IntVector2 Index in Result.Keys)
                PrimaryDisk.Add(Index, Result[Index]);
            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                {
                    IntVector2 Index = new IntVector2(i, j);
                    restrictions.Add(Index, new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
                }
            for (int i = 7; i < 16; i++)
                for (int j = 4; j < 13; j++)
                {
                    IntVector2 Index = new IntVector2(i, j);
                    downRestrictions.Add(Index, new OneNumberRuler(solutionNumbers[Index], Result[Index], true));
                }
        }

        private bool IfHaveUniqueSolution(IntVector2 Out)
        {
            bool result = true;
            if (restrictions.ContainsKey(Out))
            {
                SudokuSolution solution = new SudokuSolution(numbersInList, Rulers);
                foreach (IntVector2 Index in restrictions.Keys)
                    if (PrimaryDisk[Index] != 0)
                        if (Index != Out) solution.AddRuler(restrictions[Index]); else solution.AddRuler(new OneNumberRuler(solutionNumbers[Index], Result[Index], false));
                result = result & !solution.Solution();
            }
            if (result && downRestrictions.ContainsKey(Out))
            {
                SudokuSolution solution = new SudokuSolution(downNumberInList, downRulers);
                foreach (IntVector2 Index in downRestrictions.Keys)
                    if (PrimaryDisk[Index] != 0)
                        if (Index != Out) solution.AddRuler(downRestrictions[Index]); else solution.AddRuler(new OneNumberRuler(solutionNumbers[Index], Result[Index], false));
                result = result & !solution.Solution();
            }
            return result;
        }

        private void ReduceNumbers()
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
        //双重数独先生成一个标准数独，然后根据这个标准数独提供的信息生成另外一个重合的标准数独
        //从而完成终局的生成。而初局的生成同标准数独一样。
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
