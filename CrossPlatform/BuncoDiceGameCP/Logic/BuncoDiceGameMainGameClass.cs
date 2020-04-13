using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BuncoDiceGameCP.Data;
using BuncoDiceGameCP.EventModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BuncoDiceGameCP.Logic
{
    [SingletonGame]
    public class BuncoDiceGameMainGameClass : IAggregatorContainer
    {
        private readonly ISaveSinglePlayerClass _thisState;
        private readonly IGamePackageResolver _container;
        internal BuncoDiceGameSaveInfo _saveRoot;
        private readonly RandomGenerator _rs;
        private readonly CommandContainer _command;
        private readonly GlobalClass _global;
        private bool _mVarHadBunco;
        private DiceCup<SimpleDice>? _cup;

        internal PlayerItem? CurrentPlayer { get; set; }

        public BuncoDiceGameMainGameClass(ISaveSinglePlayerClass thisState,
            IEventAggregator aggregator,
            IGamePackageResolver container,
            RandomGenerator rs,
            CommandContainer command,
            GlobalClass global
            )
        {
            _thisState = thisState;
            Aggregator = aggregator;
            _container = container;
            _rs = rs;
            _command = command;
            _global = global;
            _saveRoot = container.ReplaceObject<BuncoDiceGameSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        }

        public IEventAggregator Aggregator { get; }
        public int FindTeamPlayer(int whatPlayer, bool showError)
        {
            PlayerItem currentPlayer;
            currentPlayer = _saveRoot!.PlayerList[whatPlayer];
            int yourteam;
            int yourtable;
            yourteam = currentPlayer.Team;
            yourtable = currentPlayer.Table;
            foreach (var newPlayer in _saveRoot.PlayerList)
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
                _saveRoot!.ThisStats.YourPoints = newScore;
            newNum = FindTeamPlayer(_saveRoot!.PlayOrder!.WhoTurn, true);
            newPlayer = _saveRoot.PlayerList[newNum];
            newPlayer.Points = newScore;
        }
        private bool HasThree()
        {
            return _saveRoot!.DiceList.DistinctCount(Items => Items.Value) == 1; //needs to be 1 distinct.
        }
        private bool HasBunco()
        {
            if (HasThree() == false)
                return false;
            int tempValue = _saveRoot!.DiceList.First().Value;
            return tempValue == _saveRoot.WhatNumber;
        }
        private int HowMany()
        {
            return _saveRoot!.DiceList.Count(Items => Items.Value == _saveRoot.WhatNumber);
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
            if (_cup == null)
            {
                throw new BasicBlankException("Cup cannot be null.  This means was never loaded.  Rethink");
            }
            var thisList = _cup.RollDice();
            await _cup.ShowRollingAsync(thisList, _saveRoot!.SameTable);
        }

        public async Task ProcessNewRoundAsync()
        {
            _global.IsActive = true;
            _command.ManuelFinish = true;
            int teammate;
            _saveRoot!.ThisStats.YourPoints = 0;
            _saveRoot.ThisStats.OpponentScore = 0;
            _saveRoot.PlayerList.ForEach(player =>
            {
                player.Points = 0;
                player.WinDetermined = false;
                player.PlayerNum = -1;
                player.Acceptable = false;
                teammate = FindTeamPlayer(player.Id, true); //no -1 anymore.
                player.PreviousMate = teammate;
                if (player.WonPrevious == true && player.Table > 1)
                    player.TempTable = player.Table - 1;
                else if (player.WonPrevious == false && player.Table < 3)
                    player.TempTable = player.Table + 1;
                else
                    player.TempTable = player.Table;
            });
            _saveRoot.PlayerList.ForEach(player =>
            {
                player.Table = player.TempTable;
                player.TempTable = 0;
            });
            await StartRoundAsync();
        }
        public async Task EndTurnAsync()
        {
            _command.ManuelFinish = true; //now has to manually be done.
            //view model is responsible for filling out canendturn and alreadyreceivedbunco upon endturnasync.
            //_thisMod.CanEndTurn = false;
            //_thisMod.AlreadyReceivedBunco = false;
            _saveRoot!.PlayOrder!.WhoTurn = await _saveRoot.PlayerList.CalculateWhoTurnAsync();
            await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
            await NewTurnAsync(); //i think
        }
        private async Task ReloadGameAsync(Func<BuncoDiceGameSaveInfo, bool, DiceCup<SimpleDice>> loadCup)
        {
            _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<BuncoDiceGameSaveInfo>();
            _container.ReplaceObject(_saveRoot); //this is now the new object.
            _saveRoot.PlayerList.MainContainer = _container; //has to redo that part.
            _saveRoot.PlayerList.AutoSaved(_saveRoot!.PlayOrder!); //i think this is needed too.
            _cup = loadCup.Invoke(_saveRoot, true);
            await Aggregator.SendLoadAsync(); //still needed.
            await NewTurnAsync();
        }
        public async Task StartGameAsync(Func<BuncoDiceGameSaveInfo, bool, DiceCup<SimpleDice>> loadCup)
        {
            if (await _thisState.CanOpenSavedSinglePlayerGameAsync() == true)
            {
                await ReloadGameAsync(loadCup);
                return;
            }
            _saveRoot = _container.Resolve<BuncoDiceGameSaveInfo>();
            _saveRoot.PlayOrder = (PlayOrderClass)_container.Resolve<IPlayOrder>(); //has to be here so they both use the same object.
            _saveRoot.PlayerList = new PlayerCollection<PlayerItem>();
            _saveRoot.PlayerList.LoadPlayers(1, 11);
            _saveRoot.PlayerList.FinishLoading(); //i think we will go ahead and shuffle the players.
            _saveRoot.PlayerList.ForEach(singleInfo =>
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
            _saveRoot.WhatSet = 1;
            _saveRoot.WhatNumber = 1;
            _saveRoot.ThisStats.Status = "Game In Progress";
            _cup = loadCup.Invoke(_saveRoot, false);
            await Aggregator.SendLoadAsync();
            await StartRoundAsync();
        }

        private PlayerItem FindSpecificPlayer(int whatteam, int whattable)
        {
            return _saveRoot!.PlayerList.Where(items => items.Team == whatteam &&
            items.Table == whattable && items.PlayerNum == -1).First();
        }

        private async Task NewTurnAsync()
        {
            CurrentPlayer = _saveRoot!.PlayerList.GetWhoPlayer();
            _saveRoot.ThisStats.Turn = CurrentPlayer.NickName;
            if (CurrentPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                _saveRoot.SameTable = true;
                _saveRoot.HadBunco = false;
                _saveRoot.HasRolled = false;
                _command.ManuelFinish = false;
                _command.IsExecuting = false;
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
            humanPlayer = _saveRoot!.PlayerList.GetSelf();
            mytable = humanPlayer.Table;
            int teamplayer;
            if (mytable == thistable)
                _saveRoot.SameTable = true;
            else
                _saveRoot.SameTable = false;
            int humanopponent;
            do
            {
                if (CurrentPlayer.Points >= 21 && thistable == 1)
                {
                    endRound = true;
                    break;
                }
                await RollDiceAsync();
                if (_saveRoot.SameTable == true)
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
                teamplayer = FindTeamPlayer(_saveRoot!.PlayOrder!.WhoTurn, true);
                if (teamplayer == humanPlayer.Id) //i think
                    _saveRoot.ThisStats.YourPoints = CurrentPlayer.Points;
                humanopponent = CalculateOpponentScore(humanPlayer.Id); //i think this simple (?)
                _saveRoot.ThisStats.OpponentScore = humanopponent;
                await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot); //i think its this simple.
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
            tempPlayer1 = _saveRoot!.PlayerList[whatPlayer];
            int yourtable;
            int yourteam;
            yourtable = tempPlayer1.Table;
            yourteam = tempPlayer1.Team;
            foreach (var tempPlayer in _saveRoot.PlayerList)
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
            tempPlayer = _saveRoot!.PlayerList[whatPlayer];
            yourscore = tempPlayer.Points;
            opponentscore = CalculateOpponentScore(whatPlayer);
            if (yourscore > opponentscore)
                return true;
            if (yourscore < opponentscore)
                return false;
            int teammate;
            teammate = FindTeamPlayer(whatPlayer, true);
            tempPlayer = _saveRoot.PlayerList[teammate];
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
            tempPlayer = _saveRoot.PlayerList[ask1];
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
            tempPlayer1 = _saveRoot!.PlayerList[whatPlayer];
            int whattable;
            int whatteam;
            whattable = tempPlayer1.Table;
            whatteam = tempPlayer1.Team;
            foreach (var tempPlayer in _saveRoot.PlayerList)
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
            _saveRoot!.ThisStats.Turn = "None";
            _saveRoot.PlayerList.ForEach(x =>
            {
                if (HasWon(x.Id) == true)
                {
                    x.WonPrevious = true;
                    x.Wins++;
                }
                else
                {
                    x.WonPrevious = false;
                    x.Losses++;
                }
                if (x.PlayerCategory == EnumPlayerCategory.Self)
                {
                    _saveRoot.ThisStats.Wins = x.Wins;
                    _saveRoot.ThisStats.Losses = x.Losses;
                }
            });
            if (_saveRoot.WhatNumber < 6)
            {
                _saveRoot.WhatNumber += 1;
                //_thisMod.NewGameVisible = true;
                await ShowRoundAsync();

                _command.IsExecuting = false;
            }
            else
            {
                _saveRoot.WhatSet += 1;
                if (_saveRoot.WhatSet < 5)
                {
                    _saveRoot.WhatNumber = 1;
                    await ShowRoundAsync(); //iffy.
                    //_thisMod.NewGameVisible = true;
                    //_thisMod.CanEndTurn = true;
                    _command.IsExecuting = false;
                }
                else
                {
                    _saveRoot.ThisStats.Status = "Game is over";
                    await _thisState.DeleteSinglePlayerGameAsync();
                    await EndGameAsync();
                }
            }
        }

        private async Task EndGameAsync()
        {
            _global.IsActive = false;
            await Aggregator.PublishAsync(new EndGameEventModel());
        }

        internal async Task PossibleNewGameAsync()
        {
            await this.SendGameOverAsync();
        }

        private async Task ShowRoundAsync()
        {
            _global.IsActive = false;
            await Aggregator.PublishAsync(new EventModels.NewRoundEventModel()); //try this way.  because the other is intended for multiplayer games anyways.  hopefully this works.
        }

        private async Task StartRoundAsync()
        {
            _saveRoot!.ThisStats.Set = _saveRoot.WhatSet;
            _saveRoot.ThisStats.NumberToGet = _saveRoot.WhatNumber;
            ProcessTeams();
            HumanLists();
            _saveRoot.SameTable = true;
            ProcessTurnOrder();
            _saveRoot.PlayOrder!.WhoTurn = 1; // no longer 0 based.
            await NewTurnAsync();
            await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
        }
        private void ProcessTurnOrder()
        {
            int x;
            int y;
            int currentteam;
            int currenttable;
            CurrentPlayer = _saveRoot!.PlayerList.GetSelf(); //i think
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
                foreach (var singleInfo in _saveRoot!.PlayerList)
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
                                _saveRoot.ThisStats.YourTeam = singleInfo.Team;
                                _saveRoot.ThisStats.YourTable = singleInfo.Table;
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
                    howmany = _saveRoot!.PlayerList.Count(Items => Items.Table == x && Items.Team == y);

                    if (howmany != 2)
                        return false;
                }
            }
            int mate;
            foreach (var tempPlayer in _saveRoot!.PlayerList)
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
            howmany = _saveRoot!.PlayerList.Count(Items => Items.Table == table);
            if (howmany == 4)
                return false;
            howmany = _saveRoot.PlayerList.Count(Items => Items.Table == table && Items.Team == team);
            if (howmany == 2)
                return false;
            return true;
        }
        private void HumanLists()
        {
            int self;
            self = FindTeamPlayer(_saveRoot!.PlayerList.GetSelf().Id, true);
            CurrentPlayer = _saveRoot.PlayerList[self]; //i think
            _saveRoot.ThisStats.TeamMate = CurrentPlayer.NickName;
            _saveRoot.ThisStats.YourTable = CurrentPlayer.Table; // i think
            int whattable;
            int whatteam;
            whattable = CurrentPlayer.Table;
            whatteam = CurrentPlayer.Team;
            int y = 0;
            foreach (var thisplayer in _saveRoot.PlayerList)
            {
                if (thisplayer.Table == whattable && thisplayer.Team != whatteam)
                {
                    y++;
                    CurrentPlayer = thisplayer;
                    if (y == 1)
                        _saveRoot.ThisStats.Opponent1 = CurrentPlayer.NickName;
                    else if (y == 2)
                        _saveRoot.ThisStats.Opponent2 = CurrentPlayer.NickName;
                    else
                        throw new Exception("There should be only 2 opponents because 2 to a team");
                }
            }
            if (y < 2)
                throw new Exception("There should be only 2 opponents not less because there are 2 to a team");
        }
    }
}