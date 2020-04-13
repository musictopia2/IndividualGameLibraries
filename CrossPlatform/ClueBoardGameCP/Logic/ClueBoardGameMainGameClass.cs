using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.TestUtilities;
using ClueBoardGameCP.Cards;
using ClueBoardGameCP.Data;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using ps = BasicGameFrameworkLibrary.BasicDrawables.MiscClasses;

namespace ClueBoardGameCP.Logic
{
    [SingletonGame]
    public class ClueBoardGameMainGameClass
        : BoardDiceGameClass<ClueBoardGamePlayerItem, ClueBoardGameSaveInfo, EnumColorChoice, int>, IMiscDataNM
    {
        public ClueBoardGameMainGameClass(IGamePackageResolver resolver,
            IEventAggregator aggregator,
            BasicData basic,
            TestOptions test,
            ClueBoardGameVMData model,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            CommandContainer command,
            ClueBoardGameGameContainer container,
            GameBoardProcesses gameBoard,
            StandardRollProcesses<SimpleDice, ClueBoardGamePlayerItem> roller
            ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, roller)
        {
            _model = model;
            _command = command;
            _gameBoard = gameBoard;
            CanPrepTurnOnSaved = false;
            _gameContainer = container;
        }
        public int MyID => PlayerList!.GetSelf().Id; //i think.
        public int OtherTurn
        {
            get
            {
                return SaveRoot!.PlayOrder.OtherTurn;
            }
            set
            {
                SaveRoot!.PlayOrder.OtherTurn = value;
            }
        }
        private readonly ClueBoardGameGameContainer _gameContainer;
        private readonly ClueBoardGameVMData _model;
        private readonly CommandContainer _command;
        private readonly GameBoardProcesses _gameBoard;

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            SaveRoot.LoadMod(_model); //we usually need this.
            if (PlayerList.DidChooseColors())
            {
                _gameBoard.LoadSpacesInRoom();
                _model.Pile.Visible = true;
                _model.Pile.ClearCards();
                SingleInfo = PlayerList.GetSelf();
                _model.HandList.HandList = SingleInfo!.MainHandList;
                SetCurrentPlayer(); //because no autoresume.
                _gameBoard.LoadSavedGame();
                _model.Cup!.CanShowDice = SaveRoot.MovesLeft > 0;
                SaveRoot.Instructions = "None";
            }
            SingleInfo = PlayerList.GetWhoPlayer();
            //anything else needed is here.
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            _gameBoard.LoadBoard();
            IsLoaded = true; //i think needs to be here.
        }
        private void LoadWeapons()
        {
            //only the host cna load weapons now.
            SaveRoot.WeaponList.Clear(); //just in case.
            6.Times(x =>
            {
                var thisWeapon = new WeaponInfo();
                thisWeapon.Weapon = (EnumWeaponList)x;
                switch (thisWeapon.Weapon)
                {
                    case EnumWeaponList.Candlestick:
                        {
                            thisWeapon.Name = "Candlestick";
                            break;
                        }

                    case EnumWeaponList.Knife:
                        {
                            thisWeapon.Name = "Knife";
                            break;
                        }

                    case EnumWeaponList.LeadPipe:
                        {
                            thisWeapon.Name = "Lead Pipe";
                            break;
                        }

                    case EnumWeaponList.Revolver:
                        {
                            thisWeapon.Name = "Revolver";
                            break;
                        }

                    case EnumWeaponList.Rope:
                        {
                            thisWeapon.Name = "Rope";
                            break;
                        }

                    case EnumWeaponList.Wrench:
                        {
                            thisWeapon.Name = "Wrench";
                            break;
                        }

                    default:
                        {
                            throw new BasicBlankException("Nothing found");
                        }
                }
                SaveRoot!.WeaponList.Add(thisWeapon);
            });
        }
        public override async Task SetUpGameAsync(bool isBeginning)
        {
            LoadControls();
            LoadWeapons(); //hopefully no problem to already do
            if (FinishUpAsync == null)
            {
                throw new BasicBlankException("The loader never set the finish up code.  Rethink");
            }
            SetUpDice();
            SaveRoot.LoadMod(_model); //we usually need this.
            SaveRoot!.ImmediatelyStartTurn = true; //most of the time, needs to immediately start turn.  if i am wrong, rethink.
            _model.HandList!.ClearHand(); //i think.
            _model.CurrentCharacterName = "";
            _model.CurrentRoomName = "";
            _model.CurrentWeaponName = "";
            SaveRoot.CurrentPrediction = new PredictionInfo(); //i think.
            await FinishUpAsync(isBeginning);
        }
        public override async Task AfterChoosingColorsAsync()
        {
            //anything else that is needed after they finished choosing colors.

            await Aggregator.SendLoadAsync(); //most of the time, you need to send an load message so the gameboard can be loaded with proper data.
            SaveRoot!.GameStatus = EnumClueStatusList.LoadGame;
            await SetUpFirstAsync();
        }

        private async Task SetUpFirstAsync()
        {
            bool rets;
            bool alsoLoad;
            if (BasicData!.MultiPlayer == false)
            {
                rets = true;
                alsoLoad = true;
            }
            else if (BasicData.Client == false)
            {
                rets = true;
                alsoLoad = true;
            }
            else
            {
                rets = false;
                alsoLoad = false;
            }
            _gameBoard.RepaintBoard(); //try here.
            await Delay!.DelaySeconds(.2);
            _gameBoard.LoadSpacesInRoom();
            _command.ManuelFinish = true; //maybe it had to be here
            if (rets == false)
            {
                Check!.IsEnabled = true;
                return; //wait for the host to send restore state once its all ready.
            }
            _gameBoard.ClearGame(alsoLoad); //try this instead.  i think only host can now.  hopefully that works.
            if (MiscDelegates.FillRestColors == null)
            {
                throw new BasicBlankException("Nobody is handling filling the rest of the colors.  Rethink");
            }
            MiscDelegates.FillRestColors.Invoke();
            //ThisMod.ColorChooser!.FillInRestOfColors(); //maybe this is it.
            //hopefully no need to fillinrestofcolors (?)

            _gameBoard.LoadColorsForCharacters();
            _gameBoard.ChooseScene();
            if (Test!.DoubleCheck)
            {
                WhoTurn = MyID;
                SetCurrentPlayer();
                _gameContainer!.CurrentCharacter!.Space = 78;
            }
            if (_gameContainer!.WeaponList.Values.Any(items => items.Room == 0))
                throw new BasicBlankException("Failed to populate weapons");
            await ShufflePassCardsAsync();
        }
        private async Task ShufflePassCardsAsync()
        {
            DeckRegularDict<CardInfo> thisList = new DeckRegularDict<CardInfo>();
            21.Times(x =>
            {
                var thisCard = _gameContainer!.ClueInfo(x);
                if (_gameBoard.CardPartOfSolution(thisCard) == false)
                    thisList.Add(thisCard);
            });
            if (thisList.Count != 18)
                throw new BasicBlankException("There must be 18 cards");
            if (PlayerList.Count() != 6)
                throw new BasicBlankException("There must be 6 players total");
            thisList.ShuffleList();
            DeckRegularDict<CardInfo> output = new DeckRegularDict<CardInfo>();
            ps.CardProcedures.PassOutCards(PlayerList!, thisList, 3, 0, false, ref output);
            SingleInfo = PlayerList!.GetSelf();
            if (SingleInfo.MainHandList.Count != 3)
                throw new BasicBlankException("Failed to pass out cards to self");
            _model.HandList.HandList.ReplaceRange(SingleInfo.MainHandList);
            _model.Pile.ClearCards();
            _model.HandList.Visible = true;
            _model.Pile.Visible = true;
            WhoTurn = WhoStarts; //i think this was missing.
            if (BasicData!.MultiPlayer)
            {
                SaveRoot!.ImmediatelyStartTurn = true;
                await Network!.SendRestoreGameAsync(SaveRoot);
            }
            await StartNewTurnAsync();
        }

        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "space":
                    _gameBoard.MoveToSpace(int.Parse(content));
                    await ContinueMoveAsync();
                    return;
                case "room":
                    _gameBoard.MoveToRoom(int.Parse(content));
                    SaveRoot!.MovesLeft = 0;
                    _model.CurrentRoomName = _gameContainer!.RoomList[int.Parse(content)].Name; //i think.
                    SaveRoot.GameStatus = EnumClueStatusList.MakePrediction;
                    Check!.IsEnabled = true; //i think this simple.
                    return;
                case "prediction":

                case "accusation":
                    SaveRoot!.CurrentPrediction = await js.DeserializeObjectAsync<PredictionInfo>(content);
                    _model!.CurrentCharacterName = SaveRoot.CurrentPrediction.CharacterName;
                    _model.CurrentRoomName = SaveRoot.CurrentPrediction.RoomName;
                    _model.CurrentWeaponName = SaveRoot.CurrentPrediction.WeaponName;
                    if (status == "prediction")
                        await MakePredictionAsync();
                    else
                        await MakeAccusationAsync();
                    return;
                case "cluegiven":
                    var thisCard = _gameContainer!.ClueInfo(int.Parse(content));
                    _model!.Pile!.AddCard(thisCard);
                    SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
                    await ShowHumanCanPlayAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                PrepStartTurn(); //if you did not choose colors, no need to prepstart because something else will do it.
                //code to run but only if you actually chose color.
                _gameBoard.NewTurn();
                SaveRoot!.Instructions = "None";
                SaveRoot.AccusationMade = false;
                SaveRoot.CurrentPrediction = new PredictionInfo();
                OtherTurn = 0;
                _model.CurrentCharacterName = "";
                _model.CurrentRoomName = "";
                _model.CurrentWeaponName = "";
                if (WhoTurn == 0)
                    throw new BasicBlankException("WhoTurn cannot be 0");
                SetCurrentPlayer();
                SaveRoot.GameStatus = EnumClueStatusList.StartTurn;
                SingleInfo = PlayerList!.GetWhoPlayer();
                await EndStepAsync();
                return;
            }


            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override Task ContinueTurnAsync()
        {
            if (PlayerList.DidChooseColors())
            {
                //can do extra things upon continue turn.  many board games require other things.
                return EndStepAsync(); //decided to use this instead.
            }
            return base.ContinueTurnAsync();
        }
        public override async Task MakeMoveAsync(int space)
        {
            //well see what we need for the move.
            await Task.CompletedTask;
        }
        public override async Task EndTurnAsync()
        {
            _command.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();


            //if anything else is needed, do here.
            if (PlayerList.DidChooseColors())
            {
                _model.Pile.ClearCards();
                //can do extra things upon ending turn.  many board games require other things. only do if the player actually chose colors.
                SaveRoot!.PreviousMoves = new Dictionary<int, MoveInfo>(); //i think.
            }
            await StartNewTurnAsync();
        }
        private void SetCurrentPlayer()
        {
            _gameContainer.CurrentCharacter = _gameContainer.CharacterList.Values.Single(items => items.Player == WhoTurn);
        }
        private void SetOtherPlayer()
        {
            if (OtherTurn == 0)
                throw new BasicBlankException("Cannot use the setotherplayer function when the otherturn is set to 0");
            _gameContainer.CurrentCharacter = _gameContainer.CharacterList.Values.Single(items => items.Player == OtherTurn);
        }

        private async Task EndStepAsync()
        {
            Aggregator.RepaintBoard();
            if (SaveRoot!.GameStatus == EnumClueStatusList.EndTurn)
            {
                OtherTurn = 0;
                SetCurrentPlayer();
            }
            if (OtherTurn == 0 && WhoTurn == MyID)
            {
                await ShowHumanCanPlayAsync();
                return;
            }
            if (OtherTurn > 0 && OtherTurn == MyID)
            {
                await ShowHumanCanPlayAsync();
                return;
            }
            if (BasicData!.MultiPlayer == false)
            {
                await ComputerRegularTurnAsync();
                return;
            }
            if (BasicData.MultiPlayer == true && OtherTurn == 0)
            {
                Check!.IsEnabled = true;
                return;
            }
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList!.GetOtherPlayer(); //i think.
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    if (BasicData.MultiPlayer == true && BasicData.Client)
                    {
                        Check!.IsEnabled = true;
                        return;
                    }
                    await ComputerRegularTurnAsync();
                    return;
                }
                if (BasicData.MultiPlayer)
                {
                    Check!.IsEnabled = true;
                    return;
                }
            }
        }
        protected override async Task ComputerTurnAsync()
        {
            if (PlayerList.DidChooseColors() == true)
                throw new BasicBlankException("Should not goto computer turn");
            if (SingleInfo!.InGame == false)
                throw new BasicBlankException("Not even in game");
            await base.ComputerTurnAsync();
        }
        private string CardToGive()
        {
            CustomBasicList<GivenInfo> thisList = new CustomBasicList<GivenInfo>();
            foreach (var thisGiven in _gameContainer!.CurrentCharacter!.ComputerData.CluesGiven)
            {
                if (thisGiven.Player == WhoTurn)
                {
                    if (thisGiven.Clue == SaveRoot!.CurrentPrediction!.CharacterName || thisGiven.Clue == SaveRoot.CurrentPrediction.RoomName || thisGiven.Clue == SaveRoot.CurrentPrediction.WeaponName)
                        thisList.Add(thisGiven);
                }
            }
            if (thisList.Count > 0)
                return thisList.First().Clue;
            ClueBoardGamePlayerItem tempPlayer = PlayerList!.GetOtherPlayer();
            tempPlayer.MainHandList.ForEach(thisCard =>
            {
                if (thisCard.Name == SaveRoot!.CurrentPrediction!.CharacterName || thisCard.Name == SaveRoot.CurrentPrediction.RoomName || thisCard.Name == SaveRoot.CurrentPrediction.WeaponName)
                {
                    var newGiven = new GivenInfo();
                    newGiven.Player = WhoTurn;
                    newGiven.Clue = thisCard.Name;
                    thisList.Add(newGiven);
                }
            });
            if (thisList.Count == 0)
                throw new BasicBlankException("There was no card to give even though the CardGive function ran");
            var finCard = thisList.GetRandomItem();
            _gameContainer.CurrentCharacter.ComputerData.CluesGiven.Add(finCard);
            return finCard.Clue;
        }
        private async Task ComputerRegularTurnAsync()
        {
            if (OtherTurn > 0 && BasicData!.MultiPlayer && BasicData.Client)
                return;
            if (SaveRoot!.GameStatus == EnumClueStatusList.StartTurn)
            {
                await EndTurnAsync();
                return; //the computer has to skip their turns because it was really hosed.
            }
            if (SaveRoot.GameStatus == EnumClueStatusList.FindClues)
            {
                SetOtherPlayer();
                if (_gameContainer!.CurrentCharacter is null)
                    throw new BasicBlankException("There is no current character before deciding on a card to give");
                string thisInfo = CardToGive();
                ClueBoardGamePlayerItem newPlayer = PlayerList!.GetOtherPlayer();
                CardInfo thisCard = newPlayer.MainHandList.Single(items => items.Name == thisInfo);
                SingleInfo = PlayerList.GetWhoPlayer();
                if (newPlayer.CanSendMessage(BasicData!) && WhoTurn != MyID)
                    await Network!.SendToParticularPlayerAsync("cluegiven", thisCard.Deck, SingleInfo.NickName);
                SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
                if (WhoTurn == MyID)
                {
                    _model.Pile.AddCard(thisCard);
                }
                await EndStepAsync();
                return;
            }
            throw new BasicBlankException("The computer should have skipped their turns since their moving was really hosed");
        }
        public override async Task AfterRollingAsync()
        {
            SaveRoot!.GameStatus = EnumClueStatusList.DiceRolled;
            _gameBoard.ResetMoves();
            if (Test!.DoubleCheck == false)
            {
                SaveRoot.DiceNumber = _model.Cup!.ValueOfOnlyDice;
                SaveRoot.MovesLeft = SaveRoot.DiceNumber;
            }
            else
            {
                SaveRoot.DiceNumber = 3; //could need this or something else (?)
                SaveRoot.MovesLeft = 3;
            }
            SaveRoot.GameStatus = EnumClueStatusList.MoveSpaces;
            await EndStepAsync();
        }
        private int ManuallyGetKey(RoomInfo thisRoom)
        {
            int x = 0;
            foreach (var tempRoom in _gameContainer!.RoomList.Values)
            {
                x++;
                if (tempRoom.Name == thisRoom.Name)
                    return x;
            }
            throw new BasicBlankException("Room Not Found");
        }
        private void PlaceObjectsOnBoard()
        {
            var thisRoom = _gameContainer!.GetRoom(SaveRoot!.CurrentPrediction!.RoomName);
            var thisWeapon = _gameContainer.GetWeapon(SaveRoot.CurrentPrediction.WeaponName);
            var thisCharacter = _gameContainer.GetCharacter(SaveRoot.CurrentPrediction.CharacterName);
            thisCharacter.CurrentRoom = ManuallyGetKey(thisRoom);
            thisCharacter.Space = 0;
            thisWeapon.Room = thisCharacter.CurrentRoom;
            Aggregator.RepaintBoard();
        }
        public void PopulateDetectiveNoteBook()
        {
            if (_gameContainer!.RoomList.Count != 9)
                throw new BasicBlankException("Needs 9 rooms");
            DetectiveInfo thisD;
            foreach (var thisRoom in _gameContainer.RoomList.Values)
            {
                thisD = new DetectiveInfo();
                thisD.Category = EnumCardType.IsRoom;
                thisD.IsChecked = false;
                thisD.Name = thisRoom.Name;
                _gameContainer.DetectiveList.Add(thisD);
            }
            CustomBasicList<string> originalList = new CustomBasicList<string> { "Mr. Green", "Colonel Mustard", "Mrs. Peacock", "Professor Plum",
                                                     "Miss Scarlet", "Mrs. White"};
            originalList.ForEach(thisItem =>
            {
                thisD = new DetectiveInfo();
                thisD.Category = EnumCardType.IsCharacter;
                thisD.IsChecked = false;
                thisD.Name = thisItem;
                _gameContainer.DetectiveList.Add(thisD);
            });
            if (_gameContainer.WeaponList.Count != 6)
                throw new BasicBlankException("Need 6 weapons when populating detective notebook");
            foreach (var thisWeapon in _gameContainer.WeaponList.Values)
            {
                thisD = new DetectiveInfo();
                thisD.Category = EnumCardType.IsWeapon;
                thisD.IsChecked = false;
                thisD.Name = thisWeapon.Name;
                _gameContainer.DetectiveList.Add(thisD);
            }
        }
        public async Task MakeAccusationAsync()
        {
            _command.ManuelFinish = true;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(BasicData!))
            {
                await Network!.SendAllAsync("accusation", SaveRoot!.CurrentPrediction!);
            }
            PlaceObjectsOnBoard();
            if (SaveRoot!.CurrentPrediction!.CharacterName == SaveRoot.Solution.CharacterName &&
                SaveRoot.CurrentPrediction.RoomName == SaveRoot.Solution.RoomName &&
                SaveRoot.CurrentPrediction.WeaponName == SaveRoot.Solution.WeaponName)
            {
                await UIPlatform.ShowMessageAsync($"The accusation was correct.  {SaveRoot.CurrentPrediction.CharacterName} did it in the {SaveRoot.CurrentPrediction.RoomName} with the {SaveRoot.CurrentPrediction.WeaponName}");
                await ShowWinAsync();
                return;
            }
            if (WhoTurn == MyID)
                await UIPlatform.ShowMessageAsync($"Sorry, the accusation was not correct {SingleInfo.NickName}.  You are out of the game but need to still be there in order to prove other predictions wrong.");
            SingleInfo.InGame = false;
            if (PlayerList.Count(items => items.InGame == true) <= 1)
            {
                await UIPlatform.ShowMessageAsync($"Sorry, nobody got the solution correct.  Therefore, nobody won.  The solution was {Constants.vbCrLf} {SaveRoot.CurrentPrediction.CharacterName} did it in the {SaveRoot.CurrentPrediction.RoomName} with the {SaveRoot.CurrentPrediction.WeaponName}");
                await ShowTieAsync();
                return;
            }
            SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
            await EndStepAsync();
        }
        public async Task ContinueMoveAsync()
        {
            if (_gameBoard.HasValidMoves() == false)
            {
                SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
                await EndStepAsync();
                return;
            }
            SaveRoot!.MovesLeft--;
            if (SaveRoot.MovesLeft == 0)
                SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
            await EndStepAsync();
        }
        private bool CanGiveCard()
        {
            if (SaveRoot!.CurrentPrediction!.CharacterName == "" || SaveRoot.CurrentPrediction.RoomName == "" || SaveRoot.CurrentPrediction.WeaponName == "")
                throw new BasicBlankException("Cannot use the cangivecard function because the prediction is not filled out completed");
            var tempPlayer = PlayerList!.GetOtherPlayer();
            if (tempPlayer.MainHandList.Any(items => items.Name == SaveRoot.CurrentPrediction.CharacterName))
                return true;
            if (tempPlayer.MainHandList.Any(items => items.Name == SaveRoot.CurrentPrediction.RoomName))
                return true;
            if (tempPlayer.MainHandList.Any(items => items.Name == SaveRoot.CurrentPrediction.WeaponName))
                return true;
            return false;
        }
        public async Task MakePredictionAsync()
        {
            _command.ManuelFinish = true;
            OtherTurn = 0;
            var thisRoom = _gameContainer!.RoomList[_gameContainer.CurrentCharacter!.CurrentRoom];
            _gameContainer.CurrentCharacter.PreviousRoom = _gameContainer.CurrentCharacter.CurrentRoom;
            _model.CurrentRoomName = thisRoom.Name;
            SaveRoot!.CurrentPrediction!.RoomName = thisRoom.Name;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self && Test!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SingleInfo.CanSendMessage(BasicData!))
                await Network!.SendAllAsync("prediction", SaveRoot.CurrentPrediction);
            PlaceObjectsOnBoard();
            int x = 0;
            do
            {
                x++;
                OtherTurn = await PlayerList.CalculateOtherTurnAsync();
                if (x > 10)
                    throw new BasicBlankException("Too Much");
                if (OtherTurn == 0)
                    break;
                SetOtherPlayer();
                if (CanGiveCard())
                    break;
            } while (true);
            if (OtherTurn == 0)
            {
                SetCurrentPlayer();
                SingleInfo = PlayerList.GetWhoPlayer();
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
                    ComputerNoCluesFound();
                SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
                await EndStepAsync();
                return;
            }
            SaveRoot.GameStatus = EnumClueStatusList.FindClues;
            await EndStepAsync();
        }
        private void ComputerNoCluesFound()
        {
            ReceivedInfo thisRe;
            if (SingleInfo!.MainHandList.Any(items => items.Name == SaveRoot!.CurrentPrediction!.RoomName) == false)
            {
                foreach (var thisRoom in _gameContainer!.RoomList.Values)
                {
                    if (thisRoom.Name != SaveRoot!.CurrentPrediction!.RoomName)
                    {
                        thisRe = new ReceivedInfo();
                        thisRe.Name = thisRoom.Name;
                        _gameContainer.CurrentCharacter!.ComputerData.CluesReceived.Add(thisRe);
                    }
                }
            }
            foreach (var thisCharacter in _gameContainer!.CharacterList.Values)
            {
                if (thisCharacter.Name != SaveRoot!.CurrentPrediction!.CharacterName)
                {
                    thisRe = new ReceivedInfo();
                    thisRe.Name = thisCharacter.Name;
                    _gameContainer.CurrentCharacter!.ComputerData.CluesReceived.Add(thisRe);
                }
            }
            foreach (var thisWeapon in _gameContainer.WeaponList.Values)
            {
                if (thisWeapon.Name != SaveRoot!.CurrentPrediction!.WeaponName)
                {
                    thisRe = new ReceivedInfo();
                    thisRe.Name = thisWeapon.Name;
                    _gameContainer.CurrentCharacter!.ComputerData.CluesReceived.Add(thisRe);
                }
            }
        }
        public async Task ComputerFoundClueAsync(string whatClue)
        {
            ReceivedInfo thisRe = new ReceivedInfo();
            thisRe.Name = whatClue;
            SetCurrentPlayer();
            if (whatClue == _gameContainer!.CurrentCharacter!.ComputerData.Weapon)
                _gameContainer.CurrentCharacter.ComputerData.Weapon = "";
            else if (whatClue == _gameContainer.CurrentCharacter.ComputerData.Character)
                _gameContainer.CurrentCharacter.ComputerData.Character = "";
            else if (whatClue == _gameContainer.CurrentCharacter.ComputerData.RoomHeaded)
                _gameContainer.CurrentCharacter.ComputerData.RoomHeaded = "";
            _gameContainer.CurrentCharacter.ComputerData.CluesReceived.Add(thisRe);
            await EndStepAsync();
        }
    }
}