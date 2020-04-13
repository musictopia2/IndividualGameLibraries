using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Rummy500CP.Data
{
    [SingletonGame]
    public class Rummy500DetailClass : IGameInfo, ICardInfo<RegularRummyCard>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "Rummy 500"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 5; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like had to use landscape after all.

        int ICardInfo<RegularRummyCard>.CardsToPassOut => 7; //change to what you need.

        CustomBasicList<int> ICardInfo<RegularRummyCard>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<RegularRummyCard>.AddToDiscardAtBeginning => true;

        bool ICardInfo<RegularRummyCard>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<RegularRummyCard>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<RegularRummyCard>.PassOutAll => false;

        bool ICardInfo<RegularRummyCard>.PlayerGetsCards => true;

        bool ICardInfo<RegularRummyCard>.NoPass => false;

        bool ICardInfo<RegularRummyCard>.NeedsDummyHand => false;

        DeckObservableDict<RegularRummyCard> ICardInfo<RegularRummyCard>.DummyHand { get; set; } = new DeckObservableDict<RegularRummyCard>();

        bool ICardInfo<RegularRummyCard>.HasDrawAnimation => true;

        bool ICardInfo<RegularRummyCard>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<RegularRummyCard>.DiscardExcludeList(IListShuffler<RegularRummyCard> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}