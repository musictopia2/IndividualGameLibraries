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
using CrazyEightsCP.Data;
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
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;

namespace CrazyEightsCP.Logic
{
    [SingletonGame]
    public class CrazyEightsMainGameClass : CardGameClass<RegularSimpleCard, CrazyEightsPlayerItem, CrazyEightsSaveInfo>
    {

        private readonly CrazyEightsComputerAI _aI = new CrazyEightsComputerAI();
        private readonly CrazyEightsVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly CrazyEightsGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly ISuitProcesses _processes;

        public CrazyEightsMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            CrazyEightsVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularSimpleCard> cardInfo,
            CommandContainer command,
            CrazyEightsGameContainer gameContainer,
            ISuitProcesses processes
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _processes = processes;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SaveRoot.LoadMod(_model);
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
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
                throw new BasicBlankException("The computer player can't go for anybody else on this game");
            if (Test!.ComputerEndsTurn == true)
                throw new BasicBlankException("The computer player was suppposed to end turn.  Rethink");
            await Delay!.DelayMilli(200);
            if (SaveRoot!.ChooseSuit == true)
            {
                EnumSuitList thisSuit = _aI.SuitToChoose(SingleInfo);
                await _processes.SuitChosenAsync(thisSuit);
                return;
            }
            await Delay.DelaySeconds(.5);
            int Nums = _aI.CardToPlay(SaveRoot);
            if (Nums > 0)
            {
                await PlayCardAsync(Nums);
                return;
            }
            if (BasicData!.MultiPlayer == true)
                await _gameContainer.SendDrawMessageAsync();
            await DrawAsync();
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
           return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync() //try here instead.
        {
            var tempCard = _model!.Pile1!.GetCardInfo();
            SaveRoot!.CurrentNumber = tempCard.Value;
            if (tempCard.DisplaySuit == EnumSuitList.None)
                SaveRoot.CurrentSuit = tempCard.Suit;
            else
                SaveRoot.CurrentSuit = tempCard.DisplaySuit;
            SaveRoot.CurrentCard = tempCard.Deck; //i think
            SaveRoot.LoadMod(_model);
            return Task.CompletedTask;
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SaveRoot!.ChooseSuit = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            if ((SingleInfo.MainHandList.Count == 0 && PlayerCanWin() == true) || Test!.ImmediatelyEndGame)
            {
                await ShowWinAsync();
                return; //because game is over now
            }
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }

        protected async override Task AfterDrawingAsync()
        {
            _gameContainer.AlreadyDrew = false;
            PlayerDraws = 0;
            await base.AfterDrawingAsync();
        }
        public bool IsValidMove(int deck)
        {
            var ThisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self && Test!.AllowAnyMove == true)
                return true;
            if (ThisCard.Value == EnumCardValueList.Eight)
                return true;
            if (ThisCard.Value == SaveRoot!.CurrentNumber)
                return true;
            if (ThisCard.Suit == SaveRoot.CurrentSuit)
                return true;
            return false;
        }
        public async Task PlayCardAsync(int deck)
        {
            var ThisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            ThisCard.Drew = false;
            ThisCard.IsSelected = false; //i think
            await _gameContainer.SendDiscardMessageAsync(deck);
            await DiscardAsync(ThisCard);
        }
        public override async Task DiscardAsync(RegularSimpleCard thisCard)
        {
            int firstCount;
            firstCount = SingleInfo!.MainHandList.Count;
            SingleInfo.MainHandList.RemoveSpecificItem(thisCard); //for now, okay.
            int secondCount = SingleInfo.MainHandList.Count;
            if (secondCount + 1 != firstCount)
            {
                throw new BasicBlankException("Warning, second count was " + secondCount + " and first count was " + firstCount + ".  Something is not right");
            }
            await AnimatePlayAsync(thisCard);

            SaveRoot!.CurrentNumber = thisCard.Value;
            if (SingleInfo.ObjectCount != SingleInfo.MainHandList.Count)
                throw new BasicBlankException("Failed to update count.  Rethink");
            if (thisCard.Value == EnumCardValueList.Eight && SingleInfo.MainHandList.Count > 0)
            {
                SaveRoot.ChooseSuit = true;
                await ContinueTurnAsync();
                return;
            }
            SaveRoot.CurrentSuit = thisCard.Suit;
            var TempCard = _model!.Pile1!.GetCardInfo();
            if (TempCard.Deck != thisCard.Deck)
                throw new BasicBlankException("Failed To Add To Deck");
            await EndTurnAsync();
        }

    }
}