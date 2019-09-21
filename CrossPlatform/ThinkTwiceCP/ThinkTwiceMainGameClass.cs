using BasicGameFramework.Attributes;
using BasicGameFramework.Dice;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace ThinkTwiceCP
{
    [SingletonGame]
    public class ThinkTwiceMainGameClass : DiceGameClass<SimpleDice, ThinkTwicePlayerItem, ThinkTwiceSaveInfo>, IMiscDataNM, IAdditionalRollProcess
    {
        internal ScoreViewModel? Scores;
        private CategoriesDice? CategoryDice;
        private Multiplier? MultDice;
        public ThinkTwiceMainGameClass(IGamePackageResolver container) : base(container) { }
        private ThinkTwiceViewModel? _thisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            _thisMod = MainContainer.Resolve<ThinkTwiceViewModel>();
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            CategoryDice!.LoadSavedGame();
            MultDice!.LoadSavedGame();
            Scores!.ItemSelected = SaveRoot!.CategorySelected;
            if (SaveRoot.RollNumber > 1)
                _thisMod!.ThisCup!.CanShowDice = true;
            else
                _thisMod!.ThisCup!.HideDice();
            SaveRoot.LoadMod(_thisMod);
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            Scores = MainContainer.Resolve<ScoreViewModel>(); //risk here too.
            CategoryDice = MainContainer.Resolve<CategoriesDice>();
            MultDice = MainContainer.Resolve<Multiplier>();
            SaveRoot!.LoadMod(_thisMod!);
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            SetUpDice();
            PlayerList!.ForEach(items =>
            {
                items.ScoreGame = 0;
                items.ScoreRound = 0;
            });
            SaveRoot!.ImmediatelyStartTurn = true;
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        private void UpdateScore(int whichScore, out bool isGameOver)
        {
            SingleInfo!.ScoreRound = whichScore;
            SingleInfo.ScoreGame += whichScore;
            if (SingleInfo.ScoreGame >= 1000)
                isGameOver = true;
            else
                isGameOver = false;
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //leave warning for now.
            {
                case "itemselected":
                    Scores!.ItemSelected = int.Parse(content);
                    await ContinueTurnAsync();
                    break;
                case "score":
                    SaveRoot!.Score = int.Parse(content);
                    await ContinueTurnAsync();
                    break;
                case "changecategory":
                    await ChangeCategoryHoldAsync();
                    break;
                case "categorydice":
                    var firstList = await CategoryDice!.GetDiceList(content);
                    await RollCatsAsync(firstList);
                    break;
                case "mults":
                    var secondList = await MultDice!.GetDiceList(content);
                    await RollMultsAsync(secondList);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            MultDice!.NewTurn();
            ProtectedStartTurn();
            CategoryDice!.NewTurn();
            SaveRoot!.Score = 0;
            Scores!.ClearBoard(); //try without protected startturn (?)
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            if (CategoryDice!.Hold == true)
            {
                await ContinueTurnAsync();
                return;
            }
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            var newList = CategoryDice.RollDice();
            if (ThisData!.MultiPlayer == true)
                await CategoryDice.SendMessageAsync("categorydice", newList);
            await RollCatsAsync(newList);
        }
        public override async Task EndTurnAsync()
        {
            int whatScore = Scores!.CalculateScore();
            SaveRoot!.Score = whatScore;
            UpdateScore(whatScore, out bool isGameOver);
            CategoryDice!.Visible = false;
            CategoryDice.Hold = false;
            _thisMod!.ThisCup!.CanShowDice = false;
            MultDice!.Visible = false;
            if (ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            _thisMod.ThisCup.UnholdDice();
            if (isGameOver == true)
            {
                await ShowWinAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public async Task RollMultsAsync()
        {
            var thisCol = MultDice!.RollDice();
            if (ThisData!.MultiPlayer == true)
                await MultDice.SendMessageAsync("mults", thisCol);
            await RollMultsAsync(thisCol);
        }
        public async Task RollMultsAsync(CustomBasicList<int> thisCol)
        {
            await MultDice!.ShowRollingAsync(thisCol);
            SaveRoot!.WhichMulti = MultDice.Value; //i think.
            await ContinueTurnAsync();
        }
        public async Task CategoryClickedAsync() //done.
        {
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("changecategory");
            await ChangeCategoryHoldAsync();
        }
        public async Task ScoreClickedAsync()
        {
            int score = Scores!.CalculateScore();
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("score", score);
            SaveRoot!.Score = score; //i think this is it.
        }
        private async Task RollCatsAsync(CustomBasicList<string> newCol)
        {
            await CategoryDice!.ShowRollingAsync(newCol);
            await ContinueTurnAsync();
        }
        public async Task ChangeCategoryHoldAsync()
        {
            CategoryDice!.Hold = !CategoryDice.Hold;
            await ContinueTurnAsync();
        }
        public async Task<bool> CanRollAsync()
        {
            if (Scores!.ItemSelected == -1 && SaveRoot!.RollNumber > 1)
            {
                await _thisMod!.ShowGameMessageAsync("Sorry, you have to select an item in order to continue");
                return false;
            }
            return true;
        }
        public async Task BeforeRollingAsync()
        {
            if (Scores!.ItemSelected > -1 && ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("itemselected", Scores.ItemSelected);
        }
    }
}