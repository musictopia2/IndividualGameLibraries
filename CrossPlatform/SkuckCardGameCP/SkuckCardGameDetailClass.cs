using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SkuckCardGameCP
{
    [SingletonGame]
    public class SkuckCardGameDetailClass : IGameInfo, ICardInfo<SkuckCardGameCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Skuck Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<SkuckCardGameCardInformation>.CardsToPassOut => 10;

        CustomBasicList<int> ICardInfo<SkuckCardGameCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<SkuckCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<SkuckCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<SkuckCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<SkuckCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<SkuckCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<SkuckCardGameCardInformation>.NoPass => false;

        bool ICardInfo<SkuckCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<SkuckCardGameCardInformation> ICardInfo<SkuckCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<SkuckCardGameCardInformation>();

        bool ICardInfo<SkuckCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<SkuckCardGameCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => true;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => true; //means to consider more than just your hand.  i think view model should implement it.
    }
}