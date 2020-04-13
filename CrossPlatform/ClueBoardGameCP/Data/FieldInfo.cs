using System;
using System.Collections.Generic;
using System.Text;

namespace ClueBoardGameCP.Data
{
    public class FieldInfo
    {
        public Dictionary<int, MoveInfo> Neighbors = new Dictionary<int, MoveInfo>(); // i think its a list of moves (well see)
    }
}