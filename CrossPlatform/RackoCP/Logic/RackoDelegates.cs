using BasicGameFrameworkLibrary.Attributes;
using System;

namespace RackoCP.Logic
{
    [SingletonGame]
    public class RackoDelegates
    {
        internal Func<int>? PlayerCount { get; set; }
    }
}