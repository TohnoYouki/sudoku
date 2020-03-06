using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class DiagonalSudoku : StandardSudoku
    {   //对角线数独，在标准9*9数独的基础上对角线上的数字也不重复
        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            base.SetBoundary(boxes);
            for (int i = 1; i < 10; i++)
            {
                boxes[new IntVector2(i, 10 - i)].UnHelightColor = SudokuBox.UnHelightColors[1];
                boxes[new IntVector2(i, i)].UnHelightColor = SudokuBox.UnHelightColors[2];
            }
        }
        //对角线数独对对角线添加了不能重复的规则后，生成方法同标准数独一样
        public DiagonalSudoku()
        {
            List<SolutionNumber> x = new List<SolutionNumber>();
            List<SolutionNumber> y = new List<SolutionNumber>();
            for (int i = 1; i <= 9; i++)
            {
                x.Add(solutionNumbers[new IntVector2(i, 10 - i)]);
                y.Add(solutionNumbers[new IntVector2(i, i)]);
            }
            NotRepeatFullRuler z = new NotRepeatFullRuler(x);
            Rulers.Add(z);
            z = new NotRepeatFullRuler(y);
            Rulers.Add(z);
        }
    }
}
