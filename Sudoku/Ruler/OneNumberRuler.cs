using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class OneNumberRuler : GlobeRuler
    {  //保证特定数字格子为特定的值或不为特定的值
        private SolutionNumber number;
        private int value;
        private bool state;
        public OneNumberRuler(SolutionNumber n, int v, bool s)
        {
            Scope.Clear();
            Scope.Add(n);
            number = n;
            value = v;
            state = s;
        }

        public override void Initial()
        {
            return;
        }
        //该规则保证单个数字格子只能为固定值。单个数字规则因为其特殊性
        //只会在数独求解器一开始运行时产生候选值去除操作，即去除所有不是给定值的候选值。
        public override List<Operation> Implement()
        {
            result.Clear();
            if (state)
            {
                for (int i = 1; i <= number.MaxNumber; i++)
                    if (number.resultRange[i] && i != value)
                        result.Add(new Operation(number, i));
            }
            else if (number.resultRange[value])
                result.Add(new Operation(number, value));
            return result;
        }
    }
}
