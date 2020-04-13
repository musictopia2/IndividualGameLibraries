using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;

namespace ChinazoCP.Data
{
    public class SendNewSet
    {
        public string CardListData { get; set; } = "";
        public bool UseSecond { get; set; }
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet { get; set; }
    }
}