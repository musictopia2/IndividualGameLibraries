using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.MultiplePilesViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMHelpers;
using System;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MillebournesCP
{
    public class TeamCP : ObservableObject
    {
        #region "Variables/Properties"
        private CustomBasicList<MillebournesCardInformation> _previouslyPlayed = new CustomBasicList<MillebournesCardInformation>(); //hopefully a basic customlist is okay for this situation.
        public BasicMultiplePilesCP<MillebournesCardInformation> CardPiles;
        public EnumHazardType CurrentHazard { get; private set; } //i think this could be the best way to handle this.
        public bool CurrentSpeed { get; private set; }
        public int Miles { get; private set; }
        public CustomBasicCollection<SafetyInfo> SafetyList = new CustomBasicCollection<SafetyInfo>();
        public bool IsSelf { get; set; }
        public MillebournesCardInformation? CurrentCard;
        public int Wrongs { get; private set; }
        public int HowMany200S { get; private set; }
        public int TeamNumber { get; private set; }
        public int TotalScore { get; private set; }
        public bool SafetyEnabled { get; private set; } //i think this will be linked up to visible (?)
        public ControlCommand SafetyCommand { get; set; }
        private readonly int _selfPlayer;
        private readonly MillebournesViewModel _thisMod;
        private readonly MillebournesMainGameClass _mainGame;
        private readonly EventAggregator _thisE;
        private string _Text = "";
        public string Text
        {
            get { return _Text; }
            set
            {
                if (SetProperty(ref _Text, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        #endregion
        #region "Init/Clear Processes"
        public void ClearPiles()
        {
            _previouslyPlayed.Clear();
            CardPiles.ClearBoard();
            SafetyList.Clear();
            CurrentCard = null;
            HowMany200S = 0;
            Miles = 0;
            Wrongs = 0;
            CurrentHazard = EnumHazardType.None;
            CurrentSpeed = false;
            Update200Data();
        }
        private void Init()
        {
            Text = $"Team {TeamNumber} Piles";
            int x = 0;
            CardPiles.PileList!.ForEach(thisPile =>
            {
                x++;
                if (x == 1)
                    thisPile.Text = "Mileage Pile";
                else if (x == 2)
                    thisPile.Text = "Speed Pile";
                else if (x == 3)
                    thisPile.Text = "Hazard Pile";
                else
                    throw new BasicBlankException("There should be only 3 piles. Find out what happened");
                thisPile.IsEnabled = false; //true has to be proven.
            });
        }
        public TeamCP(int team, MillebournesViewModel thisMod)
        {
            CardPiles = new BasicMultiplePilesCP<MillebournesCardInformation>(thisMod);
            CardPiles.Rows = 1;
            CardPiles.Columns = 3;
            CardPiles.HasFrame = true;
            CardPiles.Style = BasicMultiplePilesCP<MillebournesCardInformation>.EnumStyleList.HasList;
            CardPiles.HasText = true;
            CardPiles.LoadBoard();
            TeamNumber = team; //i think has to be here.
            SafetyCommand = new ControlCommand(CardPiles, async items =>
            {
                await thisMod.ProcessTeamClick(EnumPileType.Safety, TeamNumber);
            }, thisMod, thisMod.CommandContainer!);
            CardPiles.ExtraCommand = SafetyCommand; //so notifications work.
            _thisMod = thisMod;
            CardPiles.PileClickedAsync += CardPiles_PileClickedAsync;
            _mainGame = thisMod.MainContainer!.Resolve<MillebournesMainGameClass>();
            _thisE = thisMod.MainContainer.Resolve<EventAggregator>();
            Init();
            MillebournesPlayerItem thisPlayer = _mainGame.PlayerList!.GetSelf();
            _selfPlayer = thisPlayer.Id;
        }
        private async Task CardPiles_PileClickedAsync(int index, BasicPileInfo<MillebournesCardInformation> thisPile)
        {
            if (index == 0)
                await _thisMod.ProcessTeamClick(EnumPileType.Miles, TeamNumber);
            else if (index == 1)
                await _thisMod.ProcessTeamClick(EnumPileType.Speed, TeamNumber);
            else if (index == 2)
                await _thisMod.ProcessTeamClick(EnumPileType.Hazard, TeamNumber);
            else
                throw new BasicBlankException("Only 3 piles supported.  Find out what happened");
        }
        #endregion
        #region "Enable Processes"
        public void EnableChange()
        {
            if (CardPiles.IsEnabled == false)
            {
                SafetyEnabled = false;
                OnPropertyChanged(nameof(SafetyEnabled)); //try this
                return;
            }
            if (_mainGame.ThisMod!.CoupeVisible == true)
            {
                SafetyEnabled = false;
                OnPropertyChanged(nameof(SafetyEnabled)); //try this
                CardPiles.PileList!.ForEach(thisPile1 => thisPile1.IsEnabled = false);
                return;
            }
            SafetyEnabled = IsSelf;
            OnPropertyChanged(nameof(SafetyEnabled)); //try this
            BasicPileInfo<MillebournesCardInformation> thisPile;
            bool hasSpeedLimit = HasSpeedLimit;
            EnumHazardType hazard = WhichHazard;
            if (IsSelf == false)
            {
                thisPile = CardPiles.PileList![0]; //i think directly 0 based is fine now.
                thisPile.IsEnabled = false; //has to be false.
                thisPile = CardPiles.PileList[1];
                thisPile.IsEnabled = !hasSpeedLimit;
                thisPile = CardPiles.PileList[2];
                thisPile.IsEnabled = hazard == EnumHazardType.None;
                return;
            }
            thisPile = CardPiles.PileList![0];
            thisPile.IsEnabled = hazard == EnumHazardType.None;
            thisPile = CardPiles.PileList[2];
            thisPile.IsEnabled = hazard != EnumHazardType.None;
            thisPile = CardPiles.PileList[1];
            thisPile.IsEnabled = hasSpeedLimit;
        }
        #endregion
        #region "Can Play Processes"
        public bool CanGiveSpeedLimit(out string message)
        {
            message = ""; //default to ""
            if (_selfPlayer == _mainGame.WhoTurn && _mainGame.SaveRoot!.CurrentTeam == TeamNumber)
                throw new BasicBlankException("Cannot give yourself a speed limit. Therefore, should have can CanEndSpeedLimit instead");
            if (CurrentCard!.CardType != EnumCardCategories.Speed)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "This is not a speed limit";
                return false;
            }
            if (_mainGame.SaveRoot!.CurrentTeam == TeamNumber)
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    throw new BasicBlankException("Should have not considered a hazard because its from the same team.");
                return false;
            }
            bool hasSpeedLimit = HasSpeedLimit;
            if (hasSpeedLimit == true)
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    throw new BasicBlankException("The player already has a speed limit.  Should have disabled the control");
                return false;
            }
            if (SafetyHas("Right Of Way"))
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    message = "Sorry, cannot give him a speed limit because he already has the right of way";
                return false;
            }
            return true;
        }
        public bool CanEndSpeedLimit(out string message)
        {
            message = ""; //default to ""
            if (_selfPlayer == _mainGame.WhoTurn && _mainGame.SaveRoot!.CurrentTeam != TeamNumber)
                throw new BasicBlankException("Cannot end the speed limit for someone else.  Should have ran the CanGiveSpeedLimit instead");
            if (CurrentCard!.CardType != EnumCardCategories.EndLimit)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "This is not an end of limit";
                return false;
            }
            bool hasSpeedLimit = HasSpeedLimit;
            if (hasSpeedLimit == false)
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    throw new BasicBlankException("The player did not have a speed limit.  Therefore, it should have disabled the control");
                return false;
            }
            return true;
        }
        public bool CanGiveHazard(out string message)
        {
            message = ""; //default to ""
            if (_selfPlayer == _mainGame.WhoTurn && _mainGame.SaveRoot!.CurrentTeam == TeamNumber)
                throw new BasicBlankException("Cannot give yourself a hazard");
            if (CurrentCard!.CardType != EnumCardCategories.Hazard)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "This is not a hazard card";
                return false;
            }
            var whichHazard = WhichHazard;
            if (whichHazard != EnumHazardType.None)
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    throw new BasicBlankException("Cannot give a hazard because he already has one.  Should have disabled the control");
                return false;
            }
            if (_mainGame.SaveRoot!.CurrentTeam == TeamNumber)
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    throw new BasicBlankException("Should have not considered a hazard because its from the same team.");
                return false;
            }
            if (SafetyList.Count == 0)
                return true;
            if (SafetyHas("Right Of Way") && CurrentCard.CardName == "Stop")
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    message = "Sorry, cannot give him a stop sign because he has the right of way";
                return false;
            }
            if (SafetyHas("Driving Ace") && CurrentCard.CardName == "Accident")
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    message = "Sorry, cannot give him an accident because he has the driving ace";
                return false;
            }
            if (SafetyHas("Puncture Proof") && CurrentCard.CardName == "Flat Tire")
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    message = "Sorry, cannot give him a flat tire because he has puncture proof tires";
                return false;
            }
            if (SafetyHas("Extra Tank") && CurrentCard.CardName == "Out Of Gas")
            {
                if (_selfPlayer == _mainGame.WhoTurn)
                    message = "Sorry, cannot give him an out of gas because he has the extra tank";
                return false;
            }
            return true;
        }
        public bool CanFixHazard(out string message)
        {
            message = ""; //default to ""
            if (CurrentCard!.CardType != EnumCardCategories.Remedy)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "Sorry, this is not a remedy";
                return false;
            }
            if (_mainGame.SaveRoot!.CurrentTeam != TeamNumber)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    throw new BasicBlankException("Cannot give a remedy to someone else.  Should have ran the CanGiveHazard instead");
                return false;
            }
            var whichHazard = WhichHazard;
            if (whichHazard == EnumHazardType.None)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    throw new BasicBlankException("Sorry, you have no hazards.  Therefore, it should have disabled the control");
                return false;
            }
            if (whichHazard == EnumHazardType.Accident && CurrentCard.CardName != "Repairs")
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = $"{CurrentCard.CardName} is the wrong card to fix an accident";
                return false;
            }
            if (whichHazard == EnumHazardType.FlatTire && CurrentCard.CardName != "Spare Tire")
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = $"{CurrentCard.CardName} is the wrong card to fix a flat tire";
                return false;
            }
            if (whichHazard == EnumHazardType.OutOfGas && CurrentCard.CardName != "Gasoline")
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = $"{CurrentCard.CardName} is the wrong card to fix an out of gas";
                return false;
            }
            if (whichHazard == EnumHazardType.StopSign && CurrentCard.CardName != "Roll")
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = $"{CurrentCard.CardName} is the wrong card to fix a stop";
                return false;
            }
            return true;
        }
        public bool CanPlaceMiles(out string message)
        {
            EnumHazardType whichHazard = WhichHazard;
            message = "";
            if (_mainGame.WhoTurn == _selfPlayer && _mainGame.SaveRoot!.CurrentTeam != TeamNumber)
                throw new BasicBlankException("Cannot place miles on the other players.  Should have disabled the control");
            if (_mainGame.WhoTurn == _selfPlayer && whichHazard != EnumHazardType.None)
                throw new BasicBlankException("Cannot place miles because you have a hazard.  Should have disabled the control");
            if (_mainGame.SaveRoot!.CurrentTeam != TeamNumber)
                return false;
            if (CurrentCard!.CardType != EnumCardCategories.Miles)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "This is not a miles card";
                return false;
            }
            if (whichHazard != EnumHazardType.None)
                return false;
            if (CurrentCard.Mileage + Miles > 1000)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = $"Cannot place {CurrentCard.CardName} because that will give you over 1000 miles.  Cannot have over 1000 miles";
                return false;
            }
            if (CurrentCard.Mileage <= 50)
                return true;
            if (HasSpeedLimit == true)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = $"Cannot place {CurrentCard.CardName} because you have a speed limit.  The most miles you can place is 50";
                return false;
            }
            if (CurrentCard.Mileage < 200)
                return true;
            if (HowMany200S >= 2)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "Cannot place 200 miles because you already have 2 200 miles placed";
                return false;
            }
            return true;
        }
        public bool CanPlaceSafety(out string message)
        {
            message = "";
            if (_mainGame.WhoTurn == _selfPlayer && _mainGame.SaveRoot!.CurrentTeam != TeamNumber)
                throw new BasicBlankException("Cannot place a safety for another player.  Should have disabled the control");
            if (CurrentCard!.CardType != EnumCardCategories.Safety)
            {
                if (_mainGame.WhoTurn == _selfPlayer)
                    message = "This is not a safety card";
                return false;
            }
            return true;
        }
        #endregion
        #region "Misc Reaonly Properties"
        public int Coups => SafetyList.Count(items => items.WasCoupe);
        public int NumberOfSafeties => SafetyList.Count;
        public CustomBasicList<MillebournesCardInformation> CardsPlayed
        {
            get
            {
                if (_mainGame.SingleInfo!.PlayerCategory != BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Computer)
                    throw new BasicBlankException("Only a computer player can find out the cards played");
                return _previouslyPlayed;
            }
        }
        public bool HasSpeedLimit
        {
            get
            {
                if (CardPiles.HasCard(1) == false)
                    return false;
                var thisCard = CardPiles.GetLastCard(1);
                if (thisCard.CardType == EnumCardCategories.EndLimit)
                    return false;
                if (thisCard.CardType != EnumCardCategories.Speed)
                    throw new BasicBlankException("Only a speed limit or the end of speed limit can be placed on the speed pile");
                return true;
            }
        }
        public EnumHazardType WhichHazard
        {
            get
            {
                if (CardPiles.HasCard(2) == false)
                    return EnumHazardType.StopSign; // you have to start with a roll.
                var thisCard = CardPiles.GetLastCard(2);
                return thisCard.CardName switch
                {
                    "Stop" => EnumHazardType.StopSign,
                    "Flat Tire" => EnumHazardType.FlatTire,
                    "Out Of Gas" => EnumHazardType.OutOfGas,
                    "Accident" => EnumHazardType.Accident,
                    _ => EnumHazardType.None,
                };
            }
        }
        public int CalculateOtherPoints
        {
            get
            {
                int extras = 0;
                var coups = Coups;
                var minuss = Wrongs * 100;
                minuss *= -1;
                extras += minuss;
                var coupPoints = coups * 300;
                extras += coupPoints;
                extras += (SafetyList.Count * 100);
                if (SafetyList.Count == 4)
                    extras += 400;
                if (Miles == 1000)
                {
                    extras += 400;
                    if (HowMany200S == 0)
                        extras += 300;
                }
                return extras;
            }
        }
        #endregion
        #region "Misc Processes"
        public void AddToTotalScore(int whatScore)
        {
            TotalScore += whatScore;
        }
        public bool SafetyHas(string whichOne)
        {
            return SafetyList.Any(items => items.SafetyName == whichOne);
        }
        public void IncreaseWrongs()
        {
            Wrongs++;
        }
        private void Update200Data()
        {
            _mainGame.PlayerList!.ForConditionalItems(items => items.Team == TeamNumber, items => items.Number200s = HowMany200S);
        }
        #endregion
        #region "Play Processes"
        public void NewTurn()
        {
            CurrentSpeed = false;
            CurrentHazard = EnumHazardType.None;
            CurrentCard = null;
        }
        public void EndTurn()
        {
            CurrentHazard = EnumHazardType.None;
            CurrentSpeed = false;
        }
        public void AddCard(EnumPileType whichOne, out bool doDelay, bool isCoupe = false)
        {
            CurrentCard!.IsSelected = false;
            if (_previouslyPlayed.Any(items => items.Deck == CurrentCard.Deck))
                throw new BasicBlankException("Cannot add the card to the list of previous cards because the card was already added.  Find out what happened");
            _previouslyPlayed.Add(CurrentCard);
            doDelay = CurrentCard.CardType == EnumCardCategories.Speed || CurrentCard.CardType == EnumCardCategories.Hazard;
            if (isCoupe == true && whichOne != EnumPileType.Safety)
                throw new BasicBlankException("Cannot be a coup fourre because its not going to the saftey pile");
            if (_mainGame.ThisTest!.DoubleCheck == false) //for now, double check will add it to force more adding cards.
            {
                if (whichOne == EnumPileType.Hazard && CurrentCard.CardType != EnumCardCategories.Hazard && CurrentCard.CardType != EnumCardCategories.Remedy)
                    throw new BasicBlankException("Sorry, the card type must be a hazard or remedy only for the hazard pile");
                if (whichOne == EnumPileType.Safety && CurrentCard.CardType != EnumCardCategories.Safety)
                    throw new BasicBlankException("Sorry, the card type must be a safety only for the safeties pile");
                if (whichOne == EnumPileType.Miles && CurrentCard.CardType != EnumCardCategories.Miles)
                    throw new BasicBlankException("Sorry, the card type must be miles only for the miles pile");
                if (whichOne == EnumPileType.Speed && CurrentCard.CardType != EnumCardCategories.Speed && CurrentCard.CardType != EnumCardCategories.EndLimit)
                    throw new BasicBlankException("Sorry, the card type must be speed limit or end of limit only for the speed pile");
            }
            if (whichOne == EnumPileType.Miles)
            {
                if (CurrentCard.Mileage == 200)
                {
                    HowMany200S++;
                    Update200Data();
                }
                Miles += CurrentCard.Mileage;
                CardPiles.AddCardToPile(0, CurrentCard);
                return;
            }
            if (whichOne == EnumPileType.Hazard)
            {
                if (CurrentCard.CardType == EnumCardCategories.Hazard)
                {
                    if (CurrentCard.CardName == "Stop")
                        CurrentHazard = EnumHazardType.StopSign;
                    else if (CurrentCard.CardName == "Out Of Gas")
                        CurrentHazard = EnumHazardType.OutOfGas;
                    else if (CurrentCard.CardName == "Accident")
                        CurrentHazard = EnumHazardType.Accident;
                    else if (CurrentCard.CardName == "Flat Tire")
                        CurrentHazard = EnumHazardType.FlatTire;
                    else
                        throw new BasicBlankException($"Cannot find a hazard for {CurrentCard.CardName}");
                }
                else
                    CurrentHazard = EnumHazardType.None;
                CardPiles.AddCardToPile(2, CurrentCard);
                return;
            }
            if (whichOne == EnumPileType.Speed)
            {
                if (CurrentCard.CardType == EnumCardCategories.Speed)
                    CurrentSpeed = true;
                else if (CurrentCard.CardType == EnumCardCategories.EndLimit)
                    CurrentSpeed = false;
                else
                    throw new BasicBlankException($"Cannot find a speed for {CurrentCard.CardName}");
                CardPiles.AddCardToPile(1, CurrentCard);
                return;
            }
            if (whichOne == EnumPileType.Safety)
            {
                SafetyInfo thisSafety = new SafetyInfo();
                thisSafety.SafetyName = CurrentCard.CardName;
                thisSafety.WasCoupe = isCoupe;
                TakeOutHazardSafety(CurrentCard.CardName);
                SafetyList.Add(thisSafety);
                return;
            }
            throw new BasicBlankException("Could not find the whichone variable");
        }
        public async Task AnimateMoveAsync(EnumPileType whichPile, MillebournesCardInformation thisCard)
        {
            if (whichPile == EnumPileType.Safety)
                whichPile = EnumPileType.Hazard; //i think
            var tempPile = CardPiles.PileList![(int)whichPile - 1];
            String tag = $"team{TeamNumber}";
            await _thisE.AnimateCardAsync(thisCard, EnumAnimcationDirection.StartDownToCard, tag, tempPile);
        }
        private void TakeOutHazardSafety(string whatSafety)
        {
            EnumHazardType whichHazard = WhichHazard;
            bool hasSpeedLimit = HasSpeedLimit;
            if (whichHazard == EnumHazardType.None && hasSpeedLimit == false)
                return; //because nothing to take out
            if (whichHazard == EnumHazardType.None && hasSpeedLimit == true && whatSafety != "Right Of Way")
                return;
            if (hasSpeedLimit == true && whatSafety == "Right Of Way")
            {
                CardPiles.RemoveCardFromPile(1);
            }
            if (whichHazard == EnumHazardType.None)
                return;
            if (whichHazard == EnumHazardType.Accident && whatSafety != "Driving Ace")
                return;
            if (whichHazard == EnumHazardType.FlatTire && whatSafety != "Puncture Proof")
                return;
            if (whichHazard == EnumHazardType.OutOfGas && whatSafety != "Extra Tank")
                return;
            if (whichHazard == EnumHazardType.StopSign && whatSafety != "Right Of Way")
                return;
            if (CardPiles.HasCard(2) == false)
            {
                var thisCard = new MillebournesCardInformation();
                thisCard.Populate(100); //this is a way to clone.
                thisCard.CompleteCategory = EnumCompleteCategories.Roll;
                thisCard.IsUnknown = false;
                CardPiles.AddCardToPile(2, thisCard);
                _mainGame.SaveRoot!.DidClone100 = true;
                return;
            }
            CardPiles.RemoveCardFromPile(2); //i think.
        }
        #endregion
        #region "autoresume processes"
        public void LoadSavedGame(SavedTeam thisSave)
        {
            _previouslyPlayed = new CustomBasicList<MillebournesCardInformation>();
            CardPiles.PileList!.ReplaceRange(thisSave.SavedPiles);
            SafetyList = new CustomBasicCollection<SafetyInfo>(thisSave.SafetyList);
            Wrongs = thisSave.Wrongs;
            HowMany200S = thisSave.Number200s;
            CurrentHazard = thisSave.CurrentHazard;
            CurrentSpeed = thisSave.CurrentSpeed;
            Miles = thisSave.Miles;
            TotalScore = thisSave.TotalScore;
            CurrentCard = thisSave.CurrentCard;
            Update200Data();
        }
        public SavedTeam SavedData()
        {
            SavedTeam thisSave = new SavedTeam();
            thisSave.CurrentCard = CurrentCard;
            thisSave.CurrentHazard = CurrentHazard;
            thisSave.CurrentSpeed = CurrentSpeed;
            thisSave.Miles = Miles;
            thisSave.Number200s = HowMany200S;
            thisSave.SavedPiles = CardPiles.PileList.ToCustomBasicList();
            thisSave.PreviousList = _previouslyPlayed.ToCustomBasicList();
            thisSave.SafetyList = SafetyList.ToCustomBasicList();
            thisSave.TotalScore = TotalScore;
            thisSave.Wrongs = Wrongs;
            return thisSave;
        }
        #endregion
    }
}