using BasicGameFramework.Attributes;
using System.Collections.Generic;
namespace ConnectTheDotsCP
{
    [SingletonGame]
    public class GlobalVariableClass
    {
        internal Dictionary<int, SquareInfo>? SquareList;
        internal Dictionary<int, LineInfo>? LineList;
        internal Dictionary<int, DotInfo>? DotList;
        internal LineInfo PreviousLine = new LineInfo();
        internal DotInfo PreviousDot = new DotInfo();
    }
}