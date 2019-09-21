using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace RoundsCardGameCP
{
    [SingletonGame]
    public class RoundsCardGameDetailClass : IGameInfo, ICardInfo<RoundsCardGameCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //has to be both.  otherwise, gets stuck.

        string IGameInfo.GameName => "Rounds Card Game"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<RoundsCardGameCardInformation>.CardsToPassOut => 9;

        CustomBasicList<int> ICardInfo<RoundsCardGameCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<RoundsCardGameCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<RoundsCardGameCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<RoundsCardGameCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<RoundsCardGameCardInformation>.PassOutAll => false;

        bool ICardInfo<RoundsCardGameCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<RoundsCardGameCardInformation>.NoPass => false;

        bool ICardInfo<RoundsCardGameCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<RoundsCardGameCardInformation> ICardInfo<RoundsCardGameCardInformation>.DummyHand { get; set; } = new DeckObservableDict<RoundsCardGameCardInformation>();

        bool ICardInfo<RoundsCardGameCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<RoundsCardGameCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => false;
    }
}