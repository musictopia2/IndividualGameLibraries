using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace HorseshoeCardGameCP
{
    [SingletonGame]
    public class HorseshoeCardGameDetailClass : IGameInfo, ICardInfo<HorseshoeCardGameCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Horseshoe"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<HorseshoeCardGameCardInformation>.CardsToPassOut => 6;

        CustomBasicList<int> ICardInfo<HorseshoeCardGameCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<HorseshoeCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<HorseshoeCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<HorseshoeCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<HorseshoeCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<HorseshoeCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<HorseshoeCardGameCardInformation>.NoPass => false;

        bool ICardInfo<HorseshoeCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<HorseshoeCardGameCardInformation> ICardInfo<HorseshoeCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<HorseshoeCardGameCardInformation>();

        bool ICardInfo<HorseshoeCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<HorseshoeCardGameCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => false;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => true;
    }
}