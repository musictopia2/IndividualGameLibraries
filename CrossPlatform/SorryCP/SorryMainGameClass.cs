using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.MiscClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SorryCP
{
    [SingletonGame]
    public class SorryMainGameClass : SimpleBoardGameClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>,
        SorryPlayerItem, SorrySaveInfo, int>, IAfterDraw<SorryPlayerItem>
    {
        public SorryMainGameClass(IGamePackageResolver container) : base(container) { }
        internal SorryViewModel? ThisMod;
        private DrawShuffleClass<CardInfo, SorryPlayerItem>? _thisShuffle;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<SorryViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            BoardGameSaved(); //i think.
            ThisMod!.GameBoard1!.LoadSavedGame(); //i think
            _thisShuffle!.SaveRoot = SaveRoot;
            HookMod();
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ThisMod!.GameBoard1!.LoadBoard(); //maybe here.
            _thisShuffle = MainContainer.Resolve<DrawShuffleClass<CardInfo, SorryPlayerItem>>();
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            EraseColors();
            _thisShuffle!.SaveRoot = SaveRoot;
            HookMod();
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        private void HookMod()
        {
            SaveRoot!.LoadMod(ThisMod!);
            if (SaveRoot.DidDraw)
                ThisMod!.CardDetails = SaveRoot.CurrentCard!.Details;
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            ThisMod!.GameBoard1!.ClearBoard();
            SaveRoot!.Instructions = "Waiting for cards to be shuffled";
            WhoTurn = WhoStarts;
            ThisMod.CommandContainer!.ManuelFinish = true;
            if (ThisData!.MultiPlayer == false)
            {
                await _thisShuffle!.FirstShuffleAsync(false); //do not autodraw this time.  not candyland.
                await StartNewTurnAsync(); //i think.
                return;
            }
            if (ThisData.Client)
            {
                ThisCheck!.IsEnabled = true;
                return; //waiting to hear from host.
            }
            await _thisShuffle!.FirstShuffleAsync(false);
            SaveRoot.ImmediatelyStartTurn = true; //so the client will start new turn.
            await ThisNet!.SendAllAsync("restoregame", SaveRoot); //hopefully this simple.
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                if (ThisData!.MultiPlayer == false)
                {
                    SaveRoot!.Instructions = $"{SingleInfo!.NickName} needs to draw a card";
                }
                else
                {
                    int ourID = PlayerList!.GetSelf().Id;
                    if (ourID == WhoTurn)
                        SaveRoot!.Instructions = "Draw a card";
                    else
                        SaveRoot!.Instructions = $"Waiting for {SingleInfo!.NickName} to draw a card";
                }
                ThisMod!.GameBoard1!.StartTurn(); //i think i forgot this too.
            }
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space)
        {
            await ThisMod!.GameBoard1!.MakeMoveAsync(space);
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            ThisMod!.CommandContainer!.ManuelFinish = true;
            await StartNewTurnAsync();
        }
        public override async Task ContinueTurnAsync()
        {
            if (ThisTest!.DoubleCheck)
            {
                ThisTest.DoubleCheck = false;
                await ThisMod!.GameBoard1!.GetValidMovesAsync();
                return;
            }
            await base.ContinueTurnAsync();
        }
        internal async Task DrawCardAsync()
        {
            await _thisShuffle!.DrawAsync();
        }
        async Task IAfterDraw<SorryPlayerItem>.AfterDrawingAsync() //will automatically populate the current card (good)
        {
            ThisMod!.GameBoard1!.ShowDraw(); //i think
            await ThisMod.GameBoard1.GetValidMovesAsync();
        }
    }
}