using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using Phase10CP.Cards;
namespace Phase10CP.Data
{
    [SingletonGame]
    public class Phase10DetailClass : IGameInfo, ICardInfo<Phase10CardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //phase 10 is actually rounds.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.NetworkOnly; //try this way.

        string IGameInfo.GameName => "Phase 10"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 7; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<Phase10CardInformation>.CardsToPassOut => 10;

        CustomBasicList<int> ICardInfo<Phase10CardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<Phase10CardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<Phase10CardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<Phase10CardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<Phase10CardInformation>.PassOutAll => false;

        bool ICardInfo<Phase10CardInformation>.PlayerGetsCards => true;

        bool ICardInfo<Phase10CardInformation>.NoPass => false;

        bool ICardInfo<Phase10CardInformation>.NeedsDummyHand => false;

        DeckObservableDict<Phase10CardInformation> ICardInfo<Phase10CardInformation>.DummyHand { get; set; } = new DeckObservableDict<Phase10CardInformation>();

        bool ICardInfo<Phase10CardInformation>.HasDrawAnimation => true;

        bool ICardInfo<Phase10CardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<Phase10CardInformation>.DiscardExcludeList(IListShuffler<Phase10CardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}