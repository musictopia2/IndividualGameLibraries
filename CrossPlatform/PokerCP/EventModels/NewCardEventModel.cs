using System;
using System.Collections.Generic;
using System.Text;

namespace PokerCP.EventModels
{
    public class NewCardEventModel
    {
        public int Index { get; set; }
        public NewCardEventModel(int index)
        {
            Index = index;
        }
    }
}
