using A21DiceGameCP.Data;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace A21DiceGameCP.Logic
{
    [SingletonGame]
    public class A21DiceGameMainGameClass : DiceGameClass<SimpleDice, A21DiceGamePlayerItem, A21DiceGameSaveInfo>
    {


        private readonly A21DiceGameVMData? _model;
        private readonly StandardRollProcesses<SimpleDice, A21DiceGamePlayerItem> _roller;

        public A21DiceGameMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            A21DiceGameVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<A21DiceGamePlayerItem, A21DiceGameSaveInfo> gameContainer,
            StandardRollProcesses<SimpleDice, A21DiceGamePlayerItem> roller) :
            base(mainContainer, aggregator, basicData, test, currentMod, state, delay, command, gameContainer, roller)
        {
            _model = currentMod; //if not needed, take this out and the _model variable.
            _roller = roller;
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice(); //i think
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadMod();
            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelayMilli(300);
            if (SingleInfo!.Score <= 15)
            {
                await _roller!.RollDiceAsync();
                return;
            }
            if (BasicData!.MultiPlayer == true)
                await Network!.SendEndTurnAsync();
            await EndTurnAsync();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SaveRoot!.IsFaceOff = false;
            SetUpDice(); //was this one.
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.IsFaceOff = false;
                thisPlayer.Score = 0;
                thisPlayer.NumberOfRolls = 0;
            });
            await FinishUpAsync(isBeginning);
        }


        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.

            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        protected override async Task ProtectedAfterRollingAsync()
        {
            SingleInfo!.Score += _model!.Cup!.DiceList.Sum(Items => Items.Value);
            if (SaveRoot!.IsFaceOff == true)
            {
                if (PlayerList.Any(Items => Items.IsFaceOff == true && Items.Score == 0))
                {
                    WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                    await StartNewTurnAsync();
                    return;
                }
                await ExtendedFaceOffAsync();
                return;
            }
            SingleInfo.NumberOfRolls++;
            if (SingleInfo.Score > 21)
            {
                await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} is out for going over 21");
                await EndTurnAsync();
                return;
            }
            await ContinueTurnAsync(); //if they get 21, they are responsible for ending turn.
        }
        public override async Task EndTurnAsync()
        {
            int oldTurn;
            oldTurn = WhoTurn;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            int howMany = PlayerList.Count(Items => Items.NumberOfRolls == 0);
            if (howMany == 1)
            {
                SingleInfo = PlayerList.GetWhoPlayer();
                var FirstList = PlayerList.ToCustomBasicList();
                FirstList.RemoveSpecificItem(SingleInfo);
                if (FirstList.Any(Items => Items.Score <= 21) == false)
                {
                    await ShowWinAsync();
                    return;
                }
            }
            else if (howMany == 0)
            {
                await StartEndAsync();
                return;
            }
            await StartNewTurnAsync();
        }
        #region "Unique Game Features"

        private async Task StartEndAsync()
        {
            var firstList = PlayerList.Where(Items => Items.Score <= 21).OrderByDescending(Items => Items.Score).ThenBy(Items => Items.NumberOfRolls).ToCustomBasicList();
            if (firstList.Count == 1 || firstList.First().Score > firstList[1].Score)
            {
                SingleInfo = firstList.First();
                await ShowWinAsync();
                return;
            }
            if (firstList.First().Score == firstList[1].Score && firstList.First().NumberOfRolls < firstList[1].NumberOfRolls)
            {
                SingleInfo = firstList.First();
                await ShowWinAsync();
                return;
            }
            SaveRoot!.IsFaceOff = true;
            WhoTurn = firstList.First().Id;
            var newList = PlayerList.Where(Items => Items.Score != firstList.First().Score && Items.NumberOfRolls != firstList.First().NumberOfRolls).ToCustomBasicList();
            newList.ForEach(items =>
            {
                items.Score = 0;
                items.InGame = false;
            }); //hopefully it puts them into game to start with for new game.  if not, rethink.
            newList = PlayerList.Where(Items => Items.InGame == true).ToCustomBasicList();
            newList.ForEach(Items => Items.Score = 0);
            await StartNewTurnAsync();
        }
        private async Task ExtendedFaceOffAsync()
        {
            var thisList = PlayerList.Where(Items => Items.IsFaceOff == true).OrderByDescending(Items => Items.Score).ToCustomBasicList();
            if (thisList.Count < 2)
                throw new BasicBlankException("Must have at least 2 players for faceoff");
            if (thisList.First().Score > thisList[1].Score)
            {
                SingleInfo = thisList.First();
                await ShowWinAsync();
                return;
            }
            var newList = PlayerList.Where(Items => Items.IsFaceOff == true && Items.Score < thisList.First().Score).ToCustomBasicList();
            newList.ForEach(items =>
            {
                items.IsFaceOff = false;
                items.InGame = false;
                items.Score = 0;
            });
            newList = PlayerList.Where(Items => Items.IsFaceOff == true).ToCustomBasicList();
            newList.ForEach(Items => Items.Score = 0);
            WhoTurn = thisList.First().Id;
            await StartNewTurnAsync();
        }
        #endregion

    }
}