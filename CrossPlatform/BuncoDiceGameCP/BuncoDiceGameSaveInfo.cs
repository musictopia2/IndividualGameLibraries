using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace BuncoDiceGameCP
{
    [SingletonGame]
    public class BuncoDiceGameSaveInfo : ObservableObject
    {
        public PlayerCollection<PlayerItem> PlayerList { get; set; } = new PlayerCollection<PlayerItem>();
        public StatisticsInfo ThisStats { get; set; } = new StatisticsInfo();
        public DiceList<SimpleDice> DiceList { get; set; } = new DiceList<SimpleDice>();//i think
        public PlayOrderClass? PlayOrder; //i think
        public int WhatSet { get; set; }
        public int WhatNumber { get; set; }
        public bool DidHaveBunco { get; set; }
        public bool SameTable { get; set; }
        public int TurnNum { get; set; } //i think this is still needed.
        public bool HadBunco { get; set; }
        public bool HasRolled { get; set; }
    }
}