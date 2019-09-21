using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ConnectTheDotsCP
{
    [SingletonGame]
    public class ConnectTheDotsMainGameClass : SimpleBoardGameClass<EnumColorChoice, CheckerChoiceCP<EnumColorChoice>,
        ConnectTheDotsPlayerItem, ConnectTheDotsSaveInfo, int>
    {
        public ConnectTheDotsMainGameClass(IGamePackageResolver container) : base(container) { }
        private ConnectTheDotsViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ConnectTheDotsViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            if (DidChooseColors)
                _thisMod!.GameBoard1!.LoadGame();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        public override Task PopulateSaveRootAsync()
        {
            if (DidChooseColors)
                _thisMod!.GameBoard1!.SaveGame();
            return base.PopulateSaveRootAsync();
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
            _thisMod!.GameBoard1!.ClearBoard();
            await EndTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int dot)
        {
            await _thisMod!.GameBoard1!.MakeMoveAsync(dot);
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            _thisMod!.CommandContainer!.ManuelFinish = true;
            await StartNewTurnAsync();
        }
    }
}