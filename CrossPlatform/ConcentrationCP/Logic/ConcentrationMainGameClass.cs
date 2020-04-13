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
using ConcentrationCP.Data;
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
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;

namespace ConcentrationCP.Logic
{
    [SingletonGame]
    public class ConcentrationMainGameClass : CardGameClass<RegularSimpleCard, ConcentrationPlayerItem, ConcentrationSaveInfo>, IMiscDataNM
    {
        private readonly ConcentrationVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.

        public ConcentrationMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            ConcentrationVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularSimpleCard> cardInfo,
            CommandContainer command,
            ConcentrationGameContainer gameContainer)
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            _model!.GameBoard1!.PileList = SaveRoot!.BoardList.ToCustomBasicList();
            return base.FinishGetSavedAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            var thisCol = _model!.GameBoard1!.GetSelectedCards();
            if (thisCol.Count == 2)
            {
                await SaveStateAsync(); //so you can't cheat.
                await ProcessPlayAsync(thisCol);
                return;
            }
            await base.ContinueTurnAsync();
        }
        private async Task ProcessPlayAsync(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (IsValidMove(thisCol) == true)
            {
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(1);
                RemoveComputer(thisCol);
                _model!.GameBoard1!.SelectedCardsGone();
                SingleInfo!.Pairs++;
                if (_model.GameBoard1.CardsGone == true || Test.ImmediatelyEndGame == true)
                {
                    await GameOverAsync();
                    return;
                }
                await ContinueTurnAsync();
                return;
            }
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(5);
            _model!.GameBoard1!.UnselectCards();
            AddComputer(thisCol);
            await EndTurnAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        private bool IsValidMove(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (Test!.AllowAnyMove == true && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                return true; //for testing.
            return thisCol.HasDuplicates(items => items.Value); //hopefully this simple now.
        }
        private void RemoveComputer(DeckRegularDict<RegularSimpleCard> ThisCol)
        {
            if (PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer) == false)
                return;
            ThisCol.ForEach(thisCard =>
            {
                if (SaveRoot!.ComputerList.ObjectExist(thisCard.Deck) == true)
                    SaveRoot.ComputerList.RemoveObjectByDeck(thisCard.Deck);
            });
        }
        private void AddComputer(DeckRegularDict<RegularSimpleCard> thisCol)
        {
            if (PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer) == false)
                return;
            thisCol.ForEach(thisCard =>
            {
                if (SaveRoot!.ComputerList.ObjectExist(thisCard.Deck) == false)
                    SaveRoot.ComputerList.Add(thisCard);
            });
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            await SelectCardAsync(ComputerAI.CardToTry(this, _model));
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(thisPlayer => thisPlayer.Pairs = 0);
            SaveRoot!.ComputerList = new DeckRegularDict<RegularSimpleCard>();
            _model!.GameBoard1!.ClearBoard();
            return base.StartSetUpAsync(isBeginning);
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "selectcard":
                    await SelectCardAsync(int.Parse(content));
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync(); //try this too.
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerList.OrderByDescending(items => items.Pairs).First();
            await ShowWinAsync();
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.BoardList = _model.GameBoard1.PileList!.ToCustomBasicList();
            await base.PopulateSaveRootAsync();
        }
        internal async Task SelectCardAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
                await Network!.SendAllAsync("selectcard", deck);
            _model!.GameBoard1!.SelectCard(deck);
            await ContinueTurnAsync(); //this will handle the rest.
        }
    }
}