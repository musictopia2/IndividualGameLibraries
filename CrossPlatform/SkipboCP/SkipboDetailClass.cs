using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SkipboCP
{
    [SingletonGame]
    public class SkipboDetailClass : IGameInfo, ICardInfo<SkipboCardInformation>
    {
        private readonly SkipboMainGameClass _mainGame;
        public SkipboDetailClass(SkipboMainGameClass mainGame)
        {
            _mainGame = mainGame; //since i don't have to worry about overflow issues anymore because of the changes made.
        }
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame;

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "SkipBo";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //maybe this could be portrait (?)

        int ICardInfo<SkipboCardInformation>.CardsToPassOut
        {
            get
            {
                if (_mainGame.PlayerList!.Count == 2)
                    return 30;
                return 20;
            }
        }

        CustomBasicList<int> ICardInfo<SkipboCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<SkipboCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<SkipboCardInformation>.ReshuffleAllCardsFromDiscard => true; //we for sure need to reshuffle all cards from discard.

        bool ICardInfo<SkipboCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<SkipboCardInformation>.PassOutAll => false;

        bool ICardInfo<SkipboCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<SkipboCardInformation>.NoPass => false;

        bool ICardInfo<SkipboCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<SkipboCardInformation> ICardInfo<SkipboCardInformation>.DummyHand { get; set; } = new DeckObservableDict<SkipboCardInformation>();

        bool ICardInfo<SkipboCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<SkipboCardInformation>.CanSortCardsToBeginWith => false; //i think this would work so no sorting at first.
    }
}