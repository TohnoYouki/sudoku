using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class AutoDecision
    //数独自动求解器，用于给定一个对于数独格子数字的决定后，依照相关的数独规则，尽可能深的化简解空间
    //当发生冲突时，自动进行回溯。当求解失败时能够通知应用程序
    //大致的算法思想为：外部调用通知某个未确定的格子的推测值，然后由数独规则推导出由此而导致的其它候选值的变更
    //不断迭代，直到处理完所有的候选值更改操作。
    {
        private int time = 0;
        public int Time { get { return time; } }   //表示当前求解器所在的阶段，也表示当前搜索树的深度，即推测格子的数量
        private List<IUniqueElement> solutionNumbers;   //当前数独的所有格子

        public Stack<Operation> PendingOperation = new Stack<Operation>();  //所有等待操作的候选值去除操作
        public UniqueStack PendingGlobeRulers;   //所有需要更新的全局数独规则

        private Stack<Operation> decision = new Stack<Operation>();   //表示目前所有推测的格子
        private List<UniqueStack> TolDecision = new List<UniqueStack>(); //表示求解器各个阶段所修改的数字格子

        private List<IncrementalRuler> incrementalRulers;  //求解器所有的局部数独规则
        private List<GlobeRuler> globeRulers;   //求解器所有的全局数独规则

        public AutoDecision(List<IncrementalRuler> initialIncrementalRulers, List<GlobeRuler> initialGlobeRulers, List<SolutionNumber> initialSolutionNumber)
        {
            incrementalRulers = initialIncrementalRulers;
            globeRulers = initialGlobeRulers;

            List<IUniqueElement> Globe = new List<IUniqueElement>();
            foreach (GlobeRuler ruler in globeRulers) Globe.Add(ruler);
            PendingGlobeRulers = new UniqueStack(Globe);
            foreach (GlobeRuler ruler in globeRulers) PendingGlobeRulers.Push(ruler);

            solutionNumbers = new List<IUniqueElement>();
            foreach (SolutionNumber number in initialSolutionNumber)
            {
                solutionNumbers.Add(number);
                number.decision = this;
            }
            TolDecision.Add(new UniqueStack(solutionNumbers));
        }

        public bool GoOneStepBack()    //回到上一个阶段，若当前阶段为0则返回false，表示回溯失败
                                       //Time减1，将当前阶段修改过的数字格子恢复到上一个阶段的状态
        {
            if (time == 0) return false;
            UniqueStack x = TolDecision[time];
            time--;
            while (x.Count != 0) (x.Pop() as SolutionNumber).GoBack();
            return true;
        }

        public void AddNumber(Operation operation)  //数字格子候选值发生变化时通知求解器的接口函数
        {
            for (int i = TolDecision.Count; i < time + 1; i++)
                TolDecision.Add(new UniqueStack(solutionNumbers));
            TolDecision[time].Push(operation.number);    //将该数字格子加入到TolDecision中，表示当前阶段发生变化的数字格子。
            foreach (IncrementalRuler ruler in operation.number.incrementalRulers)
            {
                List<Operation> x = ruler.Implement(operation);
                foreach (Operation y in x) PendingOperation.Push(y);  //更新包含该格子的所有局部数独，根据规则生成待操作的候选值去除操作
            }
            foreach (GlobeRuler ruler in operation.number.globeRulers) PendingGlobeRulers.Push(ruler);
             //将包含该格子的所有全局数独规则压入待处理的队列中
        }

        public bool GuessValue(SolutionNumber target, int value)
        { //告诉求解器推测target格子的值为value,让求解器自动推导化简数独格局，Time加1
            time++;
            decision.Push(new Operation(target, value));
            if (TrySetValue(target, 0, value)) return true; else return GoBack();
        }

        public bool SetValue(SolutionNumber target, int index, int value)
        {
            if (TrySetValue(target, index, value)) return true; else return GoBack();
        }

        private bool HandlePendingOperation() //迭代处理所有的候选值去除操作，一旦某个候选值去除操作完成后
        {                            //用数独规则更新生成新的候选值去除操作，加入待处理队列中，迭代更新直到所有操作处理完毕
            while (PendingGlobeRulers.Count != 0 || PendingOperation.Count != 0)
            {
                while (PendingOperation.Count != 0)
                {
                    Operation x = PendingOperation.Pop();
                    x.number.SetRangeFalse(x.index);
                    if (x.number.None()) { PendingOperation.Clear(); return false; }
                }
                while (PendingGlobeRulers.Count != 0)
                {
                    GlobeRuler x = PendingGlobeRulers.Pop() as GlobeRuler;
                    List<Operation> y = x.Implement();
                    foreach (Operation z in y)
                        PendingOperation.Push(z);
                }
            }
            PendingOperation.Clear();
            return true;
        }

        public bool TrySetValue(SolutionNumber target, int index, int value)
        {    // 尝试去除一个数字格子的特定候选值，如果发生矛盾则返回false
            if (target != null)
            {
                if (index == 0)
                    target.SetResult(value);
                else target.SetRangeFalse(index);
                if (target.None()) return false; 
            }
            return HandlePendingOperation();
        }

        public SolutionNumber popNumber = null;
        public bool GoBack()
        {  //一直回溯直到当前推测格子还有候选值，如果回溯到根节点且根节点没有候选值，则返回false表示无解
            while (GoOneStepBack())
            {
                Operation x = decision.Pop();
                if (TrySetValue(x.number, x.index, 0))
                {
                    popNumber = x.number;
                    return true;
                }
            }
            return false;
        }
    }
}
