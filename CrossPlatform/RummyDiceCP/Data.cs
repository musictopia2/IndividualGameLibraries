using CommonBasicStandardLibraries.CollectionClasses;
namespace RummyDiceCP
{
    public class SendSet
    {
        public int WhichSet { get; set; }
        public int Dice { get; set; }
    }
    public class SetInfo
    {
        public EnumWhatSets SetType;
        public bool HasLaid;
        public bool DidSucceed;
        public int HowMany;
        public CustomBasicList<RummyDiceInfo>? SetCards;
    }
    public class PhaseList
    {
        public string Description = "";
        public CustomBasicList<SetInfo> PhaseSets = new CustomBasicList<SetInfo>();
    }
}
