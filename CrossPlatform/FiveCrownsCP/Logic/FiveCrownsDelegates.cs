using BasicGameFrameworkLibrary.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiveCrownsCP.Logic
{
    [SingletonGame]
    public class FiveCrownsDelegates
    {
        public Func<int>? CardsToPassOut { get; set; }
    }
}
