using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MiscProcesses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.GameContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BingoCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.

namespace BingoCP.Logic
{
    [SingletonGame]
    public class BingoMainGameClass : BasicGameClass<BingoPlayerItem, BingoSaveInfo>, IMiscDataNM
    {

        internal BingoItem? CurrentInfo { get; set; }
        private int _currentNum;

        public BingoMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            BingoVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            RandomGenerator rs,
            BasicGameContainer<BingoPlayerItem, BingoSaveInfo> gameContainer

            ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer)
        {
            _model = model;
            _rs = rs;
        }

        private readonly BingoVMData _model;
        private readonly RandomGenerator _rs; //if we don't need, take out.


        internal Action<bool>? SetTimerEnabled { get; set; }

        public override async Task FinishGetSavedAsync()
        {
            if (Check == null)
            {
                throw new BasicBlankException("The checker was never created.  Rethink");
            }
            LoadControls();
            PopulateOwn();
            _currentNum = 0;
            await CallNextNumberAsync();
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            SaveRoot!.BingoBoard.LoadBoard(); //i think.
            IsLoaded = true; //i think needs to be here.
        }
        internal async Task FinishAsync()
        {
            await ComputerTurnAsync();
        }
        public override Task ContinueTurnAsync()
        {
            return Task.CompletedTask; //because no turns this time.
        }
        protected override async Task ComputerTurnAsync()
        {
            await Task.Delay(10);
            var tempList = PlayerList.Where(items => items.PlayerCategory == EnumPlayerCategory.Computer).ToCustomBasicList();
            foreach (var thisPlayer in tempList)
            {
                BingoItem ThisBingo = thisPlayer.BingoList.FirstOrDefault(Items => Items.WhatValue == CurrentInfo!.WhatValue);
                if (ThisBingo != null)
                {
                    ThisBingo.DidGet = true;
                    if (thisPlayer.BingoList.HasBingo == true)
                    {
                        if (BasicData!.MultiPlayer == true)
                            await Network!.SendAllAsync("bingo", thisPlayer.Id);
                        await GameOverAsync(thisPlayer.Id);
                        break;
                    }
                }
            }
            if (BasicData!.MultiPlayer == true)
                await Network!.SendAllAsync("callnextnumber");
            await CallNextNumberAsync();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            CustomBasicList<int> CardList = _rs!.GenerateRandomList(75);
            SaveRoot!.CallList.Clear();
            SaveRoot.BingoBoard.ClearBoard(_model); //i think.
            CardList.ForEach(x =>
            {
                CurrentInfo = new BingoItem();
                CurrentInfo.WhatValue = x;
                CurrentInfo.Vector = new Vector(0, MatchNumber(x));
                CurrentInfo.Letter = WhatLetter(CurrentInfo.Vector.Column);
                SaveRoot.CallList.Add(SaveRoot.CallList.Count + 1, CurrentInfo);
            });
            CreateBingoCards();
            await FinishUpAsync(isBeginning);
            await CallNextNumberAsync(); //maybe i can have here this time.
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "bingo":
                    await GameOverAsync(int.Parse(content));
                    break;
                case "callnextnumber":
                    await CallNextNumberAsync();
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override Task StartNewTurnAsync()
        {
            throw new BasicBlankException("I don't think one starts new turn.  If I am wrong. rethink");
        }

        private void CreateBingoCards()
        {
            _currentNum = 0;
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.BingoList.ClearBoard(); //this may be best.
                NewBingoCard(thisPlayer);
            });
            PopulateOwn();
        }
        private void PopulateOwn()
        {
            SingleInfo = PlayerList!.GetSelf();
            var tempList = SingleInfo.BingoList.Where(ThisBingo => ThisBingo.WhatValue > 0).ToCustomBasicList();
            tempList.ForEach(thisBingo =>
            {
                var boardItem = SaveRoot!.BingoBoard[thisBingo.Vector.Row + 1, thisBingo.Vector.Column];
                boardItem.Text = thisBingo.WhatValue.ToString();
            });
            var finList = SaveRoot!.BingoBoard.Where(Items => Items.Vector.Row == 1).ToCustomBasicList();
            finList.ForEach(thisBingo =>
            {
                thisBingo.Text = WhatLetter(thisBingo.Vector.Column);
            });
        }
        private void NewBingoCard(BingoPlayerItem thisPlayer)
        {
            CustomBasicList<int> thisList;
            int x;
            int y;
            BingoItem thisBingo;
            int starts = 1;
            for (x = 1; x <= 5; x++)
            {
                if (x != 3)
                    y = 5;
                else
                    y = 4;
                thisList = _rs!.GenerateRandomList(starts + 14, y, starts);
                if (x == 3)
                {
                    for (y = 1; y <= 2; y++)
                    {
                        thisBingo = thisPlayer.BingoList[y, x];
                        thisBingo.DidGet = false;
                        thisBingo.Letter = WhatLetter(x);
                        thisBingo.WhatValue = thisList[y - 1];
                    }
                    thisBingo = thisPlayer.BingoList[3, 3];
                    thisBingo.Letter = "N";
                    thisBingo.WhatValue = 0;
                    if (thisPlayer.PlayerCategory == EnumPlayerCategory.Computer)
                        thisBingo.DidGet = true;
                    for (y = 3; y <= 4; y++)
                    {
                        thisBingo = thisPlayer.BingoList[y + 1, x];
                        thisBingo.DidGet = false;
                        thisBingo.Letter = WhatLetter(x);
                        thisBingo.WhatValue = thisList[y - 1];
                    }
                }
                else
                {
                    for (y = 1; y <= 5; y++)
                    {
                        thisBingo = thisPlayer.BingoList[y, x];
                        thisBingo.DidGet = false;
                        thisBingo.Letter = WhatLetter(x);
                        thisBingo.WhatValue = thisList[y - 1];
                    }
                }
                starts += 15;
            }
        }
        public string WhatLetter(int column)
        {
            switch (column)
            {
                case 1:
                    {
                        return "B";
                    }

                case 2:
                    {
                        return "I";
                    }

                case 3:
                    {
                        return "N";
                    }

                case 4:
                    {
                        return "G";
                    }

                default:
                    {
                        return "O";
                    }
            }
        }

        public int MatchNumber(int index)
        {
            switch (index)
            {
                case object _ when index < 16:
                    {
                        return 1;
                    }

                case object _ when index < 31:
                    {
                        return 2;
                    }

                case object _ when index < 46:
                    {
                        return 3;
                    }

                case object _ when index < 61:
                    {
                        return 4;
                    }

                default:
                    {
                        return 5;
                    }
            }
        }

        public async Task GameOverAsync(int player)
        {
            SingleInfo = PlayerList![player];
            if (SetTimerEnabled == null)
            {
                throw new BasicBlankException("The timer processes was never set.  Rethink");
            }
            SetTimerEnabled.Invoke(false);
            await ShowWinAsync();
        }
        internal async Task CallNextNumberAsync()
        {
            if (SetTimerEnabled == null)
            {
                return; //if multiplayer, then client has to call this method after getting the view model.
                //throw new BasicBlankException("Needs to set timer in order to call next number.  Rethink");
            }
            _currentNum++;
            if (_currentNum > 75)
                throw new BasicBlankException("Cannot go higher than 75");
            CurrentInfo = SaveRoot!.CallList[_currentNum];
            _model.NumberCalled = $"{CurrentInfo.Letter}{CurrentInfo.WhatValue}";
            SingleInfo = PlayerList!.GetSelf();
            if (Test!.DoubleCheck == false)
            {
                SetTimerEnabled.Invoke(true);
                await ShowHumanCanPlayAsync();
                //if (ThisData!.MultiPlayer == true)
                //    ThisCheck!.IsEnabled = true;
            }
            else
            {
                await RunTestAsync();
            }

        }

        private async Task RunTestAsync()
        {
            if (_currentNum == 75)
                await UIPlatform.ShowMessageAsync("Done");
            else
            {
                await Task.Delay(200);
                await CallNextNumberAsync();
            }
        }


    }
}
