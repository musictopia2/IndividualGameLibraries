using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using Phase10CP.Cards;
using Phase10CP.Data;

namespace Phase10CP.SetClasses
{
    public class SetInfo
    {
        public EnumWhatSets SetType;
        public bool HasLaid;
        public bool DidSucceed;
        public int HowMany;
        public DeckRegularDict<Phase10CardInformation>? SetCards;
    }
}