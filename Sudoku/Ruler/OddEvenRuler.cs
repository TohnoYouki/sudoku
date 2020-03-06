using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class OddEvenRuler : GlobeRuler
    {   //保证特定数字格子的结果值为偶数或奇数
        private SolutionNumber number;
        public int remainder;
        public OddEvenRuler(SolutionNumber n, bool odd)
        {
            Scope.Clear();
            Scope.Add(n);
            number = n;
            if (odd) remainder = 0; else remainder = 1;
        }

        public override void Initial()
        {
            return;
        }
        //奇偶规则因为其特殊性，只会在数独求解器一开始运行时产生候选值去除操作
        //即去除所有的奇数候选值或偶数候选值。
        public override List<Operation> Implement()
        {
            result.Clear();
            for (int i = 1; i <= number.MaxNumber; i++)
                if (i % 2 == remainder && number.resultRange[i])
                    result.Add(new Operation(number, i));
            return result;
        }
    }
}
