using BasicGameFrameworkLibrary.Attributes;
using System;

namespace ChinazoCP.Logic
{
    [SingletonGame]
    public class ChinazoDelegates
    {
        internal Func<int>? CardsToPassOut { get; set; }
    }
}
