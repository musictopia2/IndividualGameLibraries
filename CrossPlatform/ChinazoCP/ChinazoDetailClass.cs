using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace ChinazoCP
{
    [SingletonGame]
    public class ChinazoDetailClass : IGameInfo, ICardInfo<ChinazoCard>
    {
        private readonly ChinazoMainGameClass? _mainGame;
        public ChinazoDetailClass(ChinazoMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Chinazo"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<ChinazoCard>.CardsToPassOut => _mainGame!.CardsToPassOut; //change to what you need.

        CustomBasicList<int> ICardInfo<ChinazoCard>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<ChinazoCard>.AddToDiscardAtBeginning => true;

        bool ICardInfo<ChinazoCard>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<ChinazoCard>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<ChinazoCard>.PassOutAll => false;

        bool ICardInfo<ChinazoCard>.PlayerGetsCards => true;

        bool ICardInfo<ChinazoCard>.NoPass => false;

        bool ICardInfo<ChinazoCard>.NeedsDummyHand => false;

        DeckObservableDict<ChinazoCard> ICardInfo<ChinazoCard>.DummyHand { get; set; } = new DeckObservableDict<ChinazoCard>();

        bool ICardInfo<ChinazoCard>.HasDrawAnimation => true;

        bool ICardInfo<ChinazoCard>.CanSortCardsToBeginWith => true;
    }
}