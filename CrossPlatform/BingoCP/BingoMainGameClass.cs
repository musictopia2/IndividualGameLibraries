using BasicGameFramework.Attributes;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MiscProcesses;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BingoCP
{
    [SingletonGame]
    public class BingoMainGameClass : BasicGameClass<BingoPlayerItem, BingoSaveInfo>, IMiscDataNM
    {
        internal BingoItem? CurrentInfo;
        private int _currentNum;
        private readonly BingoViewModel _thisMod;

        public BingoMainGameClass(IGamePackageResolver _Container) : base(_Container)
        {
            _thisMod = MainContainer.Resolve<BingoViewModel>();
        }

        public override async Task FinishGetSavedAsync()
        {
            if (ThisCheck == null)
                throw new BasicBlankException("The check never got created.  rethink");
            if (IsLoaded == false)
                LoadControls();
            //anything else needed is here.
            PopulateOwn();
            _currentNum = 0;

            await CallNextNumberAsync();
        }
        private RandomGenerator? _rs;
        private void LoadControls()
        {
            SaveRoot!.BingoBoard.LoadBoard(); //i think.
            _rs = MainContainer.Resolve<RandomGenerator>();
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
                        if (ThisData!.MultiPlayer == true)
                            await ThisNet!.SendAllAsync("bingo", thisPlayer.Id);
                        await GameOverAsync(thisPlayer.Id);
                        break;
                    }
                }
            }
            if (ThisData!.MultiPlayer == true)
                await ThisNet!.SendAllAsync("callnextnumber");
            await CallNextNumberAsync();
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            if (isBeginning == true)
                LoadControls();
            CustomBasicList<int> CardList = _rs!.GenerateRandomList(75);
            SaveRoot!.CallList.Clear();
            SaveRoot.BingoBoard.ClearBoard(_thisMod); //i think.
            CardList.ForEach(x =>
            {
                CurrentInfo = new BingoItem();
                CurrentInfo.WhatValue = x;
                CurrentInfo.Vector = new Vector(0, MatchNumber(x));
                CurrentInfo.Letter = WhatLetter(CurrentInfo.Vector.Column);
                SaveRoot.CallList.Add(SaveRoot.CallList.Count + 1, CurrentInfo);
            });
            CreateBingoCards();
            await ThisLoader!.FinishUpAsync(isBeginning);
            await CallNextNumberAsync(); //maybe i can have here this time.
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
            _thisMod.Timer1!.Enabled = false;
            await ShowWinAsync();
        }
        internal async Task CallNextNumberAsync()
        {
            _currentNum++;
            if (_currentNum > 75)
                throw new BasicBlankException("Cannot go higher than 75");
            CurrentInfo = SaveRoot!.CallList[_currentNum];
            _thisMod.NumberCalled = $"{CurrentInfo.Letter}{CurrentInfo.WhatValue}";
            SingleInfo = PlayerList!.GetSelf();
            if (ThisTest!.DoubleCheck == false)
            {
                _thisMod.Timer1!.Enabled = true;
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
                await _thisMod.ShowGameMessageAsync("Done");
            else
            {
                await Task.Delay(200);
                await CallNextNumberAsync();
            }
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
    }
}