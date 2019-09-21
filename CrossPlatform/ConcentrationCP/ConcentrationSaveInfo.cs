using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;

namespace ConcentrationCP
{
    [SingletonGame]
    public class ConcentrationSaveInfo : BasicSavedCardClass<ConcentrationPlayerItem, RegularSimpleCard>
    { //anything needed for autoresume is here.
        public CustomBasicList<BasicPileInfo<RegularSimpleCard>> BoardList { get; set; } = new CustomBasicList<BasicPileInfo<RegularSimpleCard>>();
        public DeckRegularDict<RegularSimpleCard> ComputerList { get; set; } = new DeckRegularDict<RegularSimpleCard>();
    }
}