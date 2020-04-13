using BasicGameFrameworkLibrary.Attributes;
using System;

namespace CribbageCP.Logic
{
    [SingletonGame]
    public class CribbageDelegates
    {
        public Func<int>? GetPlayerCount { get; set; }
    }
}