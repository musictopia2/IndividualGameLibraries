using BasicGameFrameworkLibrary.Attributes;
using System;
using System.Threading.Tasks;

namespace PickelCardGameCP.Logic
{
    [SingletonGame]
    public class PickelDelegates
    {
        public Func<Task>? LoadBiddingAsync { get; set; }
        public Func<Task>? CloseBiddingAsync { get; set; }

    }
}