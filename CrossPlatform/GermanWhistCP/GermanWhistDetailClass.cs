using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace GermanWhistCP
{
    [SingletonGame]
    public class GermanWhistDetailClass : IGameInfo, ICardInfo<GermanWhistCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //this one is still new game though.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "German Whist"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<GermanWhistCardInformation>.CardsToPassOut => 13;

        CustomBasicList<int> ICardInfo<GermanWhistCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<GermanWhistCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<GermanWhistCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<GermanWhistCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<GermanWhistCardInformation>.PassOutAll => false;

        bool ICardInfo<GermanWhistCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<GermanWhistCardInformation>.NoPass => false;

        bool ICardInfo<GermanWhistCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<GermanWhistCardInformation> ICardInfo<GermanWhistCardInformation>.DummyHand { get; set; } = new DeckObservableDict<GermanWhistCardInformation>();

        bool ICardInfo<GermanWhistCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<GermanWhistCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => true;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => false;
    }
}