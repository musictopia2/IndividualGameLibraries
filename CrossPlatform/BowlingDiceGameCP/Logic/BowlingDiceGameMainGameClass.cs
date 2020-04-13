using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using BowlingDiceGameCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BowlingDiceGameCP.Logic
{
    [SingletonGame]
    public class BowlingDiceGameMainGameClass : BasicGameClass<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>, IRolledNM
    {
        public BowlingDiceGameMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            BowlingDiceGameVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            BasicGameContainer<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo> gameContainer
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer)
        {
            _test = test;
        }

        private readonly TestOptions _test;
        public BowlingScoresCP? ScoreSheets;
        public BowlingDiceSet? DiceBoard;
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.DiceData = await DiceBoard!.SaveGameAsync();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            await DiceBoard!.LoadGameAsync(SaveRoot!.DiceData);
        }
        public async Task RollReceivedAsync(string data)
        {
            if (ScoreSheets!.NeedToClear() == true)
            {
                DiceBoard!.ClearDice();
                await Delay!.DelaySeconds(0.1);
            }
            var ThisList = await DiceBoard!.GetDiceList(data);
            await RollDiceAsync(ThisList);
        }
        private async Task GameOverAsync()
        {
            SingleInfo = PlayerWon();
            await ShowWinAsync();
        }
        private BowlingDiceGamePlayerItem PlayerWon()
        {
            return PlayerList!.OrderByDescending(items => items.TotalScore).Take(1).Single();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            ScoreSheets = MainContainer.Resolve<BowlingScoresCP>();
            DiceBoard = MainContainer.Resolve<BowlingDiceSet>();
            DiceBoard.FirstLoad(); //forgot this.
            IsLoaded = true; //i think needs to be here.
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            SaveRoot!.WhatFrame = 1;
            if (isBeginning == true)
                LoadPlayerFrames();
            LoadControls();
            ScoreSheets!.ClearBoard();
            DiceBoard!.ClearDice();
            SaveRoot.WhichPart = 1;
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            await FinishUpAsync(isBeginning);
        }

        private void LoadPlayerFrames()
        {
            PlayerList!.ForEach(items =>
            {
                10.Times(x =>
                {
                    FrameInfoCP thisFrame = new FrameInfoCP();
                    3.Times(y =>
                    {
                        SectionInfoCP thisSection = new SectionInfoCP();
                        thisSection.Score = "0";
                        thisFrame.SectionList.Add(y, thisSection);
                    });
                    items.FrameList.Add(x, thisFrame);
                });
            });
        }

        public override async Task EndTurnAsync()
        {
            ScoreSheets!.UpdateScore();
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            if (WhoTurn == WhoStarts)
            {
                SaveRoot!.WhatFrame++;
                if (SaveRoot.WhatFrame > 10)
                {
                    await GameOverAsync();
                    return;
                }
            }
            await StartNewTurnAsync();
        }
        public override async Task StartNewTurnAsync()
        {
            PrepStartTurn(); //anything else is below.
            DiceBoard!.ClearDice();
            SaveRoot!.WhichPart = 1;
            SaveRoot.IsExtended = false;
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }

        protected override async Task ComputerTurnAsync()
        {
            if (BasicData!.MultiPlayer == true && BasicData.Client == true)
                throw new BasicBlankException("Clients can't go for the comptuer for multiplayer games");
            if (_test!.NoAnimations == false)
                await Delay!.DelayMilli(300);
            if (SaveRoot!.WhichPart < 3)
            {
                await RollDiceAsync();
                return;
            }
            if (SaveRoot.IsExtended == true)
            {
                SaveRoot.IsExtended = false;
                await RollDiceAsync();
                return;
            }
            if (_test.NoAnimations == false)
                await Delay!.DelayMilli(300);
            if (BasicData.MultiPlayer == true)
                await Network!.SendEndTurnAsync();
            await EndTurnAsync();
        }
        public async Task RollDiceAsync(CustomBasicList<CustomBasicList<bool>> thisList)
        {
            await DiceBoard!.ShowRollingAsync(thisList);
            int previouss;
            int hits;
            hits = DiceBoard.HowManyHit();
            previouss = ScoreSheets!.PreviousHit();
            int newOnes;
            newOnes = hits - previouss;
            ScoreSheets.UpdateForSection(newOnes);
            SaveRoot!.WhichPart += 1;
            if (SaveRoot.WhichPart == 3)
            {
                if (ScoreSheets.CanExtend() == true)
                    SaveRoot.IsExtended = true; // extended is for ui only.
                else
                    SaveRoot.IsExtended = false;
            }
            else
                SaveRoot.IsExtended = false;
            await ContinueTurnAsync();
        }
        public async Task RollDiceAsync()
        {
            if (ScoreSheets!.NeedToClear() == true)
            {
                DiceBoard!.ClearDice();
                await Delay!.DelaySeconds(0.1);
            }
            var thisList = DiceBoard!.RollDice();
            if (SingleInfo!.CanSendMessage(BasicData!) == true)
                await DiceBoard.SendMessageAsync(thisList);
            await RollDiceAsync(thisList);
        }
    }
}