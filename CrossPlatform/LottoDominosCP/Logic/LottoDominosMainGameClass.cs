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
using LottoDominosCP.Data;
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
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using LottoDominosCP.EventModels;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;

namespace LottoDominosCP.Logic
{
    [SingletonGame]
    public class LottoDominosMainGameClass : BasicGameClass<LottoDominosPlayerItem, LottoDominosSaveInfo>, IMiscDataNM, IMoveNM
    {
        public LottoDominosMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            LottoDominosVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            GameBoardCP gameBoard,
            BasicGameContainer<LottoDominosPlayerItem, LottoDominosSaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state,delay, command, gameContainer)
        {
            _test = test;
            _model = model;
            _gameBoard = gameBoard;
            _gameBoard.MakeMoveAsync = MakeMoveAsync;
        }

        private readonly TestOptions _test;
        private readonly LottoDominosVMData _model;
        private readonly GameBoardCP _gameBoard; //anybody can get it as needed.
        #region "Delegates to stop the overflow"
        //because whoever has to reload the list may have to have access to the game class.
        //but if the game class required this too, that is overflow.

        public Action? ReloadNumberLists { get; set; }
        public Func<int, Task>? ProcessNumberAsync { get; set; }
        public Func<Task>? ComputerChooseNumberAsync { get; set; }
        #endregion



        public override Task FinishGetSavedAsync()
        {
            LoadControls();

            _gameBoard!.LoadSavedGame(SaveRoot!.BoardDice!);
            _model.DominosList!.ClearObjects(); //has to clear objects first.
            _model.DominosList.OrderedObjects(); //i think this should be fine.
            if (BasicData.Client == true)
                SaveRoot.ComputerList.Clear(); //because they don't need to know about computer list.
            //hopefully no need for gameboard visible.

            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            PlayerList!.ForEach(items =>
            {
                items.NumberChosen = -1;
                items.NumberWon = 0;
            });
            _model.DominosList!.ClearObjects();
            _model.DominosList.ShuffleObjects(); //i think.
            SaveRoot!.ComputerList.Clear();
            SaveRoot.GameStatus = EnumStatus.ChooseNumber;
            _gameBoard.ClearPieces(); //i think
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            await FinishUpAsync(isBeginning);
        }
        public override async Task ContinueTurnAsync()
        {
            if (SaveRoot.GameStatus == EnumStatus.ChooseNumber)
            {
                if (ReloadNumberLists == null)
                {
                    throw new BasicBlankException("Nobody is handling the reloading of the number lists.  Rethink");
                }
                ReloadNumberLists.Invoke();
            }
            await base.ContinueTurnAsync();
            if (SaveRoot.GameStatus == EnumStatus.NormalPlay)
            {
                _gameBoard.ReportCanExecuteChange();
            }
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.BoardDice = _gameBoard!.ObjectList.ToRegularDeckDict();
            return Task.CompletedTask;
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                //put in cases here.
                case "numberchosen":
                    int chosen = int.Parse(content);
                    if (ProcessNumberAsync == null)
                    {
                        throw new BasicBlankException("There was no function to process number chosen.  Rethink");
                    }
                    await ProcessNumberAsync.Invoke(chosen);
                    break;
            
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            this.ShowTurn();
            
            if (SingleInfo!.NumberChosen > -1 && SaveRoot.GameStatus == EnumStatus.ChooseNumber)
            {
                SaveRoot!.GameStatus = EnumStatus.NormalPlay;
                await Aggregator.PublishAsync(new ChangeGameStatusEventModel()); //i think should be here instead.
            }
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public CustomBasicList<int> GetNumberList()
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            for (int x = 0; x <= 6; x++)
            {
                if (PlayerList.Any(y => y.NumberChosen == x) == false)
                    output.Add(x);
            }
            return output;
        }
        public async Task MoveReceivedAsync(string data)
        {
            await MakeMoveAsync(int.Parse(data));
        }
        public async Task MakeMoveAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(BasicData) == true)
                await Network!.SendMoveAsync(deck);
            await _gameBoard.ShowDominoAsync(deck);
            if (CanPlay(deck) == false)
            {
                AddComputer(deck);
                await EndTurnAsync();
                return;
            }
            TakeOffComputer(deck);
            _gameBoard.MakeInvisible(deck);
            SingleInfo!.NumberWon++;
            if (IsGameOver(SingleInfo.NumberWon) == true)
            {
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public bool CanPlay(int deck)
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer && _test!.AllowAnyMove == true)
                return true; //because we are allowing any move for testing.
            SimpleDominoInfo thisDomino = _model.DominosList!.GetSpecificItem(deck);
            return thisDomino.FirstNum == SingleInfo.NumberChosen || thisDomino.SecondNum == SingleInfo.NumberChosen;
        }
        private bool IsGameOver(int score)
        {
            return score >= 4;
        }

        #region "Computer Processes"
        private void AddComputer(int deck)
        {
            if (BasicData.Client == true)
                return; //host does computer.
            if (SaveRoot!.ComputerList.ObjectExist(deck) == false)
            {
                SimpleDominoInfo thisDomino = _model.DominosList!.GetSpecificItem(deck);
                SaveRoot.ComputerList.Add(thisDomino);
            }
        }
        public void TakeOffComputer(int deck)
        {
            if (BasicData.Client == true)
                return;
            if (SaveRoot!.ComputerList.ObjectExist(deck) == false)
                return;
            SaveRoot.ComputerList.RemoveObjectByDeck(deck);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (_test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SaveRoot!.GameStatus == EnumStatus.ChooseNumber)
            {
                if (ComputerChooseNumberAsync == null)
                {
                    throw new BasicBlankException("Nobody is processing the number chosen for computer.  Rethink");
                }
                await ComputerChooseNumberAsync.Invoke();
                //await ProcessNumberAsync(_thisMod.Number1!.NumberToChoose()); //i think
                return;
            }
            DeckRegularDict<SimpleDominoInfo> output;
            if (SaveRoot.ComputerList.Count() > 0)
            {
                output = SaveRoot.ComputerList.Where(Items => CanPlay(Items.Deck)).ToRegularDeckDict();
                if (output.Count > 0)
                {
                    await MakeMoveAsync(output.GetRandomItem().Deck);
                    return;
                }
            }
            output = _gameBoard.GetVisibleList();
            await MakeMoveAsync(output.GetRandomItem().Deck);
        }
        #endregion

    }
}