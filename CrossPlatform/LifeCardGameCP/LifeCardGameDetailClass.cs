using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
namespace LifeCardGameCP
{
    [SingletonGame]
    public class LifeCardGameDetailClass : IGameInfo, ICardInfo<LifeCardGameCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly;

        string IGameInfo.GameName => "Life Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.
        private readonly LifeCardGameMainGameClass _mainGame;
        public LifeCardGameDetailClass(LifeCardGameMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        int ICardInfo<LifeCardGameCardInformation>.CardsToPassOut => 5;
        CustomBasicList<int> ICardInfo<LifeCardGameCardInformation>.ExcludeList
        {
            get
            {
                return _mainGame.YearCards().Select(items => items.Deck).ToCustomBasicList();
            }
        }
        bool ICardInfo<LifeCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<LifeCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<LifeCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<LifeCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<LifeCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<LifeCardGameCardInformation>.NoPass => false;

        bool ICardInfo<LifeCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<LifeCardGameCardInformation> ICardInfo<LifeCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<LifeCardGameCardInformation>();

        bool ICardInfo<LifeCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<LifeCardGameCardInformation>.CanSortCardsToBeginWith => true;
    }
}