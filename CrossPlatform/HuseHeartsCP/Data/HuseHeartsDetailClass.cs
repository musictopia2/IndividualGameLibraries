using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using HuseHeartsCP.Cards;
using HuseHeartsCP.Logic;
namespace HuseHeartsCP.Data
{
    [SingletonGame]
    public class HuseHeartsDetailClass : IGameInfo, ICardInfo<HuseHeartsCardInformation>, ITrickData
    {
        private readonly HuseHeartsDelegates _delegates;

        public HuseHeartsDetailClass(HuseHeartsDelegates delegates)
        {
            _delegates = delegates;
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

        CustomBasicList<int> ICardInfo<HuseHeartsCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<HuseHeartsCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<HuseHeartsCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<HuseHeartsCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<HuseHeartsCardInformation>.PassOutAll => false;

        bool ICardInfo<HuseHeartsCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<HuseHeartsCardInformation>.NoPass => false;

        bool ICardInfo<HuseHeartsCardInformation>.NeedsDummyHand => true;
        DeckObservableDict<HuseHeartsCardInformation> ICardInfo<HuseHeartsCardInformation>.DummyHand
        {
            get => _delegates.GetDummyList!.Invoke();
            set => _delegates.SetDummyList!(value);
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

        CustomBasicList<int> ICardInfo<HuseHeartsCardInformation>.DiscardExcludeList(IListShuffler<HuseHeartsCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}