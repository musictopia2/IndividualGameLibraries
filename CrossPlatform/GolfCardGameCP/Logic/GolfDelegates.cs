using BasicGameFrameworkLibrary.Attributes;
using System;
using System.Threading.Tasks;

namespace GolfCardGameCP.Logic
{
    [SingletonGame]
    public class GolfDelegates
    {
        internal  Func<Task>? LoadMainScreenAsync { get; set; }
    }
}