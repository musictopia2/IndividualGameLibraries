using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;

namespace FourSuitRummyCP.Data
{
    public class SetInfo : SetInfo<EnumSuitList, EnumColorList, RegularRummyCard, SavedSet>
    {
        public SetInfo(CommandContainer command) : base(command) { }
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