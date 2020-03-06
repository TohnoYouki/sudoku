using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SumRuler : GlobeRuler
    {   //保证区域中所有数字格子的数字和为定值
        public int Sum;  //数字和
        public SumRuler(List<SolutionNumber> scope, int sum)
        {
            Scope.Clear();
            foreach (SolutionNumber number in scope)
                Scope.Add(number);
            Sum = sum;
        }

        public SumRuler(SolutionNumber number, int sum)
        {
            Scope.Clear();
            Scope.Add(number);
            Sum = sum;
        }

        public override void Initial()
        {
            return;
        }
        //该规则保证一定范围内数字格子的结果和为定值。该规则的具体实现算法如下：对于一个特定的格子
        //求解出范围内除该格子以外的最小和minSum和最大和maxSum，假定规定的求和值为sum
        //则该格子的取值范围为[sum-maxSum,sum-minSum]。由此产生新的候选值去除操作
        //易知通过该规则最终产生的解空间肯定满足数值和为sum。
        public override List<Operation> Implement()
        {
            int minSum = 0;
            int maxSum = 0;
            result.Clear();
            foreach (SolutionNumber number in Scope)
            {
                if (number.None()) { result.Clear(); return result; }
                minSum += number.MinPossible();
                maxSum += number.MaxPossible();
            }
            foreach (SolutionNumber number in Scope)
            {
                int min = number.MinPossible();
                int max = number.MaxPossible();
                for (int i = min; i <= Math.Min(Sum - maxSum + max - 1, max); i++)
                    if (number.resultRange[i])
                        result.Add(new Operation(number, i));
                for (int i = Math.Max(min, Sum - minSum + min + 1); i <= max; i++)
                    if (number.resultRange[i])
                        result.Add(new Operation(number, i));
            }
            return result;
        }

        public SumRuler Combine(SumRuler another)
        {
            List<SolutionNumber> scope = new List<SolutionNumber>();
            foreach (SolutionNumber number in Scope)
                scope.Add(number);
            foreach (SolutionNumber number in another.Scope)
                scope.Add(number);
            SumRuler result = new SumRuler(scope, Sum + another.Sum);
            return result;
        }
    }
}
