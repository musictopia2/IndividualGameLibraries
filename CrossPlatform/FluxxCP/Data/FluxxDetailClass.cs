using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FluxxCP.Cards;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
//i think this is the most common things i like to do
namespace FluxxCP.Data
{



    [SingletonGame]
    public class FluxxDetailClass : IGameInfo, ICardInfo<FluxxCardInformation>, INewCard<FluxxCardInformation>
    {
        EnumGameType IGameInfo.GameType => EnumGameType.NewGame; //if rounds, change it here.

        bool IGameInfo.CanHaveExtraComputerPlayers => false; //if you can have extra computer players, here.

        EnumPlayerChoices IGameInfo.SinglePlayerChoice => EnumPlayerChoices.ComputerOnly; //most regular card games is not pass and play.

        EnumPlayerType IGameInfo.PlayerType => EnumPlayerType.SingleAndNetworked;

        string IGameInfo.GameName => "Fluxx"; //put in the name of the game here.

        int IGameInfo.NoPlayers => 0;

        int IGameInfo.MinPlayers => 2;

        int IGameInfo.MaxPlayers => 6; //if different, put here.

        bool IGameInfo.CanAutoSave => true;

        EnumSmallestSuggested IGameInfo.SmallestSuggestedSize => EnumSmallestSuggested.AnyTablet; //default to smallest but can change as needed.

        EnumSuggestedOrientation IGameInfo.SuggestedOrientation => EnumSuggestedOrientation.Landscape; //looks like most games are landscape.  can change to what is needed though.

        int ICardInfo<FluxxCardInformation>.CardsToPassOut => 3;

        CustomBasicList<int> ICardInfo<FluxxCardInformation>.PlayerExcludeList => new CustomBasicList<int>() { 1 }; //so this will not be considered.

        bool ICardInfo<FluxxCardInformation>.AddToDiscardAtBeginning => false;

        bool ICardInfo<FluxxCardInformation>.ReshuffleAllCardsFromDiscard => false;

        bool ICardInfo<FluxxCardInformation>.ShowMessageWhenReshuffling => true;

        bool ICardInfo<FluxxCardInformation>.PassOutAll => false;

        bool ICardInfo<FluxxCardInformation>.PlayerGetsCards => true;

        bool ICardInfo<FluxxCardInformation>.NoPass => false;

        bool ICardInfo<FluxxCardInformation>.NeedsDummyHand => false;

        DeckObservableDict<FluxxCardInformation> ICardInfo<FluxxCardInformation>.DummyHand { get; set; } = new DeckObservableDict<FluxxCardInformation>();

        bool ICardInfo<FluxxCardInformation>.HasDrawAnimation => true;

        bool ICardInfo<FluxxCardInformation>.CanSortCardsToBeginWith => true;

        FluxxCardInformation INewCard<FluxxCardInformation>.GetNewCard(int chosen)
        {
            return GetNewCard(chosen);
        }
        public static FluxxCardInformation GetNewCard(int chosen)
        {
            if (chosen <= 0)
                throw new BasicBlankException("Must choose a value above 0 for GetNewCard for Fluxx");
            if (chosen <= 22)
                return new RuleCard();
            if (chosen <= 40)
                return new KeeperCard();
            if (chosen <= 63)
                return new GoalCard();
            if (chosen <= 83)
                return new ActionCard();
            throw new BasicBlankException("Must go only up to 83");
        }

        CustomBasicList<int> ICardInfo<FluxxCardInformation>.DiscardExcludeList(IListShuffler<FluxxCardInformation> deckList)
        {
            return new CustomBasicList<int>();
        }
    }
}