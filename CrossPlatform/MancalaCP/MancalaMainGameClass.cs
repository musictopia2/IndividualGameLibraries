using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using System.Collections.Generic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MancalaCP
{
    [SingletonGame]
    public class MancalaMainGameClass : BasicGameClass<MancalaPlayerItem, MancalaSaveInfo>, IMoveNM
    {
        public MancalaMainGameClass(IGamePackageResolver container) : base(container) { }
        internal MancalaViewModel? ThisMod;
        internal Dictionary<int, SpaceInfo>? SpaceList;
        internal int SpaceSelected;
        internal int SpaceStarted;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<MancalaViewModel>();
        }
        public void OpenMove()
        {
            ThisMod!.Instructions = "Waiting for the move results";
            ThisMod.PiecesAtStart = 0;
            ThisMod.PiecesLeft = 0;
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            SingleInfo = PlayerList!.GetWhoPlayer(); //i think this is needed too.
            ThisMod!.GameBoard1!.LoadSavedBoard(); //hopefully this simple.
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
            ThisMod!.GameBoard1!.ClearBoard();
            SaveRoot!.IsStart = true; //needs this too.
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override async Task ContinueTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            this.ShowTurn();
            if (SaveRoot!.IsStart == true)
            {
                await ThisMod!.GameBoard1!.StartNewTurnAsync();
            }
            else
            {
                ResetMove();
                ThisMod!.Instructions = "Make Move";
                RepaintBoard();
                await base.ContinueTurnAsync();
            }
        }
        public override async Task EndTurnAsync()
        {
            ResetMove();
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            SaveRoot!.IsStart = true;
            ThisMod!.CommandContainer!.ManuelFinish = true;
            await ContinueTurnAsync(); //hopefully this simple.
        }
        internal void RepaintBoard()
        {
            ThisE.RepaintBoard();
        }
        private void ResetMove()
        {
            SpaceSelected = 0;
            SpaceStarted = 0;
            ThisMod!.GameBoard1!.Reset();
        }
        public override Task StartNewTurnAsync()
        {
            return Task.CompletedTask;
        }
        async Task IMoveNM.MoveReceivedAsync(string data)
        {
            OpenMove(); //try this first.
            await ThisMod!.GameBoard1!.AnimateMoveAsync(int.Parse(data));
        }
    }
}