using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;

namespace MonasteryCardGameCP.Data
{
    public class SetInfo
    {
        public DeckRegularDict<MonasteryCardInfo> SetCards { get; set; } = new DeckRegularDict<MonasteryCardInfo>();
        public EnumWhatSets SetType { get; set; }
        public bool HasLaid { get; set; }
        public bool DidSucceed { get; set; }
        public int HowMany { get; set; }
    }
}