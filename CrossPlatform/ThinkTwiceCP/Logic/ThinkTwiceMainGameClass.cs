using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThinkTwiceCP.Data;

namespace ThinkTwiceCP.Logic
{
    [SingletonGame]
    public class ThinkTwiceMainGameClass : DiceGameClass<SimpleDice, ThinkTwicePlayerItem, ThinkTwiceSaveInfo>, IMiscDataNM
    {


        private readonly ThinkTwiceVMData _model;
        private readonly ScoreLogic _scoreLogic;
        private readonly CategoriesDice _categoriesDice;
        private readonly Multiplier _multiplier;

        public ThinkTwiceMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            ThinkTwiceVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            ThinkTwiceGameContainer gameContainer,
            StandardRollProcesses<SimpleDice, ThinkTwicePlayerItem> roller,
            ScoreLogic scoreLogic,
            CategoriesDice categoriesDice,
            Multiplier multiplier
            ) :
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            _scoreLogic = scoreLogic;
            _categoriesDice = categoriesDice;
            _multiplier = multiplier;
            roller.BeforeRollingAsync = BeforeRollingAsync;
            roller.CanRollAsync = CanRollAsync;
            gameContainer.ScoreClickAsync = ScoreClickedAsync;
            gameContainer.CategoryClicked = CategoryClickedAsync;
        }

        public async Task BeforeRollingAsync()
        {
            if (_model.ItemSelected > -1 && BasicData.MultiPlayer)
            {
                _model.ItemSent = true;
                await Network!.SendAllAsync("itemselected", _model.ItemSelected);
            }
        }
        public async Task<bool> CanRollAsync()
        {
            if (_model!.ItemSelected == -1 && SaveRoot!.RollNumber > 1)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you have to select an item in order to continue");
                return false;
            }
            return true;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            _categoriesDice.LoadSavedGame();
            _multiplier.LoadSavedGame();
            _model.ItemSelected = SaveRoot.CategorySelected;
            if (SaveRoot.RollNumber > 1)
            {
                _model.Cup!.CanShowDice = true;
            }
            else
            {
                _model.Cup!.HideDice();
            }
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadMod();
            SaveRoot.LoadMod(_model);
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
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            PlayerList!.ForEach(items =>
            {
                items.ScoreGame = 0;
                items.ScoreRound = 0;
            });
            SaveRoot!.ImmediatelyStartTurn = true;
            await FinishUpAsync(isBeginning);
        }

        private void UpdateScore(int whichScore, out bool isGameOver)
        {
            SingleInfo!.ScoreRound = whichScore;
            SingleInfo.ScoreGame += whichScore;
            if (SingleInfo.ScoreGame >= 1000)
            {
                isGameOver = true;

            }
            else
            {
                isGameOver = false;
            }
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "itemselected":
                    _model.ItemSelected = int.Parse(content);
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
                    var firstList = await _categoriesDice.GetDiceList(content);
                    await RollCatsAsync(firstList);
                    break;
                case "mults":
                    var secondList = await _multiplier.GetDiceList(content);
                    await RollMultsAsync(secondList);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            _multiplier.NewTurn();
            ProtectedStartTurn();
            _categoriesDice.NewTurn();
            _model.ItemSent = false;
            SaveRoot!.Score = 0;
            _model!.ClearBoard(); //try without protected startturn (?)
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            if (_categoriesDice!.Hold == true)
            {
                await ContinueTurnAsync();
                return;
            }
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.OtherHuman)
            {
                Check!.IsEnabled = true;
                return;
            }
            var newList = _categoriesDice.RollDice();
            if (BasicData!.MultiPlayer == true)
            {
                await _categoriesDice.SendMessageAsync("categorydice", newList);
            }
            await RollCatsAsync(newList);
        }
        public override async Task EndTurnAsync()
        {
            int whatScore = _scoreLogic!.CalculateScore();
            SaveRoot!.Score = whatScore;
            UpdateScore(whatScore, out bool isGameOver);
            _categoriesDice!.Visible = false;
            _categoriesDice.Hold = false;
            _model.Cup!.CanShowDice = false;
            _multiplier.Visible = false;
            if (Test!.NoAnimations == false)
            {
                await Delay!.DelaySeconds(.75);
            }
            _model.Cup.UnholdDice();
            if (isGameOver == true || Test!.ImmediatelyEndGame)
            {
                await ShowWinAsync();
                return;
            }
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        public async Task RollMultsAsync()
        {
            var thisCol = _multiplier!.RollDice();
            if (BasicData!.MultiPlayer == true)
            {
                await _multiplier.SendMessageAsync("mults", thisCol);
            }
            await RollMultsAsync(thisCol);
        }
        public async Task RollMultsAsync(CustomBasicList<int> thisCol)
        {
            await _multiplier.ShowRollingAsync(thisCol);
            SaveRoot!.WhichMulti = _multiplier.Value; //i think.
            await ContinueTurnAsync();
        }
        public async Task CategoryClickedAsync() //done.
        {
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("changecategory");
            }
            await ChangeCategoryHoldAsync();
        }
        public async Task ScoreClickedAsync()
        {
            int score = _scoreLogic.CalculateScore();
            if (BasicData.MultiPlayer == true)
            {
                await Network!.SendAllAsync("score", score);
            }
            SaveRoot!.Score = score; //i think this is it.
        }
        private async Task RollCatsAsync(CustomBasicList<string> newCol)
        {
            await _categoriesDice.ShowRollingAsync(newCol);
            await ContinueTurnAsync();
        }
        public async Task ChangeCategoryHoldAsync()
        {
            _categoriesDice.Hold = !_categoriesDice.Hold;
            await ContinueTurnAsync();
        }
    }
}
