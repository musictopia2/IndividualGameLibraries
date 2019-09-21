using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Spades2PlayerCP
{
    [SingletonGame]
    public class Spades2PlayerDetailClass : IGameInfo, ICardInfo<Spades2PlayerCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Spades (2 Player)"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<Spades2PlayerCardInformation>.CardsToPassOut => 7;

        CustomBasicList<int> ICardInfo<Spades2PlayerCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<Spades2PlayerCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<Spades2PlayerCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<Spades2PlayerCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<Spades2PlayerCardInformation>.PassOutAll => false;

        bool ICardInfo<Spades2PlayerCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<Spades2PlayerCardInformation>.NoPass => true; //this does not pass cards.

        bool ICardInfo<Spades2PlayerCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<Spades2PlayerCardInformation> ICardInfo<Spades2PlayerCardInformation>.DummyHand { get; set; } = new DeckObservableDict<Spades2PlayerCardInformation>();

        bool ICardInfo<Spades2PlayerCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<Spades2PlayerCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => true;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.Spades;

        bool ITrickData.HasDummy => false;
    }
}