using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LifeCardGameCP.Cards;
using LifeCardGameCP.Logic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LifeCardGameCP.Data
{
    [SingletonGame]
    [AutoReset] //usually needs reset
    public class LifeCardGameGameContainer : CardGameContainer<LifeCardGameCardInformation, LifeCardGamePlayerItem, LifeCardGameSaveInfo>
    {
        public LifeCardGameGameContainer(BasicData basicData,
            TestOptions test,
            IGameInfo gameInfo,
            IAsyncDelayer delay,
            IEventAggregator aggregator,
            CommandContainer command,
            IGamePackageResolver resolver,
            IListShuffler<LifeCardGameCardInformation> deckList,
            RandomGenerator random)
            : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
        {
        }
        internal DeckRegularDict<LifeCardGameCardInformation> YearCards() => DeckList.Where(items => items.CanBeInPlayerHandToBeginWith == false).ToRegularDeckDict();


        public LifeCardGameCardInformation? CardChosen { get; set; }
        public int PlayerChosen { get; set; }
        public DeckRegularDict<LifeCardGameCardInformation>? TradeList { get; set; } = new DeckRegularDict<LifeCardGameCardInformation>();

        internal Func<int, Task>? ChosePlayerAsync { get; set; }

        internal Func<Task>? LoadOtherScreenAsync { get; set; }
        internal Func<Task>? CloseOtherScreenAsync { get; set; }
        public LifeCardGamePlayerItem PlayerWithCard(LifeCardGameCardInformation thisCard)
        {
            var tempList = PlayerList!.ToCustomBasicList();
            tempList.RemoveSpecificItem(SingleInfo!); //try this way
            foreach (var thisPlayer in tempList)
            {
                if (thisPlayer.LifeStory!.HandList.ObjectExist(thisCard.Deck))
                    return thisPlayer;
            }
            throw new BasicBlankException("No player has the card");
        }
        public void CreateLifeStoryPile(LifeCardGameVMData model, LifeCardGamePlayerItem thisPlayer)
        {
            thisPlayer.LifeStory = new LifeStoryHand(this, model, thisPlayer.Id);
            thisPlayer.LifeStory.Text = thisPlayer.NickName; //hopefully no need for enabled part (?)
        }
        public int OtherCardSelected()
        {
            var tempList = PlayerList!.AllPlayersExceptForCurrent();
            int decks = 0;
            int tempDeck;
            foreach (var thisPlayer in tempList)
            {
                tempDeck = thisPlayer.LifeStory!.ObjectSelected();
                if (tempDeck > 0)
                {
                    if (decks > 0)
                        return 0;
                }
                decks = tempDeck;
            }
            return decks;
        }

    }
}
