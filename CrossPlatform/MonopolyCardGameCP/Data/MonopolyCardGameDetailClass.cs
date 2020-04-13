using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using MonopolyCardGameCP.Cards;
namespace MonopolyCardGameCP.Data
{
    [SingletonGame]
    public class MonopolyCardGameDetailClass : IGameInfo, ICardInfo<MonopolyCardGameCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "Monopoly Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<MonopolyCardGameCardInformation>.CardsToPassOut => 10;

        CustomBasicList<int> ICardInfo<MonopolyCardGameCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<MonopolyCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<MonopolyCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<MonopolyCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<MonopolyCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<MonopolyCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<MonopolyCardGameCardInformation>.NoPass => false;

        bool ICardInfo<MonopolyCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<MonopolyCardGameCardInformation> ICardInfo<MonopolyCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<MonopolyCardGameCardInformation>();

        bool ICardInfo<MonopolyCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<MonopolyCardGameCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<MonopolyCardGameCardInformation>.DiscardExcludeList(IListShuffler<MonopolyCardGameCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}