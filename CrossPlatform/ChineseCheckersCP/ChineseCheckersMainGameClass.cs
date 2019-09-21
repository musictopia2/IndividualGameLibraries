using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.Animations;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ChineseCheckersCP
{
    [SingletonGame]
    public class ChineseCheckersMainGameClass : SimpleBoardGameClass<EnumColorList, MarblePiecesCP<EnumColorList>,
        ChineseCheckersPlayerItem, ChineseCheckersSaveInfo, int>, IMiscDataNM
    {
        public ChineseCheckersMainGameClass(IGamePackageResolver container) : base(container) { }
        private ChineseCheckersViewModel? _thisMod;
        public AnimateSkiaSharpGameBoard? Animates; //i'm not sure if i have any that used this one yet.
        internal GameBoardProcesses? GameBoard1; //i think can still be internal (?)
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ChineseCheckersViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            GameBoard1!.LoadSavedGame();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            Animates = new AnimateSkiaSharpGameBoard(); //hopefully this works here too.
            GameBoard1 = Resolve<GameBoardProcesses>();
            GameBoard1.LoadBoard();
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
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            GameBoard1!.ClearBoard();
            await EndTurnAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "undomove":
                    await GameBoard1!.UnselectPieceAsync();
                    return;
                case "pieceselected":
                    await GameBoard1!.HighlightItemAsync(int.Parse(content));
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                GameBoard1!.StartTurn();
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                    SaveRoot!.Instructions = "Choose a piece to move";
                else
                    SaveRoot!.Instructions = $"Waitng for {SingleInfo.NickName} to take their turn";
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space)
        {
            await GameBoard1!.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            _thisMod!.CommandContainer!.ManuelFinish = true;
            await StartNewTurnAsync();
        }
    }
}