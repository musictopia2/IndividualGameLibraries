using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace HuseHeartsCP
{
    [SingletonGame]
    public class HuseHeartsDetailClass : IGameInfo, ICardInfo<HuseHeartsCardInformation>, ITrickData
    {
        public HuseHeartsDetailClass(HuseHeartsMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Huse Hearts"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //decided to risk not even allowing phones to play this one.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<HuseHeartsCardInformation>.CardsToPassOut => 16;

        CustomBasicList<int> ICardInfo<HuseHeartsCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<HuseHeartsCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<HuseHeartsCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<HuseHeartsCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<HuseHeartsCardInformation>.PassOutAll => false;

        bool ICardInfo<HuseHeartsCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<HuseHeartsCardInformation>.NoPass => false;

        bool ICardInfo<HuseHeartsCardInformation>.NeedsDummyHand => true;
        private readonly HuseHeartsMainGameClass _mainGame; //no more overflow issues anymore.
        DeckObservableDict<HuseHeartsCardInformation> ICardInfo<HuseHeartsCardInformation>.DummyHand
        {
            get => _mainGame.SaveRoot!.DummyList;
            set => _mainGame.SaveRoot!.DummyList = value;
        }
        bool ICardInfo<HuseHeartsCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<HuseHeartsCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.Hearts;

        bool ITrickData.HasDummy => true;
    }
}