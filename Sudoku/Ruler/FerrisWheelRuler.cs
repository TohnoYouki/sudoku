using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    class FerrisWheelRuler : GlobeRuler
    {      //保证从第一个数字格子往后看到的摩天轮数为定值
        public int FerrisWheelNumber; 
        public FerrisWheelRuler(List<SolutionNumber> scope, int ferrisWheelNumber)
        {
            Scope.Clear();
            foreach (SolutionNumber number in scope)
                Scope.Add(number);
            FerrisWheelNumber = ferrisWheelNumber;
        }

        public override void Initial()
        {
            for (int i = 0; i < Scope.Count; i++)
            {
                List<List<bool>> x = new List<List<bool>>();
                List<List<bool>> y = new List<List<bool>>();
                FrontArray.Add(x);
                IfOk.Add(y);
                for (int j = 0; j <= Math.Min(i + 1, FerrisWheelNumber); j++)
                {
                    List<bool> m = new List<bool>();
                    List<bool> n = new List<bool>();
                    x.Add(m);
                    y.Add(n);
                    for (int k = 0; k <= Scope[i].MaxNumber; k++)
                    {
                        m.Add(false);
                        n.Add(false);
                    }
                }
            }
            for (int i = 0; i < Scope.Count; i++)
            {
                List<bool> x = new List<bool>();
                for (int j = 0; j <= Scope[i].MaxNumber; j++)
                    x.Add(false);
                IfExist.Add(x);
            }
        }

        private List<List<List<bool>>> FrontArray = new List<List<List<bool>>>();
        private List<List<List<bool>>> IfOk = new List<List<List<bool>>>();
        private List<List<bool>> IfExist = new List<List<bool>>();
        //该规则保证按顺序排列的数字格子满足摩天轮规则。
        //摩天轮规则指的是数字代表高度，大数字会挡住小数字
        //而摩天轮数表示的是从第一个格子开始能看到的数字数目
        //该规则的实现最为复杂，基本的算法采用动态规划
        //我们设Front（i，h，k）表示是否存在一种数字安排方式
        //使包含0到i数字格子中满足摩天轮数为h，且最大的数为k
        //而Back（i，h，k）表示是否存在一种数字安排方式
        //使得在k数上看第i个格子到最后一个格子满足摩天轮数为h。
        public override List<Operation> Implement()
        {
            result.Clear();
            for (int i = 0; i < Scope.Count; i++)
                for (int j = 1; j <= Math.Min(i + 1, FerrisWheelNumber); j++)
                    for (int k = 1; k <= Scope[i].MaxNumber; k++)
                        FrontArray[i][j][k] = IfOk[i][j][k] = IfExist[i][k] = false;


            for (int i = 0; i < Scope.Count; i++)
                for (int j = 1; j <= Math.Min(i + 1, FerrisWheelNumber); j++)
                    if (i == 0)
                    {
                        for (int k = 1; k <= Scope[i].MaxNumber; k++)
                            if (Scope[i].resultRange[k]) FrontArray[i][j][k] = true;
                    }
                    else
                    {
                        int k;
                        if (j > 1)
                        {
                            for (k = 1; k <= Scope[i - 1].MaxNumber; k++) if (FrontArray[i - 1][j - 1][k]) break;
                            for (int n = k + 1; n <= Scope[i].MaxNumber; n++)
                                if (Scope[i].resultRange[n]) FrontArray[i][j][n] = true;
                        }
                        if (j <= i)
                        {
                            k = Scope[i].MinPossible();
                            for (int n = k + 1; n <= Scope[i - 1].MaxNumber; n++) if (FrontArray[i - 1][j][n]) FrontArray[i][j][n] = true;
                        }
                    }

            for (int k = 1; k <= Scope[Scope.Count - 1].MaxNumber; k++)
                if (FrontArray[Scope.Count - 1][FerrisWheelNumber][k]) IfOk[Scope.Count - 1][FerrisWheelNumber][k] = true;


            for (int i = Scope.Count - 1; i >= 0; i--)
                for (int j = 1; j <= Math.Min(i + 1, FerrisWheelNumber); j++)
                    if (i - 1 >= 0)
                    {
                        int k, n;
                        if (j > 1)
                        {
                            for (k = 1; k <= Scope[i - 1].MaxNumber; k++) if (FrontArray[i - 1][j - 1][k]) break;
                            for (n = k + 1; n <= Scope[i].MaxNumber; n++)
                                if (Scope[i].resultRange[n] && IfOk[i][j][n]) IfExist[i][n] = true;

                            for (n = Scope[i].MaxNumber; n >= 1; n--) if (Scope[i].resultRange[n] && IfOk[i][j][n]) break;
                            for (k = 1; k < n; k++) if (FrontArray[i - 1][j - 1][k]) IfOk[i - 1][j - 1][k] = true;
                        }
                        if (j <= i)
                        {
                            for (k = Scope[i].MaxNumber; k >= 1; k--) if (IfOk[i][j][k] && FrontArray[i - 1][j][k]) break;
                            for (n = k - 1; n >= 1; n--) IfExist[i][n] = true;

                            n = Scope[i].MinPossible();
                            for (k = Scope[i].MaxNumber; k >= n + 1; k--) if (IfOk[i][j][k] && FrontArray[i - 1][j][k]) IfOk[i - 1][j][k] = true;
                        }
                    }
                    else
                        for (int k = 1; k <= Scope[0].MaxNumber; k++) if (IfOk[i][j][k]) IfExist[i][k] = true;
            for (int i = 0; i < Scope.Count; i++)
                for (int j = 1; j <= Scope[i].MaxNumber; j++)
                    if (!IfExist[i][j] && Scope[i].resultRange[j])
                        result.Add(new Operation(Scope[i], j));
            return result;
        }
    }
}
