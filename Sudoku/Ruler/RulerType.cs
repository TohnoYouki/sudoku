using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    //用于表示数独的规则，分为局部数独和全局数独
    //局部数独可以根据数独的局部信息推导出对应导致的候选值去除操作，而全局数独则需要根据数独的全局状态才能推导出候选者去除操作
    //因此在求解器的每次迭代过程中，只有当速度比较快的局部规则迭代更新完毕后，才进行全局数独规则的迭代更新。
    public abstract class IncrementalRuler : Ruler
    {
        abstract public List<Operation> Implement(Operation operation);  //根据候选者恢复操作维护规则中的数据成员
        abstract public void GoBack(Operation operation);
        //根据提供的候选者去除操作和规则生成由此导致的新候选者去除操作
    }

    public abstract class GlobeRuler : Ruler, IUniqueElement
    {
        abstract public List<Operation> Implement();   //根据数独当前的全局信息生成对应的新候选者去除操作
        public int Index { get; set; }
    }
}
