using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class NotRepeatFullRuler : IncrementalRuler
    { //保证包含的数字格子里没有重复的数
        private int maxNumber = 0;
        private int[] rangeNumber;  //当前候选值的剩余数量

        public NotRepeatFullRuler(List<SolutionNumber> scope)
        {
            Scope.Clear();
            foreach (SolutionNumber number in scope)
                Scope.Add(number);
            maxNumber = Scope.Count;
            rangeNumber = new int[maxNumber + 1];
            for (int i = 1; i <= maxNumber; i++)
                rangeNumber[i] = maxNumber;
        }

        public override void Initial()
        {
            for (int i = 1; i <= maxNumber; i++)
                rangeNumber[i] = maxNumber;
        }

        public override void GoBack(Operation operation)
        {
            rangeNumber[operation.index]++;
        }

        //当范围内的某个数字格子的数字确定以后，将范围内其它格子当中对应的候选值去除。
        //因为一般数独规则有其特殊性：每个数字格子所能允许的候选者数量和不重复规则包含的格子数目相同。
        //因此不重复规则判断当范围内的格子数目等于候选者数量时，
        //如果有一个候选值只有一个格子拥有，则可以判断这个格子便为该候选值，这被称为唯一候选值法。
        public override List<Operation> Implement(Operation operation)
        {
            result.Clear();
            if (operation.number != null)
            {
                if (operation.number.PossibleNumber() == 1)
                {
                    int value = operation.number.RandomSelect();
                    foreach (SolutionNumber x in Scope)
                        if (x != operation.number && x.resultRange[value])
                            result.Add(new Operation(x, value));
                }
                rangeNumber[operation.index]--;
                if (rangeNumber[operation.index] == 1)
                    foreach (SolutionNumber x in Scope)
                        if (x.resultRange[operation.index])
                            SetValue(x, operation.index);
                if (rangeNumber[operation.index] == 0)
                    SetValue(Scope[0], 0);
            }
            return result;
        }
    }
}
