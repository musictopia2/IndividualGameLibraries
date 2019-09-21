using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace SnagCardGameCP
{
    [SingletonGame]
    public class SnagCardGameDetailClass : IGameInfo, ICardInfo<SnagCardGameCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Snag Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<SnagCardGameCardInformation>.CardsToPassOut => 5;

        CustomBasicList<int> ICardInfo<SnagCardGameCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<SnagCardGameCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<SnagCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<SnagCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<SnagCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<SnagCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<SnagCardGameCardInformation>.NoPass => false;

        bool ICardInfo<SnagCardGameCardInformation>.NeedsDummyHand => true;

        DeckObservableDict<SnagCardGameCardInformation> ICardInfo<SnagCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<SnagCardGameCardInformation>();

        bool ICardInfo<SnagCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<SnagCardGameCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => true;
    }
}