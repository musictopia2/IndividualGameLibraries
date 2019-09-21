using BasicGameFramework.Attributes;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DIContainers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BuncoDiceGameCP
{
    [SingletonGame]
    public class BuncoDiceGameMainGameClass
    {
        public IGamePackageResolver MainContainer { get; set; }
        public readonly ISaveSinglePlayerClass MainSave;
        public BuncoDiceGameSaveInfo? SaveRoot;
        private bool _mVarHadBunco;
        internal PlayerItem? CurrentPlayer;
        private readonly EventAggregator _thisE;
        private readonly RandomGenerator _rs;
        private readonly BuncoDiceGameViewModel _thisMod;
        public BuncoDiceGameMainGameClass(IGamePackageResolver container)
        {
            MainContainer = container;
            MainSave = MainContainer.Resolve<ISaveSinglePlayerClass>();
            _thisE = MainContainer.Resolve<EventAggregator>();
            _rs = MainContainer.Resolve<RandomGenerator>();
            _thisMod = MainContainer.Resolve<BuncoDiceGameViewModel>();
        }

        public int FindTeamPlayer(int whatPlayer, bool showError)
        {
            PlayerItem currentPlayer;
            currentPlayer = SaveRoot!.PlayerList[whatPlayer];
            int yourteam;
            int yourtable;
            yourteam = currentPlayer.Team;
            yourtable = currentPlayer.Table;
            foreach (var newPlayer in SaveRoot.PlayerList)
            {
                if (newPlayer.Team == yourteam && yourtable == newPlayer.Table && newPlayer.Id != whatPlayer)
                    return newPlayer.Id;
            }
            if (showError == true)
                throw new Exception("Cannot find the team for " + whatPlayer);
            return 0; //
        }
        public void ReceivedBunco()
        {
            CurrentPlayer!.Buncos++; //hopefully can keep track of it (?)
        }
        public bool DidHaveBunco()
        {
            return _mVarHadBunco;
        }
        public void UpdateScores(int score)
        {
            int newScore;
            newScore = CurrentPlayer!.Points + score;
            PlayerItem newPlayer;
            int newNum;
            CurrentPlayer.Points = newScore;
            if (CurrentPlayer.PlayerCategory == EnumPlayerCategory.Self)
                SaveRoot!.ThisStats.YourPoints = newScore;
            newNum = FindTeamPlayer(SaveRoot!.PlayOrder!.WhoTurn, true);
            newPlayer = SaveRoot.PlayerList[newNum];
            newPlayer.Points = newScore;
        }
        private bool HasThree()
        {
            return SaveRoot!.DiceList.DistinctCount(Items => Items.Value) == 1; //needs to be 1 distinct.
        }
        private bool HasBunco()
        {
            if (HasThree() == false)
                return false;
            int tempValue = SaveRoot!.DiceList.First().Value;
            return tempValue == SaveRoot.WhatNumber;
        }
        private int HowMany()
        {
            return SaveRoot!.DiceList.Count(Items => Items.Value == SaveRoot.WhatNumber);
        }
        public int ScoreRoll()
        {
            _mVarHadBunco = HasBunco();
            if (_mVarHadBunco == true)
                return 5;
            if (HasThree() == true)
                return 5;
            return HowMany();
        }
        public async Task RollDiceAsync()
        {
            var thisList = _thisMod.ThisCup!.RollDice();
            await _thisMod.ThisCup.ShowRollingAsync(thisList, SaveRoot!.SameTable);
        }

        public async Task ProcessNewRoundAsync()
        {
            _thisMod.CommandContainer!.ManuelFinish = true;
            int teammate;
            SaveRoot!.ThisStats.YourPoints = 0;
            SaveRoot.ThisStats.OpponentScore = 0;
            SaveRoot.PlayerList.ForEach(SingleInfo =>
            {
                SingleInfo.Points = 0;
                SingleInfo.WinDetermined = false;
                SingleInfo.PlayerNum = -1;
                SingleInfo.Acceptable = false;
                teammate = FindTeamPlayer(SingleInfo.Id, true); //no -1 anymore.
                SingleInfo.PreviousMate = teammate;
                if (SingleInfo.WonPrevious == true && SingleInfo.Table > 1)
                    SingleInfo.TempTable = SingleInfo.Table - 1;
                else if (SingleInfo.WonPrevious == false && SingleInfo.Table < 3)
                    SingleInfo.TempTable = SingleInfo.Table + 1;
                else
                    SingleInfo.TempTable = SingleInfo.Table;
            });
            SaveRoot.PlayerList.ForEach(Singleinfo =>
            {
                Singleinfo.Table = Singleinfo.TempTable;
                Singleinfo.TempTable = 0;
            });
            await StartRoundAsync();
        }
        public async Task EndTurnAsync()
        {
            _thisMod.CommandContainer!.ManuelFinish = true; //now has to manually be done.
            _thisMod.CanEndTurn = false;
            _thisMod.AlreadyReceivedBunco = false;
            SaveRoot!.PlayOrder!.WhoTurn = await SaveRoot.PlayerList.CalculateWhoTurnAsync();
            await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot);
            await NewTurnAsync(); //i think
        }
        private async Task ReloadGameAsync()
        {
            SaveRoot = await MainSave.RetrieveSinglePlayerGameAsync<BuncoDiceGameSaveInfo>();
            MainContainer.ReplaceObject(SaveRoot); //this is now the new object.
            SaveRoot.PlayerList.MainContainer = MainContainer; //has to redo that part.
            SaveRoot.PlayerList.AutoSaved(SaveRoot!.PlayOrder!); //i think this is needed too.
            _thisMod.LoadCup(SaveRoot, true);
            await _thisE.SendLoadAsync(); //still needed.
            await NewTurnAsync();
        }
        public async Task StartGameAsync()
        {
            if (await MainSave.CanOpenSavedSinglePlayerGameAsync() == true)
            {
                await ReloadGameAsync();
                return;
            }
            SaveRoot = MainContainer.Resolve<BuncoDiceGameSaveInfo>();
            SaveRoot.PlayOrder = (PlayOrderClass)MainContainer.Resolve<IPlayOrder>(); //has to be here so they both use the same object.
            SaveRoot.PlayerList = new PlayerCollection<PlayerItem>();
            SaveRoot.PlayerList.LoadPlayers(1, 11);
            SaveRoot.PlayerList.FinishLoading(); //i think we will go ahead and shuffle the players.
            SaveRoot.PlayerList.ForEach(singleInfo =>
            {
                singleInfo.Buncos = 0;
                singleInfo.Wins = 0;
                singleInfo.Losses = 0;
                singleInfo.PlayerNum = -1;
                singleInfo.Points = 0;
                singleInfo.WinDetermined = false;
                singleInfo.Acceptable = false;
                singleInfo.PreviousMate = -1;
                singleInfo.Team = 0;
                singleInfo.Table = 0;
                singleInfo.WonPrevious = false;
            });
            SaveRoot.WhatSet = 1;
            SaveRoot.WhatNumber = 1;
            SaveRoot.ThisStats.Status = "Game In Progress";
            _thisMod.LoadCup(SaveRoot, false);
            await _thisE.SendLoadAsync();
            await StartRoundAsync();
        }

        private PlayerItem FindSpecificPlayer(int whatteam, int whattable)
        {
            return SaveRoot!.PlayerList.Where(items => items.Team == whatteam &&
            items.Table == whattable && items.PlayerNum == -1).First();
        }

        private async Task NewTurnAsync()
        {
            CurrentPlayer = SaveRoot!.PlayerList.GetWhoPlayer();
            SaveRoot.ThisStats.Turn = CurrentPlayer.NickName;
            if (CurrentPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                SaveRoot.SameTable = true;
                SaveRoot.HadBunco = false;
                SaveRoot.HasRolled = false;
                _thisMod.CommandContainer!.ManuelFinish = false;
                _thisMod.CommandContainer.IsExecuting = false; 
                return;
            }
            await ComputerTurnAsync();
        }
        private async Task ComputerTurnAsync()
        {
            bool endRound = false;
            int thisscore;
            int thistable;
            thistable = CurrentPlayer!.Table;
            int mytable;
            PlayerItem humanPlayer;
            humanPlayer = SaveRoot!.PlayerList.GetSelf();
            mytable = humanPlayer.Table;
            int teamplayer;
            if (mytable == thistable)
                SaveRoot.SameTable = true;
            else
                SaveRoot.SameTable = false;
            int humanopponent;
            do
            {
                if (CurrentPlayer.Points >= 21 && thistable == 1)
                {
                    endRound = true;
                    break;
                }
                await RollDiceAsync();
                if (SaveRoot.SameTable == true)
                    await Task.Delay(1000);
                thisscore = ScoreRoll();
                if (_mVarHadBunco == true)
                {
                    thisscore = 21;
                    if (thistable == 1)
                        endRound = true;
                }
                if (thisscore == 21)
                    ReceivedBunco();
                UpdateScores(thisscore);
                if (CurrentPlayer.Points >= 21 && thistable == 1)
                    endRound = true;
                teamplayer = FindTeamPlayer(SaveRoot!.PlayOrder!.WhoTurn, true);
                if (teamplayer == humanPlayer.Id) //i think
                    SaveRoot.ThisStats.YourPoints = CurrentPlayer.Points;
                humanopponent = CalculateOpponentScore(humanPlayer.Id); //i think this simple (?)
                SaveRoot.ThisStats.OpponentScore = humanopponent;
                await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot); //i think its this simple.
                if (endRound == true)
                    break;
                if (thisscore == 0)
                    break;
            }
            while (true);// because it has to immediately end round.  does not get another turn.// can't do old fashioned pauses because no doevents when doing crossplatform.
            if (endRound == false)
            {
                await EndTurnAsync();
                return;
            }
            await FinishRoundAsync();
        }

        private int CalculateOpponentScore(int whatPlayer)
        {
            PlayerItem tempPlayer1;
            tempPlayer1 = SaveRoot!.PlayerList[whatPlayer];
            int yourtable;
            int yourteam;
            yourtable = tempPlayer1.Table;
            yourteam = tempPlayer1.Team;
            foreach (var tempPlayer in SaveRoot.PlayerList)
            {
                if (tempPlayer.Table == yourtable && tempPlayer.Team != yourteam)
                    return tempPlayer.Points;
            }
            throw new Exception("Could not find an opponent for player " + whatPlayer);
        }

        private bool HasWon(int whatPlayer)
        {
            int yourscore;
            int opponentscore;
            PlayerItem tempPlayer;
            tempPlayer = SaveRoot!.PlayerList[whatPlayer];
            yourscore = tempPlayer.Points;
            opponentscore = CalculateOpponentScore(whatPlayer);
            if (yourscore > opponentscore)
                return true;
            if (yourscore < opponentscore)
                return false;
            int teammate;
            teammate = FindTeamPlayer(whatPlayer, true);
            tempPlayer = SaveRoot.PlayerList[teammate];
            if (tempPlayer.WinDetermined == true)
            {
                if (tempPlayer.WonPrevious == true)
                    return true;
                return false;
            }
            bool opponentwin;
            bool determined = false;
            opponentwin = DidOpponentWin(whatPlayer, ref determined);
            if (determined == true)
            {
                if (opponentwin == true)
                    return false;
                return true;
            }
            int ask1;
            ask1 = _rs.GetRandomNumber(2);
            tempPlayer = SaveRoot.PlayerList[ask1];
            tempPlayer.WinDetermined = true;
            if (ask1 == 1)
                return true;
            tempPlayer.WinDetermined = false; //maybe (?)
            return false;
        }
        private bool DidOpponentWin(int whatPlayer, ref bool didFind)
        {
            didFind = false;
            PlayerItem tempPlayer1;
            tempPlayer1 = SaveRoot!.PlayerList[whatPlayer];
            int whattable;
            int whatteam;
            whattable = tempPlayer1.Table;
            whatteam = tempPlayer1.Team;
            foreach (var tempPlayer in SaveRoot.PlayerList)
            {
                if (tempPlayer.Table == whattable && tempPlayer.Team != whatteam)
                {
                    didFind = true;
                    return tempPlayer.WonPrevious;
                }
            }
            return false;
        }
        public async Task FinishRoundAsync()
        {
            SaveRoot!.ThisStats.Turn = "None";
            SaveRoot.PlayerList.ForEach(Items =>
            {
                if (HasWon(Items.Id) == true)
                {
                    Items.WonPrevious = true;
                    Items.Wins++;
                }
                else
                {
                    Items.WonPrevious = false;
                    Items.Losses++;
                }
                if (Items.PlayerCategory == EnumPlayerCategory.Self)
                {
                    SaveRoot.ThisStats.Wins = Items.Wins;
                    SaveRoot.ThisStats.Losses = Items.Losses;
                }
            });
            if (SaveRoot.WhatNumber < 6)
            {
                SaveRoot.WhatNumber += 1;
                _thisMod.NewGameVisible = true;
                _thisMod.CommandContainer!.IsExecuting = false;
            }
            else
            {
                SaveRoot.WhatSet += 1;
                if (SaveRoot.WhatSet < 5)
                {
                    _thisMod.NewGameVisible = true;
                    _thisMod.CanEndTurn = true;
                    SaveRoot.WhatNumber = 1;
                    _thisMod.CommandContainer!.IsExecuting = false;
                }
                else
                {
                    SaveRoot.ThisStats.Status = "Game is over";
                    await MainSave.DeleteSinglePlayerGameAsync();
                }
            }
        }

        private async Task StartRoundAsync()
        {
            SaveRoot!.ThisStats.Set = SaveRoot.WhatSet;
            SaveRoot.ThisStats.NumberToGet = SaveRoot.WhatNumber;
            ProcessTeams();
            HumanLists();
            SaveRoot.SameTable = true;
            ProcessTurnOrder();
            SaveRoot.PlayOrder!.WhoTurn = 1; // no longer 0 based.
            await NewTurnAsync();
            await MainSave.SaveSimpleSinglePlayerGameAsync(SaveRoot);
        }
        private void ProcessTurnOrder()
        {
            int x;
            int y;
            int currentteam;
            int currenttable;
            CurrentPlayer = SaveRoot!.PlayerList.GetSelf(); //i think
            currentteam = CurrentPlayer.Team;
            currenttable = CurrentPlayer.Table;
            int playernum;
            playernum = -1;
            for (x = 1; x <= 3; x++)
            {
                for (y = 1; y <= 4; y++)
                {
                    playernum += 1;
                    CurrentPlayer = FindSpecificPlayer(currentteam, currenttable);
                    if (CurrentPlayer.PlayerNum != -1)
                        throw new BasicBlankException("This has a duplicate.  Player is " + CurrentPlayer.Id);
                    CurrentPlayer.PlayerNum = CurrentPlayer.Id; // playernum has to be 0 based as well
                    if (currentteam == 1)
                        currentteam = 2;
                    else
                        currentteam = 1;
                }
                currenttable += 1;
                if (currenttable > 3)
                    currenttable = 1;
            }
        }
        private void ProcessTeams()
        {
            bool acceptable;
            int thisteam;
            int thistable;
            bool rets;
            bool NeedGroup = false;
            do
            {
                acceptable = true;
                foreach (var singleInfo in SaveRoot!.PlayerList)
                {
                    if (singleInfo.Acceptable == false)
                    {
                        acceptable = false;
                        thisteam = _rs.GetRandomNumber(2);
                        if (singleInfo.PreviousMate > -1)
                        {
                            NeedGroup = true;
                            thistable = singleInfo.Table;
                            rets = AcceptPlacement(thistable, thisteam, false, singleInfo.PreviousMate);
                        }
                        else
                        {
                            NeedGroup = false;
                            thistable = _rs.GetRandomNumber(3);
                            rets = AcceptPlacement(thistable, thisteam, true);
                        }

                        if (rets == true)
                        {
                            if (NeedGroup == false)
                                singleInfo.Acceptable = true;
                            singleInfo.Team = thisteam;
                            singleInfo.Table = thistable;
                            if (singleInfo.PlayerCategory == EnumPlayerCategory.Self)
                            {
                                SaveRoot.ThisStats.YourTeam = singleInfo.Team;
                                SaveRoot.ThisStats.YourTable = singleInfo.Table;
                            }
                        }
                    }
                }

                if (NeedGroup == true)
                    acceptable = GroupAcceptable();
                if (acceptable == true)
                    break;
            }
            while (true);
        }

        private bool GroupAcceptable()
        {
            int x;
            int howmany;
            int y;
            for (x = 1; x <= 3; x++)
            {
                for (y = 1; y <= 2; y++)
                {
                    howmany = SaveRoot!.PlayerList.Count(Items => Items.Table == x && Items.Team == y);

                    if (howmany != 2)
                        return false;
                }
            }
            int mate;
            foreach (var tempPlayer in SaveRoot!.PlayerList)
            {
                mate = FindTeamPlayer(tempPlayer.Id, true); // i think because 0 based
                if (mate == tempPlayer.PreviousMate)
                    return false;
            }
            return true;
        }
        private bool AcceptPlacement(int table, int team, bool firstMove, int oldMate = -1)
        {
            int howmany;
            if (firstMove == false)
            {
                if (oldMate == -1)
                    throw new Exception("Need to know the old team mate since this is not the first move");
                return true;
            };
            howmany = SaveRoot!.PlayerList.Count(Items => Items.Table == table);
            if (howmany == 4)
                return false;
            howmany = SaveRoot.PlayerList.Count(Items => Items.Table == table && Items.Team == team);
            if (howmany == 2)
                return false;
            return true;
        }
        private void HumanLists()
        {
            int self;
            self = FindTeamPlayer(SaveRoot!.PlayerList.GetSelf().Id, true);
            CurrentPlayer = SaveRoot.PlayerList[self]; //i think
            SaveRoot.ThisStats.TeamMate = CurrentPlayer.NickName;
            SaveRoot.ThisStats.YourTable = CurrentPlayer.Table; // i think
            int whattable;
            int whatteam;
            whattable = CurrentPlayer.Table;
            whatteam = CurrentPlayer.Team;
            int y = 0;
            foreach (var thisplayer in SaveRoot.PlayerList)
            {
                if (thisplayer.Table == whattable && thisplayer.Team != whatteam)
                {
                    y++;
                    CurrentPlayer = thisplayer;
                    if (y == 1)
                        SaveRoot.ThisStats.Opponent1 = CurrentPlayer.NickName;
                    else if (y == 2)
                        SaveRoot.ThisStats.Opponent2 = CurrentPlayer.NickName;
                    else
                        throw new Exception("There should be only 2 opponents because 2 to a team");
                }
            }
            if (y < 2)
                throw new Exception("There should be only 2 opponents not less because there are 2 to a team");
        }
    }
}