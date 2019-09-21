using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.DIContainers;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.MultiplayerClasses.BasicGameClasses;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.MultiplayerClasses.Extensions;
using BasicGameFramework.MultiplayerClasses.InterfaceMessages;
using BasicGameFramework.NetworkingClasses.Extensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using ps = BasicGameFramework.BasicDrawables.MiscClasses;
namespace ClueBoardGameCP
{
    [SingletonGame]
    public class ClueBoardGameMainGameClass : BoardDiceGameClass<EnumColorChoice, PawnPiecesCP<EnumColorChoice>,
        ClueBoardGamePlayerItem, ClueBoardGameSaveInfo, int>, IMiscDataNM
    {
        public GlobalClass? ThisGlobal;
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
        public int MyID => PlayerList!.GetSelf().Id; //i think.
        public ClueBoardGameMainGameClass(IGamePackageResolver container) : base(container) { }

        internal ClueBoardGameViewModel? ThisMod;
        public override void Init() //decided to have all the code under init to prevent overflow issues.
        {
            base.Init();
            ThisMod = MainContainer.Resolve<ClueBoardGameViewModel>();
            ThisGlobal = MainContainer.Resolve<GlobalClass>();
            CanPrepTurnOnSaved = false; //that could cause issues as well.
        }
        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            AfterRestoreDice();
            BoardGameSaved(); //i think.
            SaveRoot!.LoadMod(ThisMod!);
            if (DidChooseColors)
            {
                ThisMod!.GameBoard1!.LoadSpacesInRoom();
                ThisMod.ThisPile!.Visible = true;
                ThisMod.ThisPile.ClearCards(); //i think.
                SingleInfo = PlayerList!.GetSelf();
                if (SingleInfo.MainHandList.Count != 3)
                    throw new BasicBlankException("Failed to pass out cards to self");
                ThisMod.HandList!.HandList = SingleInfo.MainHandList;
                ThisMod.HandList.Visible = true;
                SetCurrentPlayer(); //because no autoresume.
                ThisMod.GameBoard1.LoadSavedGame();
                ThisMod.ThisCup!.CanShowDice = SaveRoot.MovesLeft > 0;
                SaveRoot.Instructions = "None";
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            
            return Task.CompletedTask;
        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;
            LoadUpDice();
            ThisMod!.GameBoard1!.LoadBoard();
            IsLoaded = true; //i think needs to be here.
        }
        private void LoadWeapons()
        {
            //only the host cna load weapons now.
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
            if (IsLoaded == false)
            {
                LoadControls(); //i think okay since something else is responsible for hte extra players.
                LoadWeapons();
            }
            SaveRoot!.LoadMod(ThisMod!);
            EraseColors();
            SetUpDice(); //you would not have set up colors at this point.
            ThisMod!.HandList!.ClearHand(); //i think.
            ThisMod.CurrentCharacterName = "";
            ThisMod.CurrentRoomName = "";
            ThisMod.CurrentWeaponName = "";
            SaveRoot.CurrentPrediction = new PredictionInfo(); //i think.
            SaveRoot.ImmediatelyStartTurn = true;
            await ThisLoader!.FinishUpAsync(isBeginning);
        }
        protected override async Task AfterChoosingColorsAsync()
        {
            SaveRoot!.GameStatus = EnumClueStatusList.LoadGame;
            await SetUpFirstAsync();
        }
        private async Task SetUpFirstAsync()
        {
            bool rets;
            bool alsoLoad;
            if (ThisData!.MultiPlayer == false)
            {
                rets = true;
                alsoLoad = true;
            }
            else if (ThisData.Client == false)
            {
                rets = true;
                alsoLoad = true;
            }
            else
            {
                rets = false;
                alsoLoad = false;
            }
            ThisMod!.GameBoard1!.RepaintBoard(); //try here.
            await Delay!.DelaySeconds(.2);
            ThisMod.GameBoard1.LoadSpacesInRoom();
            ThisMod.CommandContainer!.ManuelFinish = true; //maybe it had to be here
            if (rets == false)
            {
                ThisCheck!.IsEnabled = true;
                return; //wait for the host to send restore state once its all ready.
            }
            ThisMod.GameBoard1.ClearGame(alsoLoad); //try this instead.  i think only host can now.  hopefully that works.
            ThisMod.ColorChooser!.FillInRestOfColors(); //maybe this is it.
            ThisMod.GameBoard1.LoadColorsForCharacters();
            ThisMod.GameBoard1.ChooseScene();
            if (ThisTest!.DoubleCheck)
            {
                WhoTurn = MyID;
                SetCurrentPlayer();
                ThisGlobal!.CurrentCharacter!.Space = 78;
            }
            if (ThisGlobal!.WeaponList.Values.Any(items => items.Room == 0))
                throw new BasicBlankException("Failed to populate weapons");
            await ShufflePassCardsAsync();
        }
        private async Task ShufflePassCardsAsync()
        {
            DeckRegularDict<CardInfo> thisList = new DeckRegularDict<CardInfo>();
            21.Times(x =>
            {
                var thisCard = ThisGlobal!.ClueInfo(x);
                if (ThisMod!.GameBoard1!.CardPartOfSolution(thisCard) == false)
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
            ThisMod!.HandList!.HandList.ReplaceRange(SingleInfo.MainHandList);
            ThisMod.ThisPile!.ClearCards();
            ThisMod.HandList.Visible = true;
            ThisMod.ThisPile.Visible = true;
            WhoTurn = WhoStarts; //i think this was missing.
            if (ThisData!.MultiPlayer)
            {
                SaveRoot!.ImmediatelyStartTurn = true;
                await ThisNet!.SendRestoreGameAsync(SaveRoot);
            }
            await StartNewTurnAsync();
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status)
            {
                case "space":
                    ThisMod!.GameBoard1!.MoveToSpace(int.Parse(content));
                    await ContinueMoveAsync();
                    return;
                case "room":
                    ThisMod!.GameBoard1!.MoveToRoom(int.Parse(content));
                    SaveRoot!.MovesLeft = 0;
                    ThisMod.CurrentRoomName = ThisGlobal!.RoomList[int.Parse(content)].Name; //i think.
                    SaveRoot.GameStatus = EnumClueStatusList.MakePrediction;
                    ThisCheck!.IsEnabled = true; //i think this simple.
                    return;
                case "prediction":

                case "accusation":
                    SaveRoot!.CurrentPrediction = await js.DeserializeObjectAsync<PredictionInfo>(content);
                    ThisMod!.CurrentCharacterName = SaveRoot.CurrentPrediction.CharacterName;
                    ThisMod.CurrentRoomName = SaveRoot.CurrentPrediction.RoomName;
                    ThisMod.CurrentWeaponName = SaveRoot.CurrentPrediction.WeaponName;
                    if (status == "prediction")
                        await MakePredictionAsync();
                    else
                        await MakeAccusationAsync();
                    return;
                case "cluegiven":
                    var thisCard = ThisGlobal!.ClueInfo(int.Parse(content));
                    ThisMod!.ThisPile!.AddCard(thisCard);
                    SaveRoot!.GameStatus = EnumClueStatusList.EndTurn;
                    await ShowHumanCanPlayAsync();
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task ContinueTurnAsync()
        {
            if (DidChooseColors == false)
            {
                if (SaveRoot!.ImmediatelyStartTurn)
                    throw new BasicBlankException("Cannot mark beginnings until colors are chosen");
                await base.ContinueTurnAsync();
                return;
            }
            await EndStepAsync(); //decided to use this instead.
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            if (DidChooseColors == true)
            {
                ThisMod!.GameBoard1!.NewTurn();
                SaveRoot!.Instructions = "None";
                SaveRoot.AccusationMade = false;
                SaveRoot.CurrentPrediction = new PredictionInfo();
                OtherTurn = 0;
                ThisMod.CurrentCharacterName = "";
                ThisMod.CurrentRoomName = "";
                ThisMod.CurrentWeaponName = "";
                if (WhoTurn == 0)
                    throw new BasicBlankException("WhoTurn cannot be 0");
                SetCurrentPlayer();
                SaveRoot.GameStatus = EnumClueStatusList.StartTurn;
                SingleInfo = PlayerList!.GetWhoPlayer();
                await EndStepAsync();
                return;
            }
            SaveRoot!.ShowedMessage = false; //i think
            await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
        }
        public override async Task MakeMoveAsync(int space) //will not call this method this time.
        {
            //well see what we need for the move.
            await Task.CompletedTask;
        }
        public override async Task EndTurnAsync()
        {
            ThisMod!.CommandContainer!.ManuelFinish = true;
            WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
            ThisMod.ThisPile!.ClearCards();
            SaveRoot!.PreviousMoves = new System.Collections.Generic.Dictionary<int, MoveInfo>(); //i think.
            await StartNewTurnAsync();
        }
        private void SetCurrentPlayer()
        {
            ThisGlobal!.CurrentCharacter = ThisGlobal.CharacterList.Values.Single(items => items.Player == WhoTurn);
        }
        private void SetOtherPlayer()
        {
            if (OtherTurn == 0)
                throw new BasicBlankException("Cannot use the setotherplayer function when the otherturn is set to 0");
            ThisGlobal!.CurrentCharacter = ThisGlobal.CharacterList.Values.Single(items => items.Player == OtherTurn);
        }
        private async Task EndStepAsync()
        {
            ThisE.RepaintBoard(); //i think.
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
            if (ThisData!.MultiPlayer == false)
            {
                await ComputerRegularTurnAsync();
                return;
            }
            if (ThisData.MultiPlayer == true && OtherTurn == 0)
            {
                ThisCheck!.IsEnabled = true;
                return;
            }
            if (OtherTurn > 0)
            {
                SingleInfo = PlayerList!.GetOtherPlayer(); //i think.
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.Computer)
                {
                    if (ThisData.MultiPlayer == true && ThisData.Client)
                    {
                        ThisCheck!.IsEnabled = true;
                        return;
                    }
                    await ComputerRegularTurnAsync();
                    return;
                }
                if (ThisData.MultiPlayer)
                {
                    ThisCheck!.IsEnabled = true;
                    return;
                }
            }
        }
        protected override async Task ComputerTurnAsync()
        {
            if (DidChooseColors == true)
                throw new BasicBlankException("Should not goto computer turn");
            if (SingleInfo!.InGame == false)
                throw new BasicBlankException("Not even in game");
            await ComputerChooseColorsAsync();
        }
        private string CardToGive()
        {
            CustomBasicList<GivenInfo> thisList = new CustomBasicList<GivenInfo>();
            foreach (var thisGiven in ThisGlobal!.CurrentCharacter!.ComputerData.CluesGiven)
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
            ThisGlobal.CurrentCharacter.ComputerData.CluesGiven.Add(finCard);
            return finCard.Clue;
        }
        private async Task ComputerRegularTurnAsync()
        {
            if (OtherTurn > 0 && ThisData!.MultiPlayer && ThisData.Client)
                return;
            if (SaveRoot!.GameStatus == EnumClueStatusList.StartTurn)
            {
                await EndTurnAsync();
                return; //the computer has to skip their turns because it was really hosed.
            }
            if (SaveRoot.GameStatus == EnumClueStatusList.FindClues)
            {
                SetOtherPlayer();
                if (ThisGlobal!.CurrentCharacter is null)
                    throw new BasicBlankException("There is no current character before deciding on a card to give");
                string thisInfo = CardToGive();
                ClueBoardGamePlayerItem newPlayer = PlayerList!.GetOtherPlayer();
                CardInfo thisCard = newPlayer.MainHandList.Single(items => items.Name == thisInfo);
                SingleInfo = PlayerList.GetWhoPlayer();
                if (newPlayer.CanSendMessage(ThisData!) && WhoTurn != MyID)
                    await ThisNet!.SendToParticularPlayerAsync("cluegiven", thisCard.Deck, SingleInfo.NickName);
                SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
                if (WhoTurn == MyID)
                    ThisMod!.ThisPile!.AddCard(thisCard);
                await EndStepAsync();
                return;
            }
            throw new BasicBlankException("The computer should have skipped their turns since their moving was really hosed");
        }
        public override async Task AfterRollingAsync()
        {
            SaveRoot!.GameStatus = EnumClueStatusList.DiceRolled;
            ThisMod!.GameBoard1!.ResetMoves();
            if (ThisTest!.DoubleCheck == false)
            {
                SaveRoot.DiceNumber = ThisMod.ThisCup!.ValueOfOnlyDice;
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
            foreach (var tempRoom in ThisGlobal!.RoomList.Values)
            {
                x++;
                if (tempRoom.Name == thisRoom.Name)
                    return x;
            }
            throw new BasicBlankException("Room Not Found");
        }
        private void PlaceObjectsOnBoard()
        {
            var thisRoom = ThisGlobal!.GetRoom(SaveRoot!.CurrentPrediction!.RoomName);
            var thisWeapon = ThisGlobal.GetWeapon(SaveRoot.CurrentPrediction.WeaponName);
            var thisCharacter = ThisGlobal.GetCharacter(SaveRoot.CurrentPrediction.CharacterName);
            thisCharacter.CurrentRoom = ManuallyGetKey(thisRoom);
            thisCharacter.Space = 0;
            thisWeapon.Room = thisCharacter.CurrentRoom;
            ThisE.RepaintBoard();
        }
        public void PopulateDetectiveNoteBook()
        {
            if (ThisGlobal!.RoomList.Count != 9)
                throw new BasicBlankException("Needs 9 rooms");
            DetectiveInfo thisD;
            foreach (var thisRoom in ThisGlobal.RoomList.Values)
            {
                thisD = new DetectiveInfo();
                thisD.Category = EnumCardType.IsRoom;
                thisD.IsChecked = false;
                thisD.Name = thisRoom.Name;
                ThisGlobal.DetectiveList.Add(thisD);
            }
            CustomBasicList<string> originalList = new CustomBasicList<string> { "Mr. Green", "Colonel Mustard", "Mrs. Peacock", "Professor Plum",
                                                     "Miss Scarlet", "Mrs. White"};
            originalList.ForEach(thisItem =>
            {
                thisD = new DetectiveInfo();
                thisD.Category = EnumCardType.IsCharacter;
                thisD.IsChecked = false;
                thisD.Name = thisItem;
                ThisGlobal.DetectiveList.Add(thisD);
            });
            if (ThisGlobal.WeaponList.Count != 6)
                throw new BasicBlankException("Need 6 weapons when populating detective notebook");
            foreach (var thisWeapon in ThisGlobal.WeaponList.Values)
            {
                thisD = new DetectiveInfo();
                thisD.Category = EnumCardType.IsWeapon;
                thisD.IsChecked = false;
                thisD.Name = thisWeapon.Name;
                ThisGlobal.DetectiveList.Add(thisD);
            }
        }
        public async Task MakeAccusationAsync()
        {
            ThisMod!.CommandContainer!.ManuelFinish = true;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.CanSendMessage(ThisData!))
            {
                await ThisNet!.SendAllAsync("accusation", SaveRoot!.CurrentPrediction!);
            }
            PlaceObjectsOnBoard();
            if (SaveRoot!.CurrentPrediction!.CharacterName == SaveRoot.Solution.CharacterName &&
                SaveRoot.CurrentPrediction.RoomName == SaveRoot.Solution.RoomName &&
                SaveRoot.CurrentPrediction.WeaponName == SaveRoot.Solution.WeaponName)
            {
                await ThisMod.ShowGameMessageAsync($"The accusation was correct.  {SaveRoot.CurrentPrediction.CharacterName} did it in the {SaveRoot.CurrentPrediction.RoomName} with the {SaveRoot.CurrentPrediction.WeaponName}");
                await ShowWinAsync();
                return;
            }
            if (WhoTurn == MyID)
                await ThisMod.ShowGameMessageAsync($"Sorry, the accusation was not correct {SingleInfo.NickName}.  You are out of the game but need to still be there in order to prove other predictions wrong.");
            SingleInfo.InGame = false;
            if (PlayerList.Count(items => items.InGame == true) <= 1)
            {
                await ThisMod.ShowGameMessageAsync($"Sorry, nobody got the solution correct.  Therefore, nobody won.  The solution was {Constants.vbCrLf} {SaveRoot.CurrentPrediction.CharacterName} did it in the {SaveRoot.CurrentPrediction.RoomName} with the {SaveRoot.CurrentPrediction.WeaponName}");
                await ShowTieAsync();
                return;
            }
            SaveRoot.GameStatus = EnumClueStatusList.EndTurn;
            await EndStepAsync();
        }
        public async Task ContinueMoveAsync()
        {
            if (ThisMod!.GameBoard1!.HasValidMoves() == false)
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
            ThisMod!.CommandContainer!.ManuelFinish = true;
            OtherTurn = 0;
            var thisRoom = ThisGlobal!.RoomList[ThisGlobal.CurrentCharacter!.CurrentRoom];
            ThisGlobal.CurrentCharacter.PreviousRoom = ThisGlobal.CurrentCharacter.CurrentRoom;
            ThisMod.CurrentRoomName = thisRoom.Name;
            SaveRoot!.CurrentPrediction!.RoomName = thisRoom.Name;
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (SingleInfo.PlayerCategory != EnumPlayerCategory.Self && ThisTest!.NoAnimations == false)
                await Delay!.DelaySeconds(.75);
            if (SingleInfo.CanSendMessage(ThisData!))
                await ThisNet!.SendAllAsync("prediction", SaveRoot.CurrentPrediction);
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
                foreach (var thisRoom in ThisGlobal!.RoomList.Values)
                {
                    if (thisRoom.Name != SaveRoot!.CurrentPrediction!.RoomName)
                    {
                        thisRe = new ReceivedInfo();
                        thisRe.Name = thisRoom.Name;
                        ThisGlobal.CurrentCharacter!.ComputerData.CluesReceived.Add(thisRe);
                    }
                }
            }
            foreach (var thisCharacter in ThisGlobal!.CharacterList.Values)
            {
                if (thisCharacter.Name != SaveRoot!.CurrentPrediction!.CharacterName)
                {
                    thisRe = new ReceivedInfo();
                    thisRe.Name = thisCharacter.Name;
                    ThisGlobal.CurrentCharacter!.ComputerData.CluesReceived.Add(thisRe);
                }
            }
            foreach (var thisWeapon in ThisGlobal.WeaponList.Values)
            {
                if (thisWeapon.Name != SaveRoot!.CurrentPrediction!.WeaponName)
                {
                    thisRe = new ReceivedInfo();
                    thisRe.Name = thisWeapon.Name;
                    ThisGlobal.CurrentCharacter!.ComputerData.CluesReceived.Add(thisRe);
                }
            }
        }
        public async Task ComputerFoundClueAsync(string whatClue)
        {
            ReceivedInfo thisRe = new ReceivedInfo();
            thisRe.Name = whatClue;
            SetCurrentPlayer();
            if (whatClue == ThisGlobal!.CurrentCharacter!.ComputerData.Weapon)
                ThisGlobal.CurrentCharacter.ComputerData.Weapon = "";
            else if (whatClue == ThisGlobal.CurrentCharacter.ComputerData.Character)
                ThisGlobal.CurrentCharacter.ComputerData.Character = "";
            else if (whatClue == ThisGlobal.CurrentCharacter.ComputerData.RoomHeaded)
                ThisGlobal.CurrentCharacter.ComputerData.RoomHeaded = "";
            ThisGlobal.CurrentCharacter.ComputerData.CluesReceived.Add(thisRe);
            await EndStepAsync();
        }
    }
}