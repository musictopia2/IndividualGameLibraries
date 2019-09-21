using BasicGameFramework.Extensions;
using BasicGameFramework.RegularDeckOfCards;
using BasicGameFramework.SpecializedGameTypes.RummyClasses;
using BasicGameFramework.ViewModelInterfaces;
namespace FourSuitRummyCP
{
    public class SetInfo : SetInfo<EnumSuitList, EnumColorList, RegularRummyCard, SavedSet>
    {
        public SetInfo(IBasicGameVM thisMod) : base(thisMod) { }
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