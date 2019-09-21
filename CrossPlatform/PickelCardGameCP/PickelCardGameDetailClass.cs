using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace PickelCardGameCP
{
    [SingletonGame]
    public class PickelCardGameDetailClass : IGameInfo, ICardInfo<PickelCardGameCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Pickel Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 3; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //iffy this time.  well see.  there are 10 cards though.

        int ICardInfo<PickelCardGameCardInformation>.CardsToPassOut => 10;

        CustomBasicList<int> ICardInfo<PickelCardGameCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<PickelCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<PickelCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<PickelCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<PickelCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<PickelCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<PickelCardGameCardInformation>.NoPass => false;

        bool ICardInfo<PickelCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<PickelCardGameCardInformation> ICardInfo<PickelCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<PickelCardGameCardInformation>();

        bool ICardInfo<PickelCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<PickelCardGameCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => true;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => false;
    }
}