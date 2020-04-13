using A8RoundRummyCP.Cards;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;

namespace A8RoundRummyCP.Data
{
    [SingletonGame]
    public class A8RoundRummyDetailClass : IGameInfo, ICardInfo<A8RoundRummyCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "8 Round Rummy"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<A8RoundRummyCardInformation>.CardsToPassOut => 7;

        CustomBasicList<int> ICardInfo<A8RoundRummyCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<A8RoundRummyCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<A8RoundRummyCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<A8RoundRummyCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<A8RoundRummyCardInformation>.PassOutAll => false;

        bool ICardInfo<A8RoundRummyCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<A8RoundRummyCardInformation>.NoPass => false;

        bool ICardInfo<A8RoundRummyCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<A8RoundRummyCardInformation> ICardInfo<A8RoundRummyCardInformation>.DummyHand { get; set; } = new DeckObservableDict<A8RoundRummyCardInformation>();

        bool ICardInfo<A8RoundRummyCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<A8RoundRummyCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<A8RoundRummyCardInformation>.DiscardExcludeList(IListShuffler<A8RoundRummyCardInformation> deckList)
        {
            return deckList.Where(x => x.CardType == EnumCardType.Reverse).Select(x => x.Deck).ToCustomBasicList();
        }
    }
}