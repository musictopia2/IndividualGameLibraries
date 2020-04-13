using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using HitTheDeckCP.Cards;
using System.Linq;

namespace HitTheDeckCP.Data
{
    [SingletonGame]
    public class HitTheDeckDetailClass : IGameInfo, ICardInfo<HitTheDeckCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => true; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Hit The Deck"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //don't worry about small phone for this game.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<HitTheDeckCardInformation>.CardsToPassOut => 7;

        CustomBasicList<int> ICardInfo<HitTheDeckCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<HitTheDeckCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<HitTheDeckCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<HitTheDeckCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<HitTheDeckCardInformation>.PassOutAll => false;

        bool ICardInfo<HitTheDeckCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<HitTheDeckCardInformation>.NoPass => false;

        bool ICardInfo<HitTheDeckCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<HitTheDeckCardInformation> ICardInfo<HitTheDeckCardInformation>.DummyHand { get; set; } = new DeckObservableDict<HitTheDeckCardInformation>();

        bool ICardInfo<HitTheDeckCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<HitTheDeckCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<HitTheDeckCardInformation>.DiscardExcludeList(IListShuffler<HitTheDeckCardInformation> deckList)
        {
            return deckList.Where(x => x.CardType == EnumTypeList.Flip).Select(x => x.Deck).ToCustomBasicList(); //the first card on the discard cannot be flip now.
        }
    }
}