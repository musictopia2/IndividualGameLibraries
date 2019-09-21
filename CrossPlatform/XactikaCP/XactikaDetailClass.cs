using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace XactikaCP
{
    [SingletonGame]
    public class XactikaDetailClass : IGameInfo, ICardInfo<XactikaCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Xactika"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<XactikaCardInformation>.CardsToPassOut => 8;

        CustomBasicList<int> ICardInfo<XactikaCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<XactikaCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<XactikaCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<XactikaCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<XactikaCardInformation>.PassOutAll => false;

        bool ICardInfo<XactikaCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<XactikaCardInformation>.NoPass => false;

        bool ICardInfo<XactikaCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<XactikaCardInformation> ICardInfo<XactikaCardInformation>.DummyHand { get; set; } = new DeckObservableDict<XactikaCardInformation>();

        bool ICardInfo<XactikaCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<XactikaCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => false;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => false;
    }
}