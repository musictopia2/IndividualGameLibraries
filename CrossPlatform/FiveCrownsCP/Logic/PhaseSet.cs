using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Data;

namespace FiveCrownsCP.Logic
{
    public class PhaseSet : SetInfo<EnumSuitList, EnumColorList, FiveCrownsCardInformation, SavedSet>
    {
        public PhaseSet(CommandContainer command) : base(command) { }

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