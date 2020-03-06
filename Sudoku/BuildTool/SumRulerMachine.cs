using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Sudoku
{
    class CombineSumRuler
    {
        public SumRuler mainRuler;
        public List<CombineSumRuler> neighbours;
    }

    class SumRulerMachine
    {
        private Dictionary<SolutionNumber, int> resultSet;
        public SumRulerMachine(Dictionary<SolutionNumber, int> result)
        {
            resultSet = result;
        }

        public List<CombineSumRuler> sumRulers = new List<CombineSumRuler>();

        public void AddSingleSumRuler(Dictionary<IntVector2, SumRuler> singleRulers)
        {
            Dictionary<IntVector2, CombineSumRuler> rulers = new Dictionary<IntVector2, CombineSumRuler>();
            foreach (IntVector2 index in singleRulers.Keys)
                rulers.Add(index, new CombineSumRuler());

            for (int i = 1; i < 10; i++)
                for (int j = 1; j < 10; j++)
                {
                    CombineSumRuler y = rulers[new IntVector2(i, j)];
                    sumRulers.Add(y);
                    y.mainRuler = singleRulers[new IntVector2(i, j)];
                    List<CombineSumRuler> x = new List<CombineSumRuler>();
                    y.neighbours = x;
                    if (i > 1) x.Add(rulers[new IntVector2(i - 1, j)]);
                    if (j > 1) x.Add(rulers[new IntVector2(i, j - 1)]);
                    if (j < 9) x.Add(rulers[new IntVector2(i, j + 1)]);
                    if (i < 9) x.Add(rulers[new IntVector2(i + 1, j)]);
                }
        }

        private CombineSumRuler CombineRuler1;
        private CombineSumRuler CombineRuler2;
        //随机选择两个可以合并的求和规则进行合并并返回结果链表
        public List<SumRuler> RandomSelect()
        {
            int number = 0;
            foreach (CombineSumRuler y in sumRulers)
                number += y.neighbours.Count;
            if (number <= 0) return null;
            Random x = new Random();
            int index = x.Next(number) + 1;

            for (int i = 0; i < sumRulers.Count; i++)
                if (index > sumRulers[i].neighbours.Count)
                    index -= sumRulers[i].neighbours.Count;
                else
                {
                    CombineRuler1 = sumRulers[i];
                    CombineRuler2 = sumRulers[i].neighbours[index - 1];
                }
            List<SumRuler> result = new List<SumRuler>();
            foreach (CombineSumRuler ruler in sumRulers)
                if (ruler != CombineRuler1 && ruler != CombineRuler2) result.Add(ruler.mainRuler);
            result.Add(CombineRuler1.mainRuler.Combine(CombineRuler2.mainRuler));
            return result;
        }
        //合并两个求和规则
        public void Combine()
        {
            sumRulers.Remove(CombineRuler1);
            sumRulers.Remove(CombineRuler2);
            foreach (CombineSumRuler ruler in CombineRuler1.neighbours)
                if (ruler != CombineRuler2) ruler.neighbours.Remove(CombineRuler1);
            foreach (CombineSumRuler ruler in CombineRuler2.neighbours)
                if (ruler != CombineRuler1) ruler.neighbours.Remove(CombineRuler2);
            CombineSumRuler x = new CombineSumRuler();
            x.neighbours = new List<CombineSumRuler>();
            sumRulers.Add(x);
            x.mainRuler = CombineRuler1.mainRuler.Combine(CombineRuler2.mainRuler);
            foreach (CombineSumRuler ruler in CombineRuler1.neighbours)
                if (ruler != CombineRuler2) x.neighbours.Add(ruler);
            foreach (CombineSumRuler ruler in CombineRuler2.neighbours)
                if (ruler != CombineRuler1 && !x.neighbours.Contains(ruler)) x.neighbours.Add(ruler);
            foreach (CombineSumRuler ruler in x.neighbours)
                ruler.neighbours.Add(x);

            List<CombineSumRuler> HaveSameValueList = new List<CombineSumRuler>();
            foreach (CombineSumRuler ruler in x.neighbours)
                if (HaveSameValue(ruler, x))
                    HaveSameValueList.Add(ruler);
            foreach (CombineSumRuler ruler in HaveSameValueList)
                ruler.neighbours.Remove(x);
            foreach (CombineSumRuler ruler in HaveSameValueList)
                x.neighbours.Remove(ruler);
        }
        //标记两个求和规则不能合并
        public void Destroy()
        {
            CombineRuler1.neighbours.Remove(CombineRuler2);
            CombineRuler2.neighbours.Remove(CombineRuler1);
        }
        //判定两个求和规则是否有一样的数字
        private bool HaveSameValue(CombineSumRuler x, CombineSumRuler y)
        {
            bool[] valueSet = new bool[x.mainRuler.Scope[0].MaxNumber + 1];
            for (int i = 0; i < valueSet.Length; i++) valueSet[i] = false;
            foreach (SolutionNumber number in x.mainRuler.Scope)
                valueSet[resultSet[number]] = true;
            foreach (SolutionNumber number in y.mainRuler.Scope)
                if (valueSet[resultSet[number]]) return true;
            return false;
        }
    }
}
