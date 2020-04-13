using BasicGameFrameworkLibrary.Attributes;
using System;

namespace GalaxyCardGameCP.Logic
{
    [SingletonGame]
    public class GalaxyDelegates
    {
        internal Func<bool>? PlayerGetCards { get; set; }
    }
}
