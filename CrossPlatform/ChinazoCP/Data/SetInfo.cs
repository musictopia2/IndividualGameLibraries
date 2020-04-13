using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;

namespace ChinazoCP.Data
{
    public class SetInfo
    {
        public bool DidSucceed { get; set; }
        public int HowMany { get; set; }
        public RummyProcesses<EnumSuitList, EnumColorList, ChinazoCard>.EnumRummyType WhatSet { get; set; }
    }
}