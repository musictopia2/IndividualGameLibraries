using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace FiveCrownsCP
{
    [SingletonGame]
    public class FiveCrownsDetailClass : IGameInfo, ICardInfo<FiveCrownsCardInformation>
    {
        private readonly FiveCrownsMainGameClass _mainGame;
        public FiveCrownsDetailClass(FiveCrownsMainGameClass mainGame)
        {
            _mainGame = mainGame;
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

        int ICardInfo<FiveCrownsCardInformation>.CardsToPassOut => _mainGame.CardsToPassOut; //change to what you need.

        CustomBasicList<int> ICardInfo<FiveCrownsCardInformation>.ExcludeList => new CustomBasicList<int>();

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
    }
}