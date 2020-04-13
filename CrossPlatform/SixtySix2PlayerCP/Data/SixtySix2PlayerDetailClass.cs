using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using SixtySix2PlayerCP.Cards;
namespace SixtySix2PlayerCP.Data
{
    [SingletonGame]
    public class SixtySix2PlayerDetailClass : IGameInfo, ICardInfo<SixtySix2PlayerCardInformation>, ITrickData
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //most trick taking games are in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Sixty Six (2 Player)"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<SixtySix2PlayerCardInformation>.CardsToPassOut => 6;

        CustomBasicList<int> ICardInfo<SixtySix2PlayerCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<SixtySix2PlayerCardInformation>.AddToDiscardAtBeginning => true; //i think this time has to add to beginning.

        bool ICardInfo<SixtySix2PlayerCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<SixtySix2PlayerCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<SixtySix2PlayerCardInformation>.PassOutAll => false;

        bool ICardInfo<SixtySix2PlayerCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<SixtySix2PlayerCardInformation>.NoPass => false;

        bool ICardInfo<SixtySix2PlayerCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<SixtySix2PlayerCardInformation> ICardInfo<SixtySix2PlayerCardInformation>.DummyHand { get; set; } = new DeckObservableDict<SixtySix2PlayerCardInformation>();

        bool ICardInfo<SixtySix2PlayerCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<SixtySix2PlayerCardInformation>.CanSortCardsToBeginWith => true;

        bool ITrickData.FirstPlayerAnySuit => true;

        bool ITrickData.FollowSuit => true;

        bool ITrickData.MustFollow => false;

        bool ITrickData.HasTrump => true;

        bool ITrickData.MustPlayTrump => false;

        EnumTrickStyle ITrickData.TrickStyle => EnumTrickStyle.None;

        bool ITrickData.HasDummy => false;

        CustomBasicList<int> ICardInfo<SixtySix2PlayerCardInformation>.DiscardExcludeList(IListShuffler<SixtySix2PlayerCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}