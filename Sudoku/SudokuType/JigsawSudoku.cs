using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class JigsawSudoku : StandardSudoku
    {   //锯齿数独，每一行每一列和9个特殊的区域内数字不重复
        public override void ClearDraftLaber(Dictionary<IntVector2, SudokuBox> Boxs, SudokuBox nowSelectedBox)
        {
            bool Find = false;
            IntVector2 TargetIndex = new IntVector2();
            foreach (IntVector2 index in Boxs.Keys)
                if (Boxs[index] == nowSelectedBox) { TargetIndex = index; Find = true; break; }
            if (!Find) return;
            int clearDraft = nowSelectedBox.BoxNumber;
            foreach (IntVector2 Index in Boxs.Keys)
            {
                if (Index.X == TargetIndex.X || Index.Y == TargetIndex.Y)
                    Boxs[Index].SetDraftLabel(clearDraft, System.Windows.Visibility.Hidden);
                if (x.numbers[TargetIndex.X - 1][TargetIndex.Y - 1] == x.numbers[Index.X - 1][Index.Y - 1])
                    Boxs[Index].SetDraftLabel(clearDraft, System.Windows.Visibility.Hidden);
            }
        }

        public override void SetBoundary(Dictionary<IntVector2, SudokuBox> boxes)
        {
            Dictionary<IntVector2, SudokuBox> smallArray = new Dictionary<IntVector2, SudokuBox>();
            for (int i = 0; i < 9; i++)
            {
                smallArray.Clear();
                for (int m = 0; m < 9; m++)
                    for (int n = 0; n < 9; n++)
                        if (x.numbers[m][n] == i + 1)
                            smallArray.Add(new IntVector2(m + 1, n + 1), boxes[new IntVector2(m + 1, n + 1)]);
                BoundaryDraw.SetBoundary(smallArray);
            }
        }

        public JigsawSudoku()
        {
            for (int i = 0; i < 9; i++) Rulers.RemoveAt(Rulers.Count - 1);
        }

        //锯齿数独的终盘和初盘的生成同标准数独一样。而锯齿数独不一样的地方在于确定9个连通的区域
        //每个区域包含9个格子。这里也是采用随机搜索的方法，通过搜索回溯找到解空间中的一个随机解
        //并利用这个随机解生成相应的不重复规则。这里存在两个问题
        //寻找区域划分的时间可能太久和寻找到的区域划分不一定能生成一个数独解
        //但是通过测试发现两个问题发生的概率都非常低，于是限制搜索时间（如1s）
        //若1s内找不到区域划分和生成数独，则重新寻找新的区域划分和数独格局，直到找到为止
        //这样算法保证了一定可以生成合乎规则的数独，并且根据测试平均时间性能相当地好。
        private GenerateComponent x;
        public override bool Initial(int maxDelay = -1)
        {
            while (true)
            {
                x = new GenerateComponent();
                x.GenerateAllComponent();
                List<List<SolutionNumber>> ruler = new List<List<SolutionNumber>>();
                for (int i = 0; i < 9; i++) ruler.Add(new List<SolutionNumber>());
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        ruler[x.numbers[i][j] - 1].Add(solutionNumbers[new IntVector2(i + 1, j + 1)]);
                foreach (List<SolutionNumber> y in ruler)
                    Rulers.Add(new NotRepeatFullRuler(y));
                if (base.Initial(1)) return true;
                foreach (List<SolutionNumber> y in ruler) Rulers.RemoveAt(Rulers.Count - 1);
            }
        }
    }
}
