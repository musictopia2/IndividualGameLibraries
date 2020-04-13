using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using FiveCrownsCP.Cards;
using FiveCrownsCP.Logic;
namespace FiveCrownsCP.Data
{
    [SingletonGame]
    public class FiveCrownsDetailClass : IGameInfo, ICardInfo<FiveCrownsCardInformation>
    {
        private readonly FiveCrownsDelegates _delegates;

        public FiveCrownsDetailClass(FiveCrownsDelegates delegates)
        {
            _delegates = delegates;
        }
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "Five Crowns"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 7; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<FiveCrownsCardInformation>.CardsToPassOut => _delegates.CardsToPassOut!.Invoke(); //change to what you need.

        CustomBasicList<int> ICardInfo<FiveCrownsCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<FiveCrownsCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<FiveCrownsCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<FiveCrownsCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<FiveCrownsCardInformation>.PassOutAll => false;

        bool ICardInfo<FiveCrownsCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<FiveCrownsCardInformation>.NoPass => false;

        bool ICardInfo<FiveCrownsCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<FiveCrownsCardInformation> ICardInfo<FiveCrownsCardInformation>.DummyHand { get; set; } = new DeckObservableDict<FiveCrownsCardInformation>();

        bool ICardInfo<FiveCrownsCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<FiveCrownsCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<FiveCrownsCardInformation>.DiscardExcludeList(IListShuffler<FiveCrownsCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}