using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using TeeItUpCP.Cards;
namespace TeeItUpCP.Data
{
    [SingletonGame]
    public class TeeItUpDetailClass : IGameInfo, ICardInfo<TeeItUpCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.HumanOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Tee It Up"; //put in the name of the game here.
        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 2; //if different, put here.

        //needs to make it where one player does the 2 then somebody else.
        //they can't do the same time to make autoresume work on this game.
        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<TeeItUpCardInformation>.CardsToPassOut => 8;

        CustomBasicList<int> ICardInfo<TeeItUpCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<TeeItUpCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<TeeItUpCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<TeeItUpCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<TeeItUpCardInformation>.PassOutAll => false;

        bool ICardInfo<TeeItUpCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<TeeItUpCardInformation>.NoPass => false;

        bool ICardInfo<TeeItUpCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<TeeItUpCardInformation> ICardInfo<TeeItUpCardInformation>.DummyHand { get; set; } = new DeckObservableDict<TeeItUpCardInformation>();

        bool ICardInfo<TeeItUpCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<TeeItUpCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<TeeItUpCardInformation>.DiscardExcludeList(IListShuffler<TeeItUpCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}