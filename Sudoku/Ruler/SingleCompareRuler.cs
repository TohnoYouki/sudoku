using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SingleCompareRuler : GlobeRuler
    {    //保证第一个格子数字比第二个格子的数字大
        public SolutionNumber larger;
        public SolutionNumber less;
        public SingleCompareRuler(SolutionNumber higher, SolutionNumber lower)
        {
            Scope.Clear();
            Scope.Add(higher);
            Scope.Add(lower);
            larger = higher;
            less = lower;
        }

        public override void Initial()
        {
            return;
        }
        //该规则的具体实现思路是：因为第一个数字肯定大于第二个数字
        //因此我们将第一个格子中所有小于等于第二个格子中最小候选值的候选值去除
        //将第二个格子中所有大于等于第一个格子中最大候选值的候选值去除。
        public override List<Operation> Implement()
        {
            result.Clear();
            if (!larger.None() && !larger.None())
            {
                int largermax = larger.MaxPossible(), lessmax = less.MaxPossible();
                int largermin = larger.MinPossible(), lessmin = less.MinPossible();
                for (int i = largermax; i <= lessmax; i++)
                    if (less.resultRange[i])
                        result.Add(new Operation(less, i));
                for (int i = largermin; i <= lessmin; i++)
                    if (larger.resultRange[i])
                        result.Add(new Operation(larger, i));
            }
            return result;
        }
    }
}
