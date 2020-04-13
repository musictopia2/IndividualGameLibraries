using BasicGameFrameworkLibrary.Attributes;
using System;

namespace SkipboCP.Logic
{
    [SingletonGame]
    public class SkipboDelegates
    {
        public Func<int>? GetPlayerCount { get; set; }
    }
}
