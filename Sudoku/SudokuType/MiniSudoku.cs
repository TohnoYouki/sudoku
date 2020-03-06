using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class MiniSudoku : StandardSudoku
    {   //迷你6*6数独，每一行每一列每个2*3格子中数字不重复
        public override void ClearDraftLaber(Dictionary<IntVector2, SudokuBox> Boxs, SudokuBox nowSelectedBox)
        {
            bool Find = false;
            IntVector2 TargetIndex = new IntVector2();
            foreach (IntVector2 index in Boxs.Keys)
                if (Boxs[index] == nowSelectedBox) { TargetIndex = index; Find = true; break; }
            if (!Find) return;
            int clearDraft = nowSelectedBox.BoxNumber;
            foreach (IntVector2 Index in Boxs.Keys)
                if (Index.X == TargetIndex.X || Index.Y == TargetIndex.Y || ((Index.X - 1) / 2 == (TargetIndex.X - 1) / 2 && (Index.Y - 1) / 3 == (TargetIndex.Y - 1) / 3))
                    Boxs[Index].SetDraftLabel(clearDraft, System.Windows.Visibility.Hidden);
        }

        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            Dictionary<IntVector2, SudokuBox> smallArray = new Dictionary<IntVector2, SudokuBox>();
            for (int i = 1; i < 7; i = i + 2)
                for (int j = 1; j < 7; j = j + 3)
                {
                    smallArray.Clear();
                    for (int m = 0; m < 2; m++)
                        for (int n = 0; n < 3; n++) smallArray.Add(new IntVector2(i + m, j + n), boxes[new IntVector2(i + m, j + n)]);
                    BoundaryDraw.SetBoundary(smallArray);
                }
        }

        public MiniSudoku()
        {
            Rulers.Clear();
            solutionNumbers.Clear();
            numbersInList.Clear();

            for (int i = 1; i < 7; i++)
                for (int j = 1; j < 7; j++)
                    solutionNumbers.Add(new IntVector2(i, j), new SolutionNumber(6));

            for (int i = 1; i < 7; i++)
            {
                List<SolutionNumber> y = new List<SolutionNumber>();
                for (int j = 1; j < 7; j++)
                    y.Add(solutionNumbers[new IntVector2(i, j)]);
                Rulers.Add(new NotRepeatFullRuler(y));
            }

            for (int i = 1; i < 7; i++)
            {
                List<SolutionNumber> y = new List<SolutionNumber>();
                for (int j = 1; j < 7; j++)
                    y.Add(solutionNumbers[new IntVector2(j, i)]);
                Rulers.Add(new NotRepeatFullRuler(y));
            }

            for (int i = 1; i < 7; i = i + 2)
                for (int j = 1; j < 7; j = j + 3)
                {
                    List<SolutionNumber> y = new List<SolutionNumber>();
                    for (int m = 0; m < 2; m++)
                        for (int n = 0; n < 3; n++)
                            y.Add(solutionNumbers[new IntVector2(i + m, j + n)]);
                    Rulers.Add(new NotRepeatFullRuler(y));
                }

            foreach (SolutionNumber y in solutionNumbers.Values)
                numbersInList.Add(y);
            AddIndexSet();
        }
    }
}
