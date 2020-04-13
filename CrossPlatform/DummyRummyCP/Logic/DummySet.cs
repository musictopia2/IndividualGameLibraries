using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using DummyRummyCP.Data;

namespace DummyRummyCP.Logic
{
    public class DummySet : SetInfo<EnumSuitList, EnumColorList, RegularRummyCard, SavedSet>
    {
        public DummySet(CommandContainer command) : base(command) { }
        public override void LoadSet(SavedSet payLoad)
        {
            HandList.ReplaceRange(payLoad.CardList);
        }
        public override SavedSet SavedSet()
        {
            SavedSet output = new SavedSet();
            output.CardList = HandList.ToRegularDeckDict();
            return output;
        }
    }
}