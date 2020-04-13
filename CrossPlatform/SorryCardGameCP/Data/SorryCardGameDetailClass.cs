using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using SorryCardGameCP.Cards;
namespace SorryCardGameCP.Data
{
    [SingletonGame]
    public class SorryCardGameDetailClass : IGameInfo, ICardInfo<SorryCardGameCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Sorry Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<SorryCardGameCardInformation>.CardsToPassOut => 5;

        CustomBasicList<int> ICardInfo<SorryCardGameCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<SorryCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<SorryCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<SorryCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<SorryCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<SorryCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<SorryCardGameCardInformation>.NoPass => false;

        bool ICardInfo<SorryCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<SorryCardGameCardInformation> ICardInfo<SorryCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<SorryCardGameCardInformation>();

        bool ICardInfo<SorryCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<SorryCardGameCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<SorryCardGameCardInformation>.DiscardExcludeList(IListShuffler<SorryCardGameCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}