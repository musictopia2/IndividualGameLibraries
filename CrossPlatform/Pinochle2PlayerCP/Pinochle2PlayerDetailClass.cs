using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace Pinochle2PlayerCP
{
    [SingletonGame]
    public class Pinochle2PlayerDetailClass : IGameInfo, ICardInfo<Pinochle2PlayerCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Pinochle (2 Player)"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<Pinochle2PlayerCardInformation>.CardsToPassOut => 12;

        CustomBasicList<int> ICardInfo<Pinochle2PlayerCardInformation>.ExcludeList => new CustomBasicList<int>();

        bool ICardInfo<Pinochle2PlayerCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<Pinochle2PlayerCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<Pinochle2PlayerCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<Pinochle2PlayerCardInformation>.PassOutAll => false;

        bool ICardInfo<Pinochle2PlayerCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<Pinochle2PlayerCardInformation>.NoPass => false;

        bool ICardInfo<Pinochle2PlayerCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<Pinochle2PlayerCardInformation> ICardInfo<Pinochle2PlayerCardInformation>.DummyHand { get; set; } = new DeckObservableDict<Pinochle2PlayerCardInformation>();

        bool ICardInfo<Pinochle2PlayerCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<Pinochle2PlayerCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => false;

        bool ITrickData.HasTrump => false;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => false;
    }
}