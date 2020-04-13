using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using DutchBlitzCP.Cards;
namespace DutchBlitzCP.Data
{
    [SingletonGame]
    public class DutchBlitzDetailClass : IGameInfo, ICardInfo<DutchBlitzCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //this one is somehow in rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Dutch Blitz"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 4; //if different, put here.

        bool IGameInfo.CanAutoSave => false;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Portrait; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<DutchBlitzCardInformation>.CardsToPassOut => 0;

        CustomBasicList<int> ICardInfo<DutchBlitzCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<DutchBlitzCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<DutchBlitzCardInformation>.ReshuffleAllCardsFromDiscard => true; //this has to be true.  otherwise, does not shuffle enough cards when it has to reshuffle.

        bool ICardInfo<DutchBlitzCardInformation>.ShowMessageWhenReshuffling => false; //i think.

        bool ICardInfo<DutchBlitzCardInformation>.PassOutAll => false;

        bool ICardInfo<DutchBlitzCardInformation>.PlayerGetsCards => false; //trying something different.

        bool ICardInfo<DutchBlitzCardInformation>.NoPass => true; //no passing cards.  something else is happening this time.

        bool ICardInfo<DutchBlitzCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<DutchBlitzCardInformation> ICardInfo<DutchBlitzCardInformation>.DummyHand { get; set; } = new DeckObservableDict<DutchBlitzCardInformation>();

        bool ICardInfo<DutchBlitzCardInformation>.HasDrawAnimation => false;

        bool ICardInfo<DutchBlitzCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<DutchBlitzCardInformation>.DiscardExcludeList(IListShuffler<DutchBlitzCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}