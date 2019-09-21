using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ChessCP
{
    [SingletonGame]
    public class ChessMainGameClass : SimpleBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        ChessPlayerItem, ChessSaveInfo, int>, IMiscDataNM, IFinishStart
    {
        public ChessMainGameClass(IGamePackageResolver container) : base(container) { }

        public ChessViewModel? ThisMod;
        internal GlobalClass? ThisGlobal;
        private bool _autoResume;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<ChessViewModel>();
            ThisGlobal = MainContainer.Resolve<GlobalClass>();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            _autoResume = true;
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ThisMod!.GameBoard1!.LoadBoard();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            //if there is nothing, then just won't do anything.
            await Task.CompletedTask;
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            EraseColors();
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            _autoResume = false;
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            SaveRoot!.GameStatus = EnumGameStatus.None;
            ThisE.RepaintBoard(); //hopefully this simple.
            await Task.Delay(300); //so it can populate the board properly.
            ThisMod!.GameBoard1!.ClearBoard();
            ThisGlobal!.CurrentMoveList.Clear();
            SaveRoot.PreviousMove = new PreviousMove();
            SaveRoot.PossibleMove = new PreviousMove(); //try this too.
            await EndTurnAsync(); //decided to do an endturn this time.
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                //put in cases here.
                case "possibletie":
                    await ProcessTieAsync();
                    return;
                case "undomove":
                    await ThisMod!.GameBoard1!.UndoAllMovesAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            SingleInfo = PlayerList!.GetWhoPlayer(); //just in case.
            if (DidChooseColors == false)
                await ContinueTurnAsync();
            else
                await ThisMod!.GameBoard1!.StartNewTurnAsync(); //hopefully this simple.
        }
        public override async Task MakeMoveAsync(int space) //learned from problems from checkers.
        {
            await ThisMod!.GameBoard1!.MakeMoveAsync(space); //hopefully this simple.
        }
        public override async Task EndTurnAsync()
        {
            PlayerList!.ForEach(thisPlayer => thisPlayer.PossibleTie = false);
            if (DidChooseColors == true && SaveRoot!.PossibleMove != null)
                SaveRoot.PreviousMove = SaveRoot.PossibleMove; //otherwise, sets to null. wrong.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            //if anything else is needed, do here.

            await StartNewTurnAsync();
        }
        public override Task ContinueTurnAsync()
        {
            if (DidChooseColors == false)
                SaveRoot!.Instructions = "";
            else
            {
                if (SaveRoot!.GameStatus == EnumGameStatus.PossibleTie)
                    SaveRoot.Instructions = "Either Agree To Tie Or End Turn";
                else
                {
                    if (SaveRoot.SpaceHighlighted == 0)
                        SaveRoot.Instructions = "Make Move Or Initiate Tie";
                    else
                        SaveRoot.Instructions = "Finish Move";
                }
            }
            return base.ContinueTurnAsync();
        }
        public async Task ProcessTieAsync()
        {
            SingleInfo!.PossibleTie = true;
            if (PlayerList.Any(items => items.PossibleTie == false))
            {
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                SaveRoot!.GameStatus = EnumGameStatus.PossibleTie;
                SingleInfo = PlayerList.GetWhoPlayer();
                PrepStartTurn();
                await ContinueTurnAsync();
                return;
            }
            await ShowTieAsync(); //hopefully this simple.
        }
        async Task IFinishStart.FinishStartAsync()
        {
            if (_autoResume && DidChooseColors == true)
                await ThisMod!.GameBoard1!.ReloadSavedGameAsync(); //hopefully this simple.
        }
    }
}