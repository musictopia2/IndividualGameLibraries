using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
using UnoCP.Cards;
namespace UnoCP.Data
{
    [SingletonGame]
    public class UnoDetailClass : IGameInfo, ICardInfo<UnoCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.Rounds;

        bool IGameInfo.CanHaveExtraComputerPlayers => true;

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly;

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked; //computer can play this one too.

        string IGameInfo.GameName => "Uno";

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6;

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyDevice; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<UnoCardInformation>.CardsToPassOut => 7; //change to what you need.

        CustomBasicList<int> ICardInfo<UnoCardInformation>.PlayerExcludeList => new CustomBasicList<int>();

        bool ICardInfo<UnoCardInformation>.AddToDiscardAtBeginning => true;

        bool ICardInfo<UnoCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<UnoCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<UnoCardInformation>.PassOutAll => false;

        bool ICardInfo<UnoCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<UnoCardInformation>.NoPass => false;

        bool ICardInfo<UnoCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<UnoCardInformation> ICardInfo<UnoCardInformation>.DummyHand { get; set; } = new DeckObservableDict<UnoCardInformation>();

        bool ICardInfo<UnoCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<UnoCardInformation>.CanSortCardsToBeginWith => true;

        CustomBasicList<int> ICardInfo<UnoCardInformation>.DiscardExcludeList(IListShuffler<UnoCardInformation> deckList)
        {
            return deckList.Where(x => x.WhichType == EnumCardTypeList.Wild && x.Draw == 4).Select(x => x.Deck).ToCustomBasicList();
        }
    }
}