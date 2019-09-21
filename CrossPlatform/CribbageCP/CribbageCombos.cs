using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace CribbageCP
{
    public class CribbageCombos
    {
        public string Description = "";
        public EnumPlayType WhenUsed;
        public int NumberNeeded;
        public int CardsToUse;
        public int NumberInStraight;
        public bool IsFlush;
        public int NumberForKind;
        public EnumCribbagEequals WhatEqual;
        public int Points;
        public bool DoublePairNeeded;
        public EnumScoreGroups Group;
        public bool JackStarter;
        public bool JackSameSuitAsStarter;
        public CustomBasicList<RegularSimpleCard> Hand = new CustomBasicList<RegularSimpleCard>();
    }
}