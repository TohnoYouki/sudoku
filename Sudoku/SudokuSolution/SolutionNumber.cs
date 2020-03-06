using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class SolutionNumber : IUniqueElement   //用于表示数独单个格子里面的候选数字，封装了数独求解器求解时
    {                                              //对单个格子所需要的所有操作
        public int Index { get; set; }
        public int MaxNumber = 9;

        public List<IncrementalRuler> incrementalRulers = new List<IncrementalRuler>();
        //存储拥有该格子的所有局部数独规则，属于十字链表数据结构的一部分，用于格子候选值发生改变时，快速通知所有拥有该格子的规则更新数据
        public List<GlobeRuler> globeRulers = new List<GlobeRuler>(); // 存储拥有该格子的所有全局数独规则

        public bool[] resultRange;     //当前该格子的取值，为真表示可以取，为假表示不能取

        private int possibleNumber;    // 表示当前格子候选值的个数，为0时表示当前格子没有候选值，求解遇到了矛盾，需回溯
                                       //当为1时表示只有一个候选，则当前格子的值便为该候选值，表示求解过程中格子已填入
                                       //当大于1时，表示格子有多个候选值，求解器需要在后续求解过程中确定格子的值
        private Stack<OperationRecord> records = new Stack<OperationRecord>();
        //该格子到目前为止按时间先后顺序发生的去除操作集合,用于后续回溯时将候选值恢复
        public AutoDecision decision;  //该格子所属的求解器

        public SolutionNumber(int max)
        {
            MaxNumber = Math.Max(max, 1);
            possibleNumber = MaxNumber;
            resultRange = new bool[MaxNumber + 1];
            for (int i = 1; i <= MaxNumber; i++)
                resultRange[i] = true;
        }

        public void Initial()   //初始化所有的成员，保证后续求解过程的正确性
        {
            Index = 0;
            for (int i = 1; i <= MaxNumber; i++) resultRange[i] = true;
            possibleNumber = MaxNumber;
            records.Clear();
            incrementalRulers.Clear();
            globeRulers.Clear();
        }

        public void AddRuler(List<Ruler> TolRuler)    //将TolRuler里面所有的Ruler按照incrementRuler和GlobeRuler两种类别加到集合中去
        {
            foreach (Ruler ruler in TolRuler)
                if (ruler is IncrementalRuler)
                    incrementalRulers.Add(ruler as IncrementalRuler);
                else if (ruler is GlobeRuler)
                    globeRulers.Add(ruler as GlobeRuler);
        }

        public void AddRuler(Ruler ruler)   //将TolRuler里面所有的Ruler按照incrementRuler和GlobeRuler两种类别加到集合中去
        {
            if (ruler is IncrementalRuler)
                incrementalRulers.Add(ruler as IncrementalRuler);
            else if (ruler is GlobeRuler)
                globeRulers.Add(ruler as GlobeRuler);
        }

        public void RemoveRuler(Ruler ruler)  //从集合中移去特定的数独规则
        {
            if (ruler is IncrementalRuler)
                incrementalRulers.Remove(ruler as IncrementalRuler);
            else if (ruler is GlobeRuler)
                globeRulers.Remove(ruler as GlobeRuler);
        }

        public bool IfNotDecide()   //返回当前格子是否已经有值，判断标准是possibleNumber>1，即有两个及两个以上的候选值
        {
            return possibleNumber > 1;
        }

        public bool None()   //返回当前格子是否产生矛盾，判断标准是possibleNumber == 0，即是否存在候选值
        {
            return possibleNumber == 0;
        }

        public int PossibleNumber()
        {
            return possibleNumber;
        }

        public int RandomSelect()  //随机从候选值中返回一个，用于求解器的赋值搜索操作
        {
            int result = 0;
            ContinusRandomSelect x = new ContinusRandomSelect();
            for (int i = 1; i <= MaxNumber; i++)
                if (resultRange[i] && x.SelectThis())
                    result = i;
            return result;
        }

        public bool SetRangeFalse(int index)  //将候选值index去除，并生成操作记录压栈，并通知求解器
        {
            if (resultRange[index])
            {
                possibleNumber--;
                resultRange[index] = false;
                records.Push(new OperationRecord(index, decision.Time));
                decision.AddNumber(new Operation(this, index));   //通知求解器
                return true;
            }
            return false;
        }

        public bool SetResult(int value)
        {
            bool result = false;
            for (int i = 1; i <= MaxNumber; i++)
                if (i != value && resultRange[i])
                    result = result | SetRangeFalse(i);
            return result;
        }

        public void GoBack()    //回溯，将所有在求解器当前阶段后发生的去除操作还原，并通知所有的局部数独规则
        {
            while (records.Count != 0 && records.Peek().operationTime > decision.Time)
            {
                OperationRecord x = records.Pop();
                resultRange[x.operationIndex] = true;
                foreach (IncrementalRuler ruler in incrementalRulers)
                    ruler.GoBack(new Operation(this, x.operationIndex));
                possibleNumber++;
            }
        }

        public int MinPossible()    //返回当前候选值中最小的一个
        {
            int i;
            for (i = 1; i <= MaxNumber; i++)
                if (resultRange[i]) break;
            return i;
        }

        public int MaxPossible()    //返回当前候选值中最大的一个
        {
            int i;
            for (i = MaxNumber; i >= 1; i--)
                if (resultRange[i]) break;
            return i;
        }
    }
}
