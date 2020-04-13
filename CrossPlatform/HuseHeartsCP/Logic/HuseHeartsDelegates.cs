using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using HuseHeartsCP.Cards;
using System;

namespace HuseHeartsCP.Logic
{
    [SingletonGame]
    public class HuseHeartsDelegates
    {
        public Action <DeckObservableDict<HuseHeartsCardInformation>>? SetDummyList{ get;set; }
        public Func<DeckObservableDict<HuseHeartsCardInformation>>? GetDummyList { get; set; }
    }
}