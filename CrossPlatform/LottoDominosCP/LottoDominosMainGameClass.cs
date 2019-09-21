using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DIContainers;
using BasicGameFramework.Dominos;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LottoDominosCP
{
    [SingletonGame]
    public class LottoDominosMainGameClass : BasicGameClass<LottoDominosPlayerItem, LottoDominosSaveInfo>, IMoveNM, IMiscDataNM
    {

        public GameBoardCP GameBoard1;
        private readonly LottoDominosViewModel _thisMod;
        public LottoDominosMainGameClass(IGamePackageResolver container) : base(container)
        {
            GameBoard1 = MainContainer.Resolve<GameBoardCP>();
            GameBoard1.Text = "Dominos";
            _thisMod = MainContainer.Resolve<LottoDominosViewModel>(); //since this worked, might as well do here this time.
        }
        public async Task MakeMoveAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendMoveAsync(deck);
            await GameBoard1.ShowDominoAsync(deck);
            if (CanPlay(deck) == false)
            {
                AddComputer(deck);
                await EndTurnAsync();
                return;
            }
            TakeOffComputer(deck);
            GameBoard1.MakeInvisible(deck);
            SingleInfo!.NumberWon++;
            if (IsGameOver(SingleInfo.NumberWon) == true)
            {
                await ShowWinAsync();
                return;
            }
            await EndTurnAsync();
        }
        public CustomBasicList<int> GetNumberList()
        {
            CustomBasicList<int> output = new CustomBasicList<int>();
            for (int x = 0; x <= 6; x++)
            {
                if (PlayerList.Any(Items => Items.NumberChosen == x) == false)
                    output.Add(x);
            }
            return output;
        }
        public async Task ProcessNumberAsync(int numberToChoose)
        {
            SingleInfo!.NumberChosen = numberToChoose;
            _thisMod.Number1!.SelectNumberValue(numberToChoose);
            if (SingleInfo.CanSendMessage(ThisData!) == true)
                await ThisNet!.SendAllAsync("numberchosen", numberToChoose);
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self && ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            await EndTurnAsync();
        }
        public override Task FinishGetSavedAsync()
        {
            if (IsLoaded == false)
                LoadControls();
            GameBoard1!.LoadSavedGame(SaveRoot!.BoardDice!);
            _thisMod.DominosList!.ClearObjects(); //has to clear objects first.
            _thisMod.DominosList.OrderedObjects(); //i think this should be fine.
            if (ThisData!.Client == true)
                SaveRoot.ComputerList.Clear(); //because they don't need to know about computer list.
            if (SaveRoot.GameStatus == EnumStatus.NormalPlay)
                GameBoard1.Visible = true;
            return Task.CompletedTask;
        }
        private void LoadControls() { }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (IsLoaded == false)
                LoadControls();
            PlayerList!.ForEach(items =>
            {
                items.NumberChosen = -1;
                items.NumberWon = 0;
            });
            _thisMod.DominosList!.ClearObjects();
            _thisMod.DominosList.ShuffleObjects(); //i think.
            SaveRoot!.ComputerList.Clear();
            SaveRoot.GameStatus = EnumStatus.ChooseNumber;
            GameBoard1.ClearPieces(); //i think
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.BoardDice = GameBoard1!.ObjectList.ToRegularDeckDict();
            return Task.CompletedTask;
        }
        public override Task ContinueTurnAsync()
        {
            if (SaveRoot!.GameStatus == EnumStatus.ChooseNumber)
                _thisMod.ReloadLists();
            return base.ContinueTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            this.ShowTurn();
            if (SingleInfo!.NumberChosen > -1)
                SaveRoot!.GameStatus = EnumStatus.NormalPlay;
            await ContinueTurnAsync();
        }
        public override async Task EndTurnAsync()
        {
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public async Task MoveReceivedAsync(string data)
        {
            await MakeMoveAsync(int.Parse(data));
        }
        public async Task MiscDataReceived(string status, string content)
        {
            if (status == "numberchosen")
                await ProcessNumberAsync(int.Parse(content));
            else
                throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
        }
        public bool CanPlay(int deck)
        {
            if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer && ThisTest!.AllowAnyMove == true)
                return true; //because we are allowing any move for testing.
            SimpleDominoInfo thisDomino = _thisMod.DominosList!.GetSpecificItem(deck);
            return thisDomino.FirstNum == SingleInfo.NumberChosen || thisDomino.SecondNum == SingleInfo.NumberChosen;
        }
        private bool IsGameOver(int score)
        {
            return score >= 4;
        }
        #region "Computer Processes"
        private void AddComputer(int deck)
        {
            if (ThisData!.Client == true)
                return; //host does computer.
            if (SaveRoot!.ComputerList.ObjectExist(deck) == false)
            {
                SimpleDominoInfo thisDomino = _thisMod.DominosList!.GetSpecificItem(deck);
                SaveRoot.ComputerList.Add(thisDomino);
            }
        }
        public void TakeOffComputer(int deck)
        {
            if (ThisData!.Client == true)
                return;
            if (SaveRoot!.ComputerList.ObjectExist(deck) == false)
                return;
            SaveRoot.ComputerList.RemoveObjectByDeck(deck);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SaveRoot!.GameStatus == EnumStatus.ChooseNumber)
            {
                await ProcessNumberAsync(_thisMod.Number1!.NumberToChoose()); //i think
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
            output = GameBoard1.GetVisibleList();
            await MakeMoveAsync(output.GetRandomItem().Deck);
        }
        #endregion
    }
}