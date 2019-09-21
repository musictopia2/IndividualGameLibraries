using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MonasteryCardGameCP
{
    public class MissionList
    {
        public string Description { get; set; } = "";
        public CustomBasicList<SetInfo> MissionSets { get; set; } = new CustomBasicList<SetInfo>();
    }
    public class SetInfo
    {
        public DeckRegularDict<MonasteryCardInfo> SetCards { get; set; } = new DeckRegularDict<MonasteryCardInfo>();
        public EnumWhatSets SetType { get; set; }
        public bool HasLaid { get; set; }
        public bool DidSucceed { get; set; }
        public int HowMany { get; set; }
    }
    public class SendExpandSet
    {
        public int SetNumber { get; set; }
        public int Position { get; set; }
        public string CardData { get; set; } = "";
    }
    public class SendNewSet
    {
        public int Index { get; set; }
        public string CardData { get; set; } = "";
        public string MissionCompleted { get; set; } = "";
    }
    public struct InstructionInfo
    {
        public int SetNumber { get; set; } // i think
        public int WhichOne { get; set; } // will be 1, 2, 3, etc
    }
}