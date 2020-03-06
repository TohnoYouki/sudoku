using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public abstract class Ruler //数独规则
    {
        public List<SolutionNumber> Scope = new List<SolutionNumber>(); //当前规则作用的数独格子范围
        public List<Operation> result = new List<Operation>();  //每次更新规则时生成的候选值操作

        abstract public void Initial();  //规则的初始化

        public void SetValue(SolutionNumber number, int value)
        {
            for (int i = 1; i <= number.MaxNumber; i++)
                if (i != value && number.resultRange[i])
                    result.Add(new Operation(number, i));
        }

        public bool Exist(SolutionNumber target)   //判断该规则是否包含target
        {
            foreach (SolutionNumber x in Scope)
                if (x == target)
                    return true;
            return false;
        }

        public SolutionNumber RandomSelect()
        {
            ContinusRandomSelect x = new ContinusRandomSelect();
            SolutionNumber result = null;
            foreach (SolutionNumber number in Scope)
                if (number.IfNotDecide() && x.SelectThis())
                    result = number;
            return result;
        }
    }
}
