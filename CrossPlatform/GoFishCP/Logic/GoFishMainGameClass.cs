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
using BasicGameFrameworkLibrary.CommonInterfaces;
using GoFishCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainGameInterfaces;

namespace GoFishCP.Logic
{
    [SingletonGame]
    public class GoFishMainGameClass : CardGameClass<RegularSimpleCard, GoFishPlayerItem, GoFishSaveInfo>, IMiscDataNM, IFinishStart
    {
        

        private readonly GoFishVMData _model;
        private readonly IAskProcesses _processes;
        private readonly GoFishComputerAI _ai = new GoFishComputerAI();
        public GoFishMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            GoFishVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularSimpleCard> cardInfo,
            CommandContainer command,
            GoFishGameContainer gameContainer,
            IAskProcesses processes)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _processes = processes;
        }
        public bool IsValidMove(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (Test!.AllowAnyMove == true)
                return true;
            return thisCol.First().Value == thisCol.Last().Value;
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            //anything else needed is here.
            return base.FinishGetSavedAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        } 
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.5);
            if (SaveRoot!.RemovePairs == false && SaveRoot.NumberAsked == false)
            {
                if (PlayerList.Count() == 2)
                {
                    await _processes.NumberToAskAsync(_ai.NumberToAsk(SaveRoot));
                    return;
                }
                throw new BasicBlankException("Only 2 players are supported now");
            }
            var thisList = _ai.PairToPlay(SaveRoot);
            if (thisList.Count == 0)
            {
                await EndTurnAsync();
                return;
            }
            if (thisList.Count != 2)
                throw new BasicBlankException("Needed one pair to remove.  Rethink");
            await ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            SaveRoot!.RemovePairs = true;
            SaveRoot.NumberAsked = false; //i think.
            _model.AskList.ItemList.Clear();
            PlayerList!.ForEach(items => items.Pairs = 0); //i think i forgot that part in the old version.
            return base.StartSetUpAsync(isBeginning);
        }

        public async Task ProcessPlayAsync(int deck1, int deck2)
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(deck1);
            RegularSimpleCard secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(deck2);
            SingleInfo.Pairs++;
            secondCard.IsSelected = false;
            secondCard.Drew = false;
            if (_model!.Pile1!.CardExist(secondCard.Deck) == false)
                _model.Pile1.AddCard(secondCard);
            else if (_model.Pile1.CardExist(deck2) == false)
            {
                secondCard = SingleInfo.MainHandList.RemoveObjectByDeck(deck1);
                _model.Pile1.AddCard(secondCard);
            }
            if (SingleInfo.MainHandList.Count == 0)
            {
                int cards = _model.Deck1!.CardsLeft();
                if (cards < 5 && cards > 0)
                {
                    LeftToDraw = cards;
                    PlayerDraws = WhoTurn;
                    await DrawAsync();
                    _processes.LoadAskList();
                    return;
                }
                else if (cards > 0)
                {
                    LeftToDraw = 5;
                    PlayerDraws = WhoTurn;
                    await DrawAsync();
                    _processes.LoadAskList();
                }
            }
            await ContinueTurnAsync();
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "numbertoask":
                    EnumCardValueList thisValue = await js.DeserializeObjectAsync<EnumCardValueList>(content);
                    await _processes.NumberToAskAsync(thisValue);
                    return;
                case "processplay":
                    SendPair thisPair = await js.DeserializeObjectAsync<SendPair>(content);
                    await ProcessPlayAsync(thisPair.Card1, thisPair.Card2);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        private bool CanEndGame()
        {
            if (Test!.ImmediatelyEndGame)
            {
                return true;
            }
            foreach (var thisPlayer in PlayerList!)
            {
                if (thisPlayer.MainHandList.Count() != 0)
                    return false;
            }
            return true;
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.Pairs).Take(1).Single();
            if (SingleInfo.Pairs == 13)
                await ShowTieAsync();
            else
                await ShowWinAsync();
        }
        
        public override async Task EndTurnAsync()
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                _model!.PlayerHand1!.EndTurn();
            if (CanEndGame() == true)
            {
                await GameOverAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (SaveRoot!.RemovePairs == true && WhoTurn == WhoStarts)
                SaveRoot.RemovePairs = false;
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.NumberAsked = false;
            if (SaveRoot.RemovePairs == false)
                _processes.LoadAskList();
            await ContinueTurnAsync();
        }
        public Task FinishStartAsync()
        {
            if (SaveRoot!.RemovePairs == false)
                _processes.LoadAskList(); //i think.
            return Task.CompletedTask;
        }
    }
}