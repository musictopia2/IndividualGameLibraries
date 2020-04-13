using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using YahtzeeHandsDownCP.Cards;
namespace YahtzeeHandsDownCP.Data
{
    [SingletonGame]
    public class YahtzeeHandsDownDetailClass : IGameInfo, ICardInfo<YahtzeeHandsDownCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "Yahtzee Hands Down"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //decided no phone because too many details are needed

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<YahtzeeHandsDownCardInformation>.CardsToPassOut => 5;

        CustomBasicList<int> ICardInfo<YahtzeeHandsDownCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<YahtzeeHandsDownCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<YahtzeeHandsDownCardInformation>.ReshuffleAllCardsFromDiscard => true; //maybe this is it.

        bool ICardInfo<YahtzeeHandsDownCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<YahtzeeHandsDownCardInformation>.PassOutAll => false;

        bool ICardInfo<YahtzeeHandsDownCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<YahtzeeHandsDownCardInformation>.NoPass => false;

        bool ICardInfo<YahtzeeHandsDownCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<YahtzeeHandsDownCardInformation> ICardInfo<YahtzeeHandsDownCardInformation>.DummyHand { get; set; } = new DeckObservableDict<YahtzeeHandsDownCardInformation>();

        bool ICardInfo<YahtzeeHandsDownCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<YahtzeeHandsDownCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<YahtzeeHandsDownCardInformation>.DiscardExcludeList(IListShuffler<YahtzeeHandsDownCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}