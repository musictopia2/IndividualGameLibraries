using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace RackoCP
{
    [SingletonGame]
    public class RackoDetailClass : IGameInfo, ICardInfo<RackoCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => true; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Racko"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //decided to not risk phone because of the scoreboard.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait;

        int ICardInfo<RackoCardInformation>.CardsToPassOut => 10;

        CustomBasicList<int> ICardInfo<RackoCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<RackoCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<RackoCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<RackoCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<RackoCardInformation>.PassOutAll => false;

        bool ICardInfo<RackoCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<RackoCardInformation>.NoPass => false;

        bool ICardInfo<RackoCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<RackoCardInformation> ICardInfo<RackoCardInformation>.DummyHand { get; set; } = new DeckObservableDict<RackoCardInformation>();

        bool ICardInfo<RackoCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<RackoCardInformation>.CanSortCardsToBeginWith => false; //i think this time it can't sort the cards.
    }
}