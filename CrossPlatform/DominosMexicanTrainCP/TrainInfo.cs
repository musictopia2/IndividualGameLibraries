﻿using BasicGameFramework.BasicDrawables.Dictionary;
namespace DominosMexicanTrainCP
{
    public class TrainInfo
    {
        public DeckRegularDict<MexicanDomino> DominoList = new DeckRegularDict<MexicanDomino>();
        public int Index { get; set; }
        public bool TrainUp { get; set; }
        public bool IsPublic { get; set; }
    }
}