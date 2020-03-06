using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class SudokuSolution
    {
        private List<SolutionNumber> solutionNumbers;  //数独求解包括的所有数独格子
        private List<IncrementalRuler> incrementalRulers;  //数独求解器中的所有局部数独规则
        private List<GlobeRuler> globeRulers;  //数独求解器中的所有全局数独规则
        private AutoDecision autoDecision;  //自动求解器
        private int maxTime = -1;
        //表示数独求解所能允许的最大时间，如果超过时间则停止求解并返回无解。如果maxTime等于-1，则不考虑时间限制
        public SudokuSolution(List<SolutionNumber> initialNumbers, List<Ruler> initialRulers, int maxDelay = -1)
        {
            maxTime = maxDelay;
            foreach (SolutionNumber number in initialNumbers)
                number.Initial();
            foreach (Ruler ruler in initialRulers)
                ruler.Initial();
            solutionNumbers = initialNumbers;
            incrementalRulers = new List<IncrementalRuler>();
            globeRulers = new List<GlobeRuler>();
            foreach (Ruler ruler in initialRulers)
                if (ruler is IncrementalRuler)
                    incrementalRulers.Add(ruler as IncrementalRuler);
                else if (ruler is GlobeRuler)
                    globeRulers.Add(ruler as GlobeRuler);
            foreach (Ruler ruler in initialRulers)
                foreach (SolutionNumber number in ruler.Scope)
                    number.AddRuler(ruler);
        }

        public void AddRuler(Ruler ruler)
        {
            ruler.Initial();
            if (ruler is IncrementalRuler)
                incrementalRulers.Add(ruler as IncrementalRuler);
            else if (ruler is GlobeRuler)
                globeRulers.Add(ruler as GlobeRuler);
            foreach (SolutionNumber number in ruler.Scope)
                number.AddRuler(ruler);
        }

        public SolutionNumber GetGuessNumber()  //利用最小候选数原则选择下一个推测值
        {
            SolutionNumber result = null;
            SelectContinusMinNumber x = new SelectContinusMinNumber();
            foreach (SolutionNumber number in solutionNumbers)
                if (number.IfNotDecide() && x.IfMin(number.PossibleNumber()))
                    result = number;
            return result;
        }

        public bool Solution()    //求解数独，返回真表示数独有解，返回假表示数独无解
        {
            autoDecision = new AutoDecision(incrementalRulers, globeRulers, solutionNumbers);
            if (!autoDecision.SetValue(null, 0, 0))
                return false;
            DateTime x = DateTime.Now;
            while (true)
            {
                if (maxTime != -1 && (DateTime.Now - x).TotalSeconds >= maxTime) return false;
                SolutionNumber decision;
                if (autoDecision.popNumber != null && autoDecision.popNumber.IfNotDecide())
                {
                    decision = autoDecision.popNumber;
                    autoDecision.popNumber = null;
                }
                else
                    decision = GetGuessNumber();
                if (decision == null) return true;
                if (!autoDecision.GuessValue(decision, decision.RandomSelect())) return false;
            }
        }

        public bool IfHaveUniqueSolution()   //返回真表示数独仅有唯一解，返回假表示数独没有唯一解
        {
            if (!Solution()) return false;
            if (!autoDecision.GoBack()) return true;
            while (true)
            {
                SolutionNumber decision;
                if (autoDecision.popNumber != null && autoDecision.popNumber.IfNotDecide())
                {
                    decision = autoDecision.popNumber;
                    autoDecision.popNumber = null;
                }
                else
                    decision = GetGuessNumber();
                if (decision == null) return false;
                if (!autoDecision.GuessValue(decision, decision.RandomSelect())) return true;
            }
        }
    }
}
