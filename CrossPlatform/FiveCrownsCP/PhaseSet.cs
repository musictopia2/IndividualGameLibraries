using BasicGameFramework.Extensions;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
namespace FiveCrownsCP
{
    public class PhaseSet : SetInfo<EnumSuitList, EnumColorList, FiveCrownsCardInformation, SavedSet>
    {
        public PhaseSet(IBasicGameVM thisMod) : base(thisMod) { }

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