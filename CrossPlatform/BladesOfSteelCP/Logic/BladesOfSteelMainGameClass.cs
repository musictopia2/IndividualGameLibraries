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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BladesOfSteelCP.Data;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceMessages;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions; //most likely will be used.
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;

namespace BladesOfSteelCP.Logic
{
    [SingletonGame]
    public class BladesOfSteelMainGameClass : CardGameClass<RegularSimpleCard, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>, IMiscDataNM
    {
        

        private readonly BladesOfSteelVMData _model;
        private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
        private readonly BladesOfSteelGameContainer _gameContainer; //if we don't need it, take it out.
        private readonly IFaceoffProcesses _processes;
        private readonly ComputerAI _ai;
        private readonly BladesOfSteelScreenDelegates _delegates;
        private bool _drewCard;
        private bool _firstDraw;
        public BladesOfSteelMainGameClass(IGamePackageResolver mainContainer,
            IEventAggregator aggregator,
            BasicData basicData,
            TestOptions test,
            BladesOfSteelVMData currentMod,
            IMultiplayerSaveState state,
            IAsyncDelayer delay,
            ICardInfo<RegularSimpleCard> cardInfo,
            CommandContainer command,
            BladesOfSteelGameContainer gameContainer,
            IFaceoffProcesses processes,
            ComputerAI ai,
            BladesOfSteelScreenDelegates delegates
            )
            : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer)
        {
            _model = currentMod;
            _command = command;
            _gameContainer = gameContainer;
            _processes = processes;
            _ai = ai;
            _delegates = delegates;
            _gameContainer.GetAttackStage = GetAttackStage;
            _gameContainer.GetDefenseStage = GetDefenseStage;
        }
        protected override bool ShowNewCardDrawn(BladesOfSteelPlayerItem tempPlayer)
        {
            return _drewCard;
        }
        protected override void SortAfterDrawing()
        {
            if (_firstDraw == false)
            {
                base.SortAfterDrawing();
                return;
            }
            SingleInfo = PlayerList!.GetSelf();
            if (SingleInfo.MainHandList.Count == 6)
                SortCards();
        }
        protected override async Task AfterDrawingAsync()
        {
            if (_firstDraw == false)
            {
                if (SingleInfo!.MainHandList.Count != 6)
                    throw new BasicBlankException("Should have 6 cards in hand after drawing");
                await base.AfterDrawingAsync();
                return;
            }
            if (PlayerList.First().MainHandList.Count == 6 && PlayerList.Last().MainHandList.Count == 6)
            {
                _firstDraw = false;
            }
            await EndTurnAsync();
        }
        protected override void LinkHand()
        {
            _model!.PlayerHand1!.HandList = SingleInfo!.MainHandList; //hopefully this is it.
            PrepSort();
        }
        public override async Task DrawAsync()
        {
            if (SaveRoot!.IsFaceOff == true)
            {
                if (SingleInfo!.FaceOff != null)
                    throw new BasicBlankException("Player already has a faceoff card.  Therefore; cannot get another faceoff card");
                if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
                {
                    Check!.IsEnabled = true;
                    return; //has to wait for other player to send the faceoff card info
                }
                int deck = _gameContainer.DeckList!.ToRegularDeckDict().GetRandomItem().Deck;
                RegularSimpleCard thisCard = new RegularSimpleCard();
                thisCard.Populate(deck);
                if (BasicData!.MultiPlayer == true)
                    await Network!.SendAllAsync("faceoff", deck);
                await _processes.FaceOffCardAsync(thisCard); //decided to risk letting this handle faceoff.
                return;
            }
            await base.DrawAsync();
        }

        private async Task StartDrawingAsync()
        {
            if (SaveRoot!.IsFaceOff == true)
            {
                await ContinueTurnAsync();
                return;
            }
            SingleInfo = PlayerList!.GetWhoPlayer();
            if (_firstDraw == true)
            {
                if (SingleInfo.MainHandList.Count == 6)
                    throw new BasicBlankException("Cannot have 6 cards because its first draw");
                LeftToDraw = 0;
                await DrawAsync();
                return;
            }
            if (SingleInfo.MainHandList.Count == 6)
            {
                await ContinueTurnAsync();
                return;
            }
            if (SingleInfo.MainHandList.Count > 6)
                throw new BasicBlankException("Cannot have more than 6 cards in hand");
            _drewCard = SingleInfo.MainHandList.Count > 0;
            LeftToDraw = 6 - SingleInfo.MainHandList.Count;
            if (LeftToDraw > _model!.Deck1!.CardsLeft())
            {
                await StartEndAsync();
                return;
            }
            PlayerDraws = WhoTurn;
            if (LeftToDraw == 1)
            {
                LeftToDraw = 0;
                PlayerDraws = 0; //because it will do the part automatically;
            }
            await DrawAsync();
        }
        protected override void GetPlayerToContinueTurn()
        {
            if (SaveRoot!.PlayOrder.OtherTurn == 0)
            {
                _model!.OtherPlayer = "None";
                if (SaveRoot.IsFaceOff == true)
                    _model.Instructions = "Face-Off.  Click the deck to draw a card at random.  Whoever draws a higher number goes first for the game.  If there is a tie; then repeat.";
                else if (_model.MainDefense1!.HasCards == true)
                {
                    _model.Instructions = "Look at the results to see that the goal was blocked and end turn.";
                }
                else
                    _model.Instructions = "Either throw away all 6 of your cards, choose to attack or choose cards for defense.";
                base.GetPlayerToContinueTurn();
                return;
            }
            SingleInfo = PlayerList!.GetOtherPlayer();
            _model!.OtherPlayer = SingleInfo.NickName;
            _model.Instructions = "Either choose cards for defense or choose to pass to allow the goal to go through";
        }
        private int CalculateWin
        {
            get
            {
                if (PlayerList.First().Score == PlayerList.Last().Score)
                    return 0;
                if (PlayerList.First().Score > PlayerList.Last().Score)
                    return 1;
                return 2;
            }
        }
        protected override async Task ShowHumanCanPlayAsync()
        {
            await base.ShowHumanCanPlayAsync();
            _model.YourDefensePile.ReportCanExecuteChange();
        }
        private async Task StartEndAsync()
        {
            int whoWon = CalculateWin;
            if (whoWon == 0)
            {
                await UIPlatform.ShowMessageAsync("There was a tie.  Therefore; all the cards are being reshuffled.  Then a faceoff will happen again to see who goes first and the cards are passed out.  Whoever gets the first point in this round wins");
                SaveRoot!.WasTie = true;
                if (BasicData!.MultiPlayer == true && BasicData.Client == true)
                {
                    Check!.IsEnabled = true; //to wait to hear back from host again.
                    return;
                }
                if (_delegates.ReloadFaceoffAsync == null)
                {
                    throw new BasicBlankException("Nobody is reloading the faceoff.  Rethink");
                }
                await _delegates.ReloadFaceoffAsync();
                WhoTurn = WhoStarts;
                WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
                await SetUpGameAsync(false); //hopefully this works.
                return;
            }
            WhoTurn = whoWon;
            SingleInfo = PlayerList!.GetWhoPlayer();
            SaveRoot!.WasTie = false;
            await ShowWinAsync();
        }

        public override Task FinishGetSavedAsync()
        {
            LoadControls();
            LoadPlayerControls();
            LinkRestPiles();
            //anything else needed is here.
            return base.FinishGetSavedAsync();
        }
        private void LoadPlayerControls()
        {
            SaveRoot!.LoadMod(_model!);
            foreach (var player in PlayerList)
            {
                if (player.PlayerCategory == EnumPlayerCategory.Self)
                {
                    player.DefensePile = _model.YourDefensePile;
                    
                    player.AttackPile = _model.YourAttackPile;
                  
                }
                else
                {
                    player.DefensePile = _model.OpponentDefensePile;
                    player.AttackPile = _model.OpponentAttackPile;
                }
                player.DefensePile.LoadBoard(player);
                player.AttackPile.LoadBoard(player);
            }


        }
        private void LoadControls()
        {
            if (IsLoaded == true)
                return;

            IsLoaded = true; //i think needs to be here.
        }
        protected override async Task ComputerTurnAsync()
        {
            if (Test!.NoAnimations == false)
                await Delay!.DelaySeconds(1);
            if (SaveRoot!.IsFaceOff == true)
            {
                await DrawAsync();
                return;
            }
            if (Test.DoubleCheck == true)
                return; //decided to make it stuck when double checking.  not during faceoffs though.
            if (_model!.MainDefense1!.HasCards == true)
            {
                await EndTurnAsync();
                return;
            }
            if (_gameContainer.OtherTurn > 0)
            {
                var (defenseStep, cardList1) = _ai!.CardsForDefense();
                if (defenseStep == EnumDefenseStep.Pass)
                {
                    await PassDefenseAsync();
                    return;
                }
                if (defenseStep == EnumDefenseStep.Hand)
                    await PlayDefenseCardsAsync(cardList1, true);
                else
                    await PlayDefenseCardsAsync(cardList1, false);
                return;
            }
            var (firstStep, cardList2) = _ai!.GetFirstMove();
            if (firstStep == EnumFirstStep.ThrowAwayAllCards)
            {
                await ThrowAwayAllCardsFromHandAsync();
                return;
            }
            if (firstStep == EnumFirstStep.PlayDefense)
            {
                if (SingleInfo!.DefenseList.Count + cardList2.Count > 3)
                {
                    await ThrowAwayDefenseCardsAsync();
                    return;
                }
                await AddCardsToDefensePileAsync(cardList2);
                return;
            }
            await PlayAttackCardsAsync(cardList2);
        }
        protected override Task StartSetUpAsync(bool isBeginning)
        {
            if (IsLoaded == false)
                LoadPlayerControls();
            LoadControls();
            if (_model!.MainDefense1!.HasCards == true)
                throw new BasicBlankException("Cannot start setup when there are cards in defense");
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.MainHandList.Clear();
                thisPlayer.DefenseList.Clear();
                thisPlayer.AttackList.Clear();
                thisPlayer.Score = 0;
                thisPlayer.FaceOff = null;
            });
            SaveRoot.IsFaceOff = true; //hopefully still okay for multiplayer (?)
            return base.StartSetUpAsync(isBeginning);
        }
        protected override Task LastPartOfSetUpBeforeBindingsAsync()
        {
            LinkRestPiles();
            _model!.Pile1!.ClearCards(); //i think this is where it should have been at.
            return base.LastPartOfSetUpBeforeBindingsAsync();
        }
        private void LinkRestPiles()
        {
            PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.DefensePile!.HandList = thisPlayer.DefenseList;
                thisPlayer.AttackPile!.HandList = thisPlayer.AttackList;
            });
            if (_model!.OpponentFaceOffCard != null)
            {
                _model.OpponentFaceOffCard.ClearCards();
                _model.YourFaceOffCard!.ClearCards();
            }
            _firstDraw = true;
        }
        async Task IMiscDataNM.MiscDataReceived(string status, string content)
        {
            switch (status) //can't do switch because we don't know what the cases are ahead of time.
            {
                case "passdefense":
                    await PassDefenseAsync();
                    return;
                case "faceoff":
                    RegularSimpleCard thisCard = new RegularSimpleCard();
                    thisCard.Populate(int.Parse(content));
                    await _processes.FaceOffCardAsync(thisCard);
                    return;
                case "throwawaydefense":
                    await ThrowAwayDefenseCardsAsync();
                    return;
                case "throwawayallcardsfromhand":
                    await ThrowAwayAllCardsFromHandAsync();
                    return;
                case "defensehand":
                    var thisList = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                    await PlayDefenseCardsAsync(thisList, true);
                    return;
                case "defenseboard":
                    var thisList2 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                    await PlayDefenseCardsAsync(thisList2, false);
                    return;
                case "attack":
                    var thisList3 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                    await PlayAttackCardsAsync(thisList3);
                    return;
                case "addtodefense":
                    var thisList4 = await content.GetNewObjectListFromDeckListAsync(_gameContainer.DeckList!);
                    await AddCardsToDefensePileAsync(thisList4);
                    return;
                default:
                    throw new BasicBlankException($"Nothing for status {status}  with the message of {content}");
            }
        }
        public override async Task StartNewTurnAsync()
        {
            await base.StartNewTurnAsync();
            await StartDrawingAsync();
        }
        public override async Task EndTurnAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.
            _gameContainer.OtherTurn = 0;
            if (_model!.MainDefense1!.HasCards == true)
            {
                DeckRegularDict<RegularSimpleCard> thisList = _model.MainDefense1.HandList.ToRegularDeckDict();
                await thisList.ForEachAsync(async thisCard =>
                {
                    _model.MainDefense1.HandList.RemoveSpecificItem(thisCard);
                    await AnimatePlayAsync(thisCard); //i think.
                    if (Test!.NoAnimations == false)
                        await Delay!.DelaySeconds(.1);
                });

                if (SingleInfo.AttackList.Count == 0)
                    throw new BasicBlankException("The main defense cannot have any cards because there are no cards for attack");
                thisList = SingleInfo.AttackList.ToRegularDeckDict();
                await thisList.ForEachAsync(async thisCard =>
                {
                    SingleInfo.AttackList.RemoveObjectByDeck(thisCard.Deck);
                    await AnimatePlayAsync(thisCard); //i think.
                    if (Test!.NoAnimations == false)
                        await Delay!.DelaySeconds(.1);
                });
                if (SingleInfo.AttackList.Count > 0)
                    throw new BasicBlankException("Must have 0 cards left for attack");
            }

            _command.ManuelFinish = true; //because it could be somebody else's turn.
            WhoTurn = await PlayerList.CalculateWhoTurnAsync();
            await StartNewTurnAsync();
        }


        #region "public functions"
        public EnumAttackGroup GetAttackStage(ICustomBasicList<RegularSimpleCard> thisList)
        {
            if (thisList.Any(items => items.Color == EnumColorList.Black))
                throw new BasicBlankException("Cannot get the attack stage if black cards are present");
            if (thisList.Count < 2)
                throw new BasicBlankException("Must have at least 2 cards for attack");
            if (thisList.Count == 2)
            {
                if (thisList.All(items => items.Value == EnumCardValueList.Nine))
                    return EnumAttackGroup.GreatOne;
                if (thisList.HasDuplicates(items => items.Value))
                    return EnumAttackGroup.OneTimer;
                if (thisList.Any(items => items.Value == EnumCardValueList.HighAce))
                {
                    bool suitMatch = thisList.HasDuplicates(items => items.Suit);
                    if (suitMatch == true)
                        return EnumAttackGroup.BreakAway;
                }
                return EnumAttackGroup.HighCard;
            }
            if (thisList.Count > 3)
                throw new BasicBlankException("Can only attack with 3 cards at the most");
            int counts = thisList.DistinctCount(items => items.Suit);
            if (counts == 1)
                return EnumAttackGroup.Flush;
            return EnumAttackGroup.HighCard;
        }
        public EnumDefenseGroup GetDefenseStage(ICustomBasicList<RegularSimpleCard> thisList)
        {
            if (thisList.Any(items => items.Color == EnumColorList.Red))
                throw new BasicBlankException("Cannot get the defense stage if there are red cards present");
            if (thisList.Count == 0)
                throw new BasicBlankException("Must have at least one card for defense");
            if (thisList.Count > 3)
                throw new BasicBlankException("Cannot defend with more than 3 cards");
            if (thisList.Count == 1)
            {
                if (thisList.Single().Value == EnumCardValueList.HighAce)
                    return EnumDefenseGroup.StarGoalie;
                return EnumDefenseGroup.HighCard;
            }
            int counts = thisList.DistinctCount(items => items.Suit);
            if (thisList.Count == 2)
            {
                if (thisList.HasDuplicates(items => items.Value) == true) //this will imply this time.
                    return EnumDefenseGroup.StarDefense;
                return EnumDefenseGroup.HighCard;
            }
            if (counts == 1)
                return EnumDefenseGroup.Flush;
            return EnumDefenseGroup.HighCard;
        }
        public async Task<bool> CanPlayAttackCardsAsync(IDeckDict<RegularSimpleCard> thisList)
        {
            if (thisList.Count < 2)
            {
                await UIPlatform.ShowMessageAsync("Must have at least 2 cards for attacking");
                return false;
            }
            if (thisList.Any(items => items.Color == EnumColorList.Black))
            {
                await UIPlatform.ShowMessageAsync("Cannot attack with black (defense) cards");
                return false;
            }
            if (thisList.Count > 3)
            {
                await UIPlatform.ShowMessageAsync("Cannot attack with more than 3 cards");
                return false;
            }
            return true;
        }
        public async Task<bool> CanPlayDefenseCardsAsync(IDeckDict<RegularSimpleCard> thisList)
        {
            if (thisList.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose at least one card");
                return false;
            }
            if (thisList.Count > 3)
            {
                await UIPlatform.ShowMessageAsync("Cannot play more than 3 cards for defense");
                return false;
            }
            if (thisList.Any(items => items.Color == EnumColorList.Red))
            {
                await UIPlatform.ShowMessageAsync("Cannot use attack (red) cards for defense");
                return false;
            }
            return true;
        }
        public async Task PlayAttackCardsAsync(IDeckDict<RegularSimpleCard> thisList)
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            SingleInfo.MainHandList.RemoveGivenList(thisList); //hopefully this simple.
            thisList.UnhighlightObjects();
            if (SingleInfo.AttackList.Count > 0)
                throw new BasicBlankException("There are already attack cards played.  There should not have been able to play attack cards");
            SingleInfo.AttackList.AddRange(thisList);
            if (WhoTurn == 1)
                _gameContainer.OtherTurn = 2;
            else
                _gameContainer.OtherTurn = 1;
            await ContinueTurnAsync();
        }
        public async Task PlayDefenseCardsAsync(IDeckDict<RegularSimpleCard> defenseList, bool fromHand)
        {
            if (fromHand == true)
            {
                SingleInfo!.MainHandList.RemoveSelectedItems(defenseList); //well see if this works or not (?)
            }
            else
                SingleInfo!.DefenseList.RemoveSelectedItems(defenseList);
            _model.MainDefense1!.PopulateObjects(defenseList);
            _gameContainer.OtherTurn = 0;
            await ContinueTurnAsync();
        }
        public async Task AddCardsToDefensePileAsync(IDeckDict<RegularSimpleCard> defenseList)
        {
            SingleInfo!.MainHandList!.RemoveGivenList(defenseList); //maybe works both ways (?)
            SingleInfo.DefensePile!.PopulateObjects(defenseList);
            await EndTurnAsync();
        }
        public async Task PassDefenseAsync()
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            await UIPlatform.ShowMessageAsync($"{SingleInfo.NickName} has scored a goal");
            SingleInfo.Score++;
            SingleInfo.AttackList.Clear();
            if (SaveRoot!.WasTie == true)
            {
                await ShowWinAsync(); //this one won period now.
                return;
            }
            await EndTurnAsync();
        }
        public async Task ThrowAwayDefenseCardsAsync()
        {
            if (SingleInfo!.DefenseList.Count == 0)
                throw new BasicBlankException("There are no defense cards to throw away");
            var thisList = SingleInfo.DefenseList.ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                SingleInfo.DefenseList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard);
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            if (SingleInfo.DefenseList.Count > 0)
                throw new BasicBlankException("All the defense cards should be gone");
            await EndTurnAsync(); //i think.
        }
        public async Task ThrowAwayAllCardsFromHandAsync()
        {
            var thisList = SingleInfo!.MainHandList.ToRegularDeckDict();
            await thisList.ForEachAsync(async thisCard =>
            {
                SingleInfo.MainHandList.RemoveObjectByDeck(thisCard.Deck);
                await AnimatePlayAsync(thisCard);
                if (Test!.NoAnimations == false)
                    await Delay!.DelaySeconds(.1);
            });
            if (SingleInfo.MainHandList.Count > 0)
                throw new BasicBlankException("Should have 0 cards left after discarding all your cards");
            await EndTurnAsync();
        }
        #endregion

    }
}
