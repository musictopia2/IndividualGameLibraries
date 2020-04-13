using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MillebournesCP.Cards;
using MillebournesCP.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.

namespace MillebournesCP.Logic
{
    [SingletonGame]
    public class MillebournesMainGameClass : CardGameClass<MillebournesCardInformation, MillebournesPlayerItem, MillebournesSaveInfo>, IMiscDataNM, IStartNewGame
    {


        private readonly MillebournesVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly MillebournesGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly ComputerAI _ai;

        public MillebournesMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            MillebournesVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<MillebournesCardInformation> cardInfo,
            CommandContainer command,
            MillebournesGameContainer gameContainer,
            ComputerAI ai
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _ai = ai;
        }
        public int FindDeck
        {
            get
            {
                int thisDeck = _model!.Pile1!.CardSelected();
                if (thisDeck > 0)
                    return thisDeck;
                return _model.PlayerHand1!.ObjectSelected();
            }
        }
        private int TeamWon
        {
            get
            {
                TeamCP thisTeam = _gameContainer.TeamList.Where(items => items.TotalScore >= 5000).OrderByDescending(items => items.TotalScore).FirstOrDefault();
                if (thisTeam == null)
                    return 0; //0 means you can't end game.
                return thisTeam.TeamNumber;
            }
        }
        public async Task GameOverAsync(int teamWon)
        {
            if (PlayerList.Count() <= 3)
            {
                SingleInfo = PlayerList.Single(items => items.Team == teamWon);
                await ShowWinAsync();
                return;
            }
            StrCat cats = new StrCat();
            PlayerList!.ForConditionalItems(thisplayer => thisplayer.Team == teamWon, thisPlayer =>
            {
                cats.AddToString(thisPlayer.NickName, ",");
            });
            await ShowWinAsync(cats.GetInfo());
        }

        public override Task FinishGetSavedAsync()
        {
            LoadTeams(); //i think
            _howMany = 0;
            _gameContainer.TeamList.ForEach(thisTeam =>
            {
                var thisTemp = SaveRoot!.TeamData.Single(items => items.Team == thisTeam.TeamNumber);
                thisTeam.LoadSavedGame(thisTemp.SavedData!);
            });
            if (SaveRoot!.LastThrowAway > 0)
            {
                var thisCard = new MillebournesCardInformation();
                thisCard.Populate(SaveRoot.LastThrowAway);
                _model!.Pile2!.AddCard(thisCard);
            }
            return base.FinishGetSavedAsync();
        }
        protected override async Task ComputerTurnAsync()
        {
            var thisMove = _ai.ComputerMove();
            if (thisMove.WillThrowAway == true && thisMove.Team > 0)
                throw new BasicBlankException("The computer cannot play a card on a pile plus throw away a card");
            if (thisMove.WillThrowAway == false && thisMove.Team == 0)
                throw new BasicBlankException("The computer must either throw away a card or play on a pile");
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            if (thisMove.WillThrowAway)
            {
                await ThrowawayCardAsync(thisMove.Deck);
                return;
            }
            if (BasicData!.MultiPlayer == true)
            {
                SendPlay thisSend = new SendPlay();
                thisSend.Deck = thisMove.Deck;
                thisSend.Pile = thisMove.WhichPile;
                thisSend.Team = thisMove.Team;
                await Network!.SendAllAsync("regularplay", thisSend);
            }
            await PlayAsync(thisMove.Deck, thisMove.WhichPile, thisMove.Team, false);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            _howMany = 0;
            SaveRoot!.DidClone100 = false;
            SaveRoot.ImmediatelyStartTurn = true;
            AssignTeams();
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.Miles = 0;
                thisPlayer.OtherPoints = 0;
            });
            if (_gameContainer.TeamList.Count > 0)
                _gameContainer.TeamList.ForEach(thisTeam => thisTeam.ClearPiles());
            if (isBeginning == false)
            {
                _model!.Deck1!.ClearCards();
                _model.Pile1!.ClearCards();
                _model.Pile2!.ClearCards();
            }
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            if (_gameContainer.TeamList.Count == 0)
                LoadTeams(); //try this way.
            else
            {
                _gameContainer.TeamList.ForEach(thisTeam => thisTeam.ClearPiles());
            }
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }

        public override Task PopulateSaveRootAsync()
        {
            SaveRoot!.TeamData = new CustomBasicList<TempData>();
            _gameContainer.TeamList.ForEach(thisTeam =>
            {
                TempData thisTemp = new TempData();
                thisTemp.SavedData = thisTeam.SavedData();
                thisTemp.Team = thisTeam.TeamNumber;
                SaveRoot.TeamData.Add(thisTemp);
            });
            if (_model!.Pile2!.PileEmpty())
                SaveRoot.LastThrowAway = 0;
            else
                SaveRoot.LastThrowAway = _model.Pile2!.GetCardInfo().Deck;
            return base.PopulateSaveRootAsync();
        }
        private int HowManyTeams
        {
            get
            {
                if (PlayerList.Count() <= 3)
                    return PlayerList.Count();
                if (PlayerList.Count() == 4)
                    return 2;
                return 3;
            }
        }
        private void DoNextTeam(ref int whichOne, int manys)
        {
            whichOne++;
            if (whichOne > manys)
                whichOne = 1;
        }
        private void AssignTeams()
        {
            if (PlayerList.All(items => items.Team > 0))
                return;
            MillebournesPlayerItem thisPlayer;
            if (PlayerList.Count() <= 3)
            {
                for (int x = 1; x <= PlayerList.Count(); x++)
                {
                    thisPlayer = PlayerList![x];
                    thisPlayer.Team = x;
                }
                return;
            }
            thisPlayer = PlayerList![1];
            thisPlayer.Team = 1;
            thisPlayer = PlayerList[2];
            thisPlayer.Team = 2;
            thisPlayer = PlayerList[3];
            if (PlayerList.Count() == 4)
                thisPlayer.Team = 1;
            else
                thisPlayer.Team = 3;
            thisPlayer = PlayerList[4];
            if (PlayerList.Count() == 4)
                thisPlayer.Team = 2;
            else
                thisPlayer.Team = 1;
            if (PlayerList.Count() > 4)
            {
                thisPlayer = PlayerList[5];
                thisPlayer.Team = 2;
                thisPlayer = PlayerList[6];
                thisPlayer.Team = 3;
            }
        }
        private void LoadTeams()
        {
            _gameContainer.TeamList.Clear(); //i think often times, we have to clear the list.
            _gameContainer.TeamList = new CustomBasicList<TeamCP>();
            int howMany = HowManyTeams;
            SingleInfo = PlayerList!.GetSelf();
            TeamCP thisTeam = new TeamCP(SingleInfo.Team, _gameContainer, _model);
            thisTeam.IsSelf = true;
            _gameContainer.TeamList.Add(thisTeam);
            int nexts = SingleInfo.Team;
            for (int x = 1; x <= howMany - 1; x++)
            {
                DoNextTeam(ref nexts, howMany);
                thisTeam = new TeamCP(nexts, _gameContainer, _model);
                thisTeam.IsSelf = false;
                _gameContainer.TeamList.Add(thisTeam);
            }
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            SendPlay? thisSend = null;
            if (status == "nocoupe" || status == "hascoupe" || status == "regularplay")
                thisSend = await js.DeserializeObjectAsync<SendPlay>(content);
            switch (status)
            {
                case "timeup":
                    await EndCoupeAsync(int.Parse(content));
                    break;
                case "throw":
                    await ThrowawayCardAsync(int.Parse(content));
                    break;
                case "regularplay":
                    await PlayAsync(thisSend!.Deck, thisSend.Pile, thisSend.Team, false);
                    break;
                case "nocoupe":
                    TeamCP thisTeam = _gameContainer.FindTeam(thisSend!.Team);
                    thisTeam.IncreaseWrongs();
                    UpdateGrid(thisTeam.TeamNumber);
                    await EndCoupeAsync(thisSend.Player);
                    break;
                case "hascoupe":
                    _gameContainer.CurrentCoupe.Player = thisSend!.Player;
                    _gameContainer.CurrentCoupe.Card = thisSend.Deck;
                    await EndCoupeAsync(_gameContainer.CurrentCoupe.Player);
                    break;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            _gameContainer.CurrentCoupe.Player = 0;
            _gameContainer.CurrentCoupe.Card = 0;
            PlayerList!.ForEach(thisPlayer => thisPlayer.OtherTurn = false);
            await base.StartNewTurnAsync();
            SingleInfo = PlayerList.GetWhoPlayer();
            if (SingleInfo.Team == 0)
                throw new BasicBlankException($"The player's team cannot be 0.  The name of the player to go is {SingleInfo.NickName}.  Find out what happened");
            _gameContainer.CurrentCP = _gameContainer.FindTeam(SingleInfo.Team); //not sure if we need this upon autoresume (?)
            SaveRoot!.CurrentTeam = SingleInfo.Team;
            await DrawAsync(); //does automatically.
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

            //anything else is here.  varies by game.


            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }
        Task IStartNewGame.ResetAsync()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.TotalPoints = 0;
                thisPlayer.Number200s = 0; //just in case.
            });
            return Task.CompletedTask;
        }

        public override async Task EndRoundAsync()
        {
            _gameContainer.TeamList.ForEach(thisTeam =>
            {
                int miless = thisTeam.Miles;
                int totalOther = CalculateTotalOtherPoints(thisTeam);
                thisTeam.AddToTotalScore(miless);
                thisTeam.AddToTotalScore(totalOther);
            });
            PlayerList!.ForEach(thisPlayer =>
            {
                var thisTeam = _gameContainer.FindTeam(thisPlayer.Team);
                thisPlayer.TotalPoints = thisTeam.TotalScore;
                thisPlayer.OtherPoints = CalculateTotalOtherPoints(thisTeam);
            });
            int wins = TeamWon;
            if (wins > 0)
            {
                await GameOverAsync(wins);
                return;
            }
            //_wasOver = true;
            await this.RoundOverNextAsync();
        }
        public int CalculateTotalOtherPoints(TeamCP thisTeam)
        {
            int temps = thisTeam.CalculateOtherPoints;
            if (thisTeam.Miles == 1000)
            {
                if (HadShutOut(thisTeam.TeamNumber))
                    temps += 500;
                if (_model!.Deck1!.IsEndOfDeck() == true)
                    temps += 300;
            }
            return temps;
        }
        private bool HadShutOut(int whichTeam)
        {
            return _gameContainer.TeamList.Where(items => items.TeamNumber != whichTeam).All(items => items.Miles == 0);
        }
        private int _howMany;
        private bool CanEndRound
        {
            get
            {
                _howMany++;
                if (_howMany >= 10 && Test!.EndRoundEarly)
                {
                    return true;
                }
                if (_gameContainer.CurrentCP!.Miles == 1000)
                    return true;
                return PlayerNoCards;
            }
        }
        private bool PlayerNoCards
        {
            get
            {
                return PlayerList.All(items => items.MainHandList.Count == 0);
            }
        }
        public void AdjustHand(int deck)
        {
            if (SingleInfo!.MainHandList.ObjectExist(deck))
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(deck);
                if (_model!.Pile1!.PileEmpty() == false)
                {
                    var newCard = _model.Pile1.GetCardInfo();
                    SingleInfo.MainHandList.Add(newCard); //hopefully okay.
                }
            }
            if (_model!.Pile1!.PileEmpty() == false)
                _model.Pile1.RemoveFromPile();
        }
        public bool HasCoupe(out int newDeck)
        {
            newDeck = 0;
            if (_gameContainer.CurrentCP!.CurrentHazard == EnumHazardType.None && _gameContainer.CurrentCP.CurrentSpeed == false)
                throw new BasicBlankException("There was no new hazard");
            string searchFor;
            if (_gameContainer.CurrentCP.CurrentHazard == EnumHazardType.StopSign || _gameContainer.CurrentCP.CurrentSpeed == true)
                searchFor = "Right Of Way";
            else if (_gameContainer.CurrentCP.CurrentHazard == EnumHazardType.Accident)
                searchFor = "Driving Ace";
            else if (_gameContainer.CurrentCP.CurrentHazard == EnumHazardType.FlatTire)
                searchFor = "Puncture Proof";
            else if (_gameContainer.CurrentCP.CurrentHazard == EnumHazardType.OutOfGas)
                searchFor = "Extra Tank";
            else
                throw new BasicBlankException("Cannot find anything to search for based on the current hazard");
            var thisCard = SingleInfo!.MainHandList.FirstOrDefault(items => items.CardName == searchFor);
            if (thisCard != null)
                newDeck = thisCard.Deck;
            return newDeck > 0;
        }
        public async Task EndPartAsync(bool hadSafety)
        {
            if (hadSafety == false)
            {
                int x;
                var loopTo = HowManyTeams;
                for (x = 1; x <= loopTo; x++)
                {
                    _gameContainer.CurrentCP = _gameContainer.FindTeam(x);
                    _gameContainer.CurrentCP.EndTurn();
                }
                _gameContainer.CurrentCP = _gameContainer.FindTeam(SingleInfo!.Team);
            }
            if (SingleInfo!.MainHandList.Count == 0)
                SingleInfo.InGame = false;
            if (CanEndRound == true)
            {
                await EndRoundAsync();
                return;
            }
            if (hadSafety == false)
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync(true);
            await StartNewTurnAsync();
        }
        public void UpdateGrid(int whichTeam = 0)
        {
            if (whichTeam == 0)
                whichTeam = SaveRoot!.CurrentTeam;
            PlayerList!.ForConditionalItems(items => items.Team == whichTeam, thisPlayer =>
            {
                var tempControl = _gameContainer.FindTeam(thisPlayer.Team);
                thisPlayer.Miles = tempControl.Miles;
                thisPlayer.OtherPoints = tempControl.CalculateOtherPoints;
            });
        }
        public async Task ThrowawayCardAsync(int deck)
        {
            if (SingleInfo!.CanSendMessage(BasicData!))
                await Network!.SendAllAsync("throw", deck);
            AdjustHand(deck);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            await AnimatePlayAsync(thisCard);
            _model!.Pile2!.AddCard(thisCard);
            await EndPartAsync(false);
        }
        private bool _isCoupe;

        public async Task ProcessCoupeAsync(int deck, int player)
        {
            WhoTurn = player;
            SingleInfo = PlayerList!.GetWhoPlayer();
            _gameContainer.CurrentCP = _gameContainer.FindTeam(SingleInfo.Team);
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                if (SingleInfo.MainHandList.Count == 1)
                    await UIPlatform.ShowMessageAsync("You got a coup fourre.  However, you will not get another turn because you have no cards left");
                else
                    await UIPlatform.ShowMessageAsync("You got a coup fourre.  Please take another turn.");
            }
            else
            {
                if (SingleInfo.MainHandList.Count == 1)
                    await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} has got a coup fourre.  However, he will not get another turn turn because he has no more cards left");
                else
                    await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} got a coup fourre.  Therefore, he will get another turn.");
            }
            _isCoupe = true;
            await DrawAsync();
            await PlayAsync(deck, EnumPileType.Safety, SingleInfo.Team, true);
        }
        private int ComputerCoupe(int whichTeam, out int player)
        {
            int tempPlayer = 0;
            int decks = 0;
            PlayerList!.ForConditionalItems(items => items.PlayerCategory == EnumPlayerCategory.Computer && items.Team == whichTeam, thisPlayer =>
            {
                if (tempPlayer == 0)
                {
                    SingleInfo = thisPlayer;
                    bool rets = HasCoupe(out decks);
                    if (rets == true)
                    {
                        tempPlayer = thisPlayer.Id;
                    }

                }
            });
            SingleInfo = PlayerList.GetWhoPlayer(); //looks like computer coupe foure was bugged in the old version.
            player = tempPlayer;
            return decks;
        }
        protected override Task AfterDrawingAsync()
        {
            if (_isCoupe == true)
            {
                _isCoupe = false; //so when it goes again, can do like normal
                return Task.CompletedTask; // because more to be done.
            }
            return base.AfterDrawingAsync();
        }
        public async Task EndCoupeAsync(int player)
        {
            var tempPlayer = PlayerList![player];
            tempPlayer.OtherTurn = true;
            if (!PlayerList.Any(items => items.OtherTurn == false))
            {
                if (_gameContainer.CurrentCoupe.Player == 0)
                {
                    await EndPartAsync(false);
                    return;
                }
                await ProcessCoupeAsync(_gameContainer.CurrentCoupe.Card, _gameContainer.CurrentCoupe.Player);
                return;
            }
            if (BasicData!.MultiPlayer == false)
                throw new BasicBlankException("All should have processed.  Rethink");
            Check!.IsEnabled = true;
        }
        public async Task PlayAsync(int deck, EnumPileType whichPile, int whichTeam, bool isCoupe)
        {
            var thisTeam = _gameContainer.FindTeam(whichTeam);
            _gameContainer.CurrentCP = thisTeam;
            _command.ManuelFinish = true; //because somebody could have a coupe foure.
            thisTeam.CurrentCard = _gameContainer.DeckList!.GetSpecificItem(deck);
            AdjustHand(deck);
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                SortCards();
            await thisTeam.AnimateMoveAsync(whichPile, thisTeam.CurrentCard);
            thisTeam.AddCard(whichPile, out bool doDelay, isCoupe);
            UpdateGrid(whichTeam);
            if (doDelay == true)
            {
                var thisCoupe = ComputerCoupe(whichTeam, out int newPlayer);
                if (thisCoupe > 0)
                {
                    await ProcessCoupeAsync(thisCoupe, newPlayer);
                    return;
                }
                PlayerList!.ForConditionalItems(items => items.PlayerCategory == EnumPlayerCategory.Computer || items.Team != whichTeam, thisPlayer =>
                {
                    thisPlayer.OtherTurn = true;
                });
                MillebournesPlayerItem finPlayer1 = PlayerList.GetSelf();
                if (whichTeam == finPlayer1.Team)
                {

                    if (_gameContainer.LoadCoupeAsync == null)
                    {
                        throw new BasicBlankException("Nobody is loading the coupe screen.  Rethink");
                    }
                    await _gameContainer.LoadCoupeAsync.Invoke();
                    _model.Stops!.StartTimer();
                    await ShowHumanCanPlayAsync();
                    return;
                }
                if (PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.OtherHuman && items.Team == whichTeam))
                {
                    Check!.IsEnabled = true;
                    return;
                }
                await EndPartAsync(false);
                return;
            }
            if (whichPile == EnumPileType.Safety)
            {
                if (isCoupe == false)
                {
                    if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                    {
                        if (SingleInfo.MainHandList.Count == 0)
                            await UIPlatform.ShowMessageAsync("You got a safety.  However, you will not get another turn because you have no more cards left");
                        else
                            await UIPlatform.ShowMessageAsync("You got a safety.  Please take another turn.");
                    }
                    else
                    {
                        if (SingleInfo.MainHandList.Count == 0)
                            await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} has gotten a safety.  However, he will not get another turn because he does not have any more cards left");
                        else
                            await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} has gotten a safety.  Therefore, he will get another turn.");
                    }
                }
                if (SingleInfo.MainHandList.Count > 0)
                {
                    await EndPartAsync(true);
                    return;
                }
            }
            await EndPartAsync(false);
        }
    }
}
