using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFramework.Attributes;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.NetworkingClasses.Extensions;

namespace BowlingDiceGameCP
{
    [SingletonGame]
    public class BowlingDiceGameMainGameClass : BasicGameClass<BowlingDiceGamePlayerItem, BowlingDiceGameSaveInfo>, IRolledNM
    {
        public BowlingDiceGameMainGameClass(IGamePackageResolver container) : base(container) { }
        public BowlingScoresCP? ScoreSheets;
        public BowlingDiceSet? DiceBoard;
        private BowlingDiceGameViewModel? _thisMod;

        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
			base.Init();
            _thisMod = MainContainer.Resolve<BowlingDiceGameViewModel>();
        }
        public override async Task PopulateSaveRootAsync()
        {
            SaveRoot!.DiceData = await DiceBoard!.SaveGameAsync();
        }
        public override async Task FinishGetSavedAsync()
        {
            LoadControls();
            await DiceBoard!.LoadGameAsync(SaveRoot!.DiceData);
            SaveRoot.LoadMod(_thisMod!);
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
            SaveRoot!.LoadMod(_thisMod!);
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
            await ThisLoader!.FinishUpAsync(isBeginning);
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

        public async Task RollDiceAsync()
        {
            if (ScoreSheets!.NeedToClear() == true)
            {
                DiceBoard!.ClearDice();
                await Delay!.DelaySeconds(0.1);
            }
            var thisList = DiceBoard!.RollDice();
            if (SingleInfo!.CanSendMessage(ThisData!) == true)
                await DiceBoard.SendMessageAsync(thisList);
            await RollDiceAsync(thisList);
        }
        protected override async Task ComputerTurnAsync()
        {
            if (ThisData!.MultiPlayer == true && ThisData.Client == true)
                throw new BasicBlankException("Clients can't go for the comptuer for multiplayer games");
            if (ThisTest!.NoAnimations == false)
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
            if (ThisTest.NoAnimations == false)
                await Delay!.DelayMilli(300);
            if (ThisData.MultiPlayer == true)
                await ThisNet!.SendEndTurnAsync();
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
    }
}