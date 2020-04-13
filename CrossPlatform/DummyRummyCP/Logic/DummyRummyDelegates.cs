using BasicGameFrameworkLibrary.Attributes;
using System;

namespace DummyRummyCP.Logic
{
    [SingletonGame]
    public class DummyRummyDelegates
    {
        internal Func<int>? CardsToPassOut { get; set; }
    }
}
