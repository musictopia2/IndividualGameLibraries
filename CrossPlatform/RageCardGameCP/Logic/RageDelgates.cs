using BasicGameFrameworkLibrary.Attributes;
using System;
using System.Threading.Tasks;

namespace RageCardGameCP.Logic
{
    [SingletonGame]
    public class RageDelgates
    {
        internal Func<int>? CardsToPassOut { get; set; }
        public Func<Task>? LoadColorScreenAsync { get; set; }
        public Func<Task>? CloseColorScreenAsync { get; set; }
        public Func<Task>? LoadBidScreenAsync { get; set; }
        public Func<Task>? CloseBidScreenAsync { get; set; }
    }
}
