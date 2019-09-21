using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommandClasses;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace FluxxCP
{
    [SingletonGame]
    public class ActionViewModel : ObservableObject, IAction
    {
        public DetailCardViewModel? ActionDetail;
        public DetailCardViewModel? CurrentDetail;
        private readonly CustomBasicList<int> _tempList = new CustomBasicList<int>();
        private int _loads;
        public ListViewPicker? Direction1;
        public ListViewPicker? Rule1;
        public ListViewPicker? Player1;
        public ListViewPicker? CardList1;
        public CustomBasicList<int>? TempRuleList = new CustomBasicList<int>();
        private EnumActionCategory _ActionCategory;
        private readonly FluxxViewModel _thisMod;
        private readonly FluxxMainGameClass _mainGame;
        private readonly IFluxxEvent _thisEvent;
        public HandViewModel<GoalCard>? PrivateGoals;
        public HandViewModel<FluxxCardInformation>? YourCards;
        public HandViewModel<KeeperCard>? YourKeepers;
        public HandViewModel<FluxxCardInformation>? OtherHand;
        public HandViewModel<FluxxCardInformation>? TempHand;
        public ActionViewModel(FluxxViewModel thisMod, FluxxMainGameClass mainGame)
        {
            _thisMod = thisMod;
            _mainGame = mainGame;
            _thisEvent = _thisMod;
        }
        public EnumActionCategory ActionCategory
        {
            get { return _ActionCategory; }
            set
            {
                if (SetProperty(ref _ActionCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _RulesToDiscard;
        public int RulesToDiscard
        {
            get { return _RulesToDiscard; }
            set
            {
                if (SetProperty(ref _RulesToDiscard, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        #region "Filled Properties
        private int _IndexPlayer = -1;
        public int IndexPlayer
        {
            get { return _IndexPlayer; }
            set
            {
                if (SetProperty(ref _IndexPlayer, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _IndexDirection = -1;
        public int IndexDirection
        {
            get { return _IndexDirection; }
            set
            {
                if (SetProperty(ref _IndexDirection, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _IndexRule = -1;
        public int IndexRule
        {
            get { return _IndexRule; }
            set
            {
                if (SetProperty(ref _IndexRule, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _IndexCard;
        public int IndexCard
        {
            get { return _IndexCard; }
            set
            {
                if (SetProperty(ref _IndexCard, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _ActionFrameText = "Action Card Information";
        public string ActionFrameText
        {
            get { return _ActionFrameText; }
            set
            {
                if (SetProperty(ref _ActionFrameText, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        #endregion
        #region "Misc Visible Properties"
        public bool EntireVisible => _thisMod.ActionVisible;
        private bool _ButtonChooseCardVisible;
        public bool ButtonChooseCardVisible
        {
            get { return _ButtonChooseCardVisible; }
            set
            {
                if (SetProperty(ref _ButtonChooseCardVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _ButtonChoosePlayerVisible;
        public bool ButtonChoosePlayerVisible
        {
            get { return _ButtonChoosePlayerVisible; }
            set
            {
                if (SetProperty(ref _ButtonChoosePlayerVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        #endregion
        #region "Commands"
        public BasicGameCommand? DirectionCommand { get; set; }
        public BasicGameCommand? ShowKeepersCommand { get; set; }
        public BasicGameCommand? ChooseCardCommand { get; set; }
        public BasicGameCommand? GiveCardsCommand { get; set; }
        public BasicGameCommand? ChoosePlayerCommand { get; set; }
        public BasicGameCommand? ViewRuleCardCommand { get; set; }
        public BasicGameCommand? DiscardRulesCommand { get; set; }
        public BasicGameCommand? SelectCardCommand { get; set; }
        public BasicGameCommand? ViewCardCommand { get; set; }
        private void LoadOtherCommands()
        {
            DirectionCommand = new BasicGameCommand(_thisMod, async items =>
            {
                ShowMainScreenAgain();
                await _thisEvent.DirectionChosenAsync(IndexDirection);
            }, items =>
            {
                return _thisMod.FluxxScreenUsed == EnumActionScreen.ActionScreen && ActionCategory == EnumActionCategory.Directions && IndexDirection > -1;
            }, _thisMod, _thisMod.CommandContainer!);
            ShowKeepersCommand = new BasicGameCommand(_thisMod, items =>
            {
                _thisMod.FluxxScreenUsed = EnumActionScreen.KeeperScreen;
                _thisMod.KeeperControl1!.ShowKeepers();
            }, items =>
            {
                return true;
            }, _thisMod, _thisMod.CommandContainer!);
            ChooseCardCommand = new BasicGameCommand(_thisMod, async items =>
            {
                if (ActionCategory != EnumActionCategory.FirstRandom)
                {
                    if (_mainGame.ThisGlobal!.CurrentAction!.Deck != EnumActionMain.UseWhatYouTake)
                    {
                        if (TempHand!.ObjectSelected() == 0)
                        {
                            await _thisMod.ShowGameMessageAsync("Must choose a card");
                            return;
                        }
                        ShowMainScreenAgain();
                        await _thisEvent.CardToUseAsync(TempHand.ObjectSelected());
                        return;
                    }
                }
                if (OtherHand!.ObjectSelected() == 0)
                {
                    await _thisMod.ShowGameMessageAsync("Must choose a card");
                    return;
                }
                if (ActionCategory == EnumActionCategory.FirstRandom)
                {
                    ShowMainScreenAgain();
                    await _thisEvent.FirstCardRandomChosenAsync(OtherHand.ObjectSelected());
                    return;
                }
                if (IndexPlayer == -1)
                {
                    throw new BasicBlankException("Must have the player chosen in order to use what you take from another player");
                }
                ShowMainScreenAgain();
                await _thisEvent.CardChosenToPlayAtAsync(OtherHand.ObjectSelected(), IndexPlayer);
            }, items =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen || ButtonChooseCardVisible == false)
                    return false;
                return ActionCategory == EnumActionCategory.UseTake || ActionCategory == EnumActionCategory.DrawUse || ActionCategory == EnumActionCategory.FirstRandom;
            }, _thisMod, _thisMod.CommandContainer!);
            GiveCardsCommand = new BasicGameCommand(_thisMod, async yy =>
            {
                CustomBasicList<int> thisList;
                if (TempHand!.AutoSelect == HandViewModel<FluxxCardInformation>.EnumAutoType.SelectOneOnly)
                {
                    if (TempHand.ObjectSelected() == 0)
                    {
                        await _thisMod.ShowGameMessageAsync("Must choose a card to give to a player");
                        return;
                    }
                    thisList = new CustomBasicList<int>() { TempHand.ObjectSelected() };
                    ShowMainScreenAgain();
                    await _thisEvent.ChoseForEverybodyGetsOneAsync(thisList, IndexPlayer);
                    return;
                }
                if (TempHand.HowManySelectedObjects > 2)
                {
                    await _thisMod.ShowGameMessageAsync("Cannot choose more than 2 cards to give to player");
                    return;
                }
                int index = GetPlayerIndex(IndexPlayer);
                int howManySoFar = _mainGame.ThisGlobal!.EverybodyGetsOneList.Count(items => items.Player == index);
                howManySoFar += TempHand.HowManySelectedObjects;
                int extras = _mainGame.ThisGlobal.IncreaseAmount();
                int mosts = extras + 1;
                if (howManySoFar > mosts)
                {
                    await _thisMod.ShowGameMessageAsync($"Cannot choose more than 2 cards each for the player.  So far; you chose {howManySoFar} cards.");
                    return;
                }
                var finalList = TempHand.ListSelectedObjects();
                thisList = finalList.GetDeckListFromObjectList();
                ShowMainScreenAgain();
                await _thisEvent.ChoseForEverybodyGetsOneAsync(thisList, IndexPlayer);
            }, items =>
            {
                return _thisMod.FluxxScreenUsed == EnumActionScreen.ActionScreen && ActionCategory == EnumActionCategory.Everybody1 && IndexPlayer > -1;
            }, _thisMod, _thisMod.CommandContainer!);
            ChoosePlayerCommand = new BasicGameCommand(_thisMod, async items =>
            {
                ShowMainScreenAgain();
                if (_mainGame.ThisGlobal!.CurrentAction!.Deck == EnumActionMain.UseWhatYouTake)
                    await _thisEvent.ChosePlayerForCardChosenAsync(IndexPlayer);
                else if (_mainGame.ThisGlobal.CurrentAction.Deck == EnumActionMain.TradeHands)
                    await _thisEvent.TradeHandsAsync(IndexPlayer);
                else
                    throw new BasicBlankException("Not sure how to choose player from here.  Rethink");
            }, items =>
            {
                if (ButtonChoosePlayerVisible == false || IndexPlayer == -1)
                    return false;
                return CanEnableChoosePlayer();
            }, _thisMod, _thisMod.CommandContainer!);
            ViewRuleCardCommand = new BasicGameCommand(_thisMod, items =>
            {
                if (Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
                {
                    CurrentDetail!.ShowCard(_mainGame.SaveRoot!.RuleList[IndexRule + 1]);
                    return;
                }
                CurrentDetail!.ShowCard(_mainGame.SaveRoot!.RuleList[TempRuleList.Single() + 1]);
            }, items =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen || ActionCategory != EnumActionCategory.Rules)
                    return false;
                if (Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
                    return IndexRule > -1;
                return TempRuleList!.Count == 1;
            }, _thisMod, _thisMod.CommandContainer!);
            DiscardRulesCommand = new BasicGameCommand(_thisMod, async items =>
            {
                ShowMainScreenAgain();
                if (Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
                {
                    await _thisEvent.RuleTrashedAsync(IndexRule);
                    return;
                }
                await _thisEvent.RulesSimplifiedAsync(TempRuleList!);
            }, items =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen || ActionCategory != EnumActionCategory.Rules)
                    return false;
                if (Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
                    return IndexRule > -1;
                return TempRuleList!.Count <= RulesToDiscard;
            }, _thisMod, _thisMod.CommandContainer!);
            SelectCardCommand = new BasicGameCommand(_thisMod, async items =>
            {
                ShowMainScreenAgain();
                await _thisEvent.DoAgainSelectedAsync(IndexCard);
            }, items =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen || ActionCategory != EnumActionCategory.DoAgain)
                    return false;
                return IndexCard > -1;
            }, _thisMod, _thisMod.CommandContainer!);
            ViewCardCommand = new BasicGameCommand(_thisMod, items =>
            {
                var thisCard = GetCardToDoAgain(IndexCard);
                CurrentDetail!.ShowCard(thisCard);
            }, items =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen || ActionCategory != EnumActionCategory.DoAgain)
                    return false;
                return IndexCard > -1;
            }, _thisMod, _thisMod.CommandContainer!);
        }
        #endregion
        #region "Private Methods"
        private void SimpleLoadPlayers(bool includingSelf)
        {
            var tempLists = _mainGame.PlayerList.ToCustomBasicList();
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
            if (includingSelf == false)
                tempLists.RemoveSpecificItem(_mainGame.SingleInfo);
            _tempList.Clear();
            CustomBasicList<string> firstList = new CustomBasicList<string>();
            IndexPlayer = -1;
            tempLists.ForEach(thisPlayer =>
            {
                _tempList.Add(thisPlayer.Id);
                firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} hand, # {_tempList.Last()}");
            });
            Player1!.LoadTextList(firstList);
        }
        private void LoadDoAgainCards()
        {
            ActionCategory = EnumActionCategory.DoAgain;
            var firstList = _thisMod.Pile1!.DiscardList();
            var tempList = firstList.Where(items => items.CanDoCardAgain()).Select(items => items.Text()).ToCustomBasicList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Must have at least one card to do again.  Otherwise; can't do again");
            IndexCard = -1;
            CardList1!.LoadTextList(tempList);
        }
        private void LoadDirections()
        {
            ActionCategory = EnumActionCategory.Directions;
            Direction1!.UnselectAll();
            IndexDirection = -1;
        }
        private void LoadPlayers(bool includingSelf)
        {
            int oldSelectedIndex = IndexPlayer;
            SimpleLoadPlayers(includingSelf);
            if (_mainGame.ThisGlobal!.CurrentAction!.Deck == EnumActionMain.UseWhatYouTake)
            {
                if (_loads == 1)
                    IndexPlayer = _mainGame.SaveRoot!.SavedActionData.SelectedIndex;
                else
                    IndexPlayer = oldSelectedIndex;
                if (IndexPlayer > -1)
                    Player1!.SelectSpecificItem(IndexPlayer);
            }
        }
        private void LoadUseTake()
        {
            ActionCategory = EnumActionCategory.UseTake;
            LoadPlayers(false);
            if (IndexPlayer > -1)
            {
                OtherHand!.Visible = true;
                int index = GetPlayerIndex(IndexPlayer);
                var thisPlayer = _mainGame.PlayerList![index];
                ActionFrameText = "Other Cards";
                LoadOtherCards(thisPlayer);
            }
            else
            {
                ButtonChoosePlayerVisible = true;
            }
        }
        private void LoadTempCards()
        {
            TempHand!.Visible = true;
            if (_mainGame.ThisGlobal!.TempActionHandList.Count == 0)
                throw new BasicBlankException("There are no cards left for the temp cards");
            TempHand.AutoSelect = HandViewModel<FluxxCardInformation>.EnumAutoType.SelectOneOnly;
            var firstList = _mainGame.ThisGlobal.TempActionHandList.GetNewObjectListFromDeckList(_mainGame.DeckList!);
            firstList.ForEach(thisCard => thisCard.IsUnknown = false);
            TempHand.HandList.ReplaceRange(firstList);
        }
        private void LoadEverybodyGetsOne()
        {
            ActionCategory = EnumActionCategory.Everybody1;
            LoadTempCards();
            if (_mainGame.ThisGlobal!.EverybodyGetsOneList.Count == 0)
            {
                LoadPlayers(true);
                return;
            }
            var tempLists = PlayersLeftForEverybodyGetsOne();
            if (tempLists.Count < 2)
                throw new BasicBlankException("Cannot load everybody gets one because less than 2 players");
            _tempList.Clear();
            if (_mainGame.ThisGlobal.IncreaseAmount() == 1)
                TempHand!.AutoSelect = HandViewModel<FluxxCardInformation>.EnumAutoType.SelectAsMany;
            IndexPlayer = -1;
            CustomBasicList<string> firstList = new CustomBasicList<string>();
            tempLists.ForEach(thisPlayer =>
            {
                _tempList.Add(thisPlayer.Id);
                firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} cards in hand, {thisPlayer.Id}");
            });
            Player1!.LoadTextList(firstList);
        }
        private void LoadDrawUse()
        {
            ActionCategory = EnumActionCategory.DrawUse;
            LoadTempCards();
            ButtonChooseCardVisible = true;
        }
        private void LoadRules()
        {
            ActionCategory = EnumActionCategory.Rules;
            var tempList = _mainGame.SaveRoot!.RuleList.Where(items => items.Category != EnumRuleCategory.Basic).Select(items => items.Text()).ToCustomBasicList();
            if (tempList.Count == 0)
                throw new BasicBlankException("Cannot load the rules because there are no rules");
            if (_mainGame.ThisGlobal!.CurrentAction!.Deck == EnumActionMain.TrashANewRule)
            {
                Rule1!.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
                RulesToDiscard = 1;
            }
            else
            {
                Rule1!.SelectionMode = ListViewPicker.EnumSelectionMode.MultipleItems;
                RulesToDiscard = tempList.RulesThatCanBeDiscarded();
            }
            Rule1.LoadTextList(tempList);
        }
        private bool CanEnableChoosePlayer()
        {
            if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                return false;
            if (ActionCategory == EnumActionCategory.TradeHands || ActionCategory == EnumActionCategory.UseTake || ActionCategory == EnumActionCategory.Everybody1)
            {
                if (ActionCategory == EnumActionCategory.UseTake && ButtonChoosePlayerVisible == false)
                    return false;
                var thisC = _mainGame.ThisGlobal!.CurrentAction!.Deck;
                if (thisC == EnumActionMain.UseWhatYouTake && _mainGame.SaveRoot!.SavedActionData.SelectedIndex == -1 || thisC != EnumActionMain.UseWhatYouTake)
                    return true;
            }
            return false;
        }
        private void LoadOtherCards(FluxxPlayerItem thisPlayer)
        {
            var thisList = thisPlayer.MainHandList.Select(items => items.Deck).ToCustomBasicList();
            DeckRegularDict<FluxxCardInformation> newList = new DeckRegularDict<FluxxCardInformation>();
            thisList.ForEach(thisItem =>
            {
                var thisCard = FluxxDetailClass.GetNewCard(thisItem);
                thisCard.Populate(thisItem); //i think this too.
                thisCard.IsUnknown = true;
                newList.Add(thisCard);
            });
            OtherHand!.HandList.ReplaceRange(newList);
            ButtonChooseCardVisible = true;
            OtherHand.Visible = true;
        }
        private void ShowMainScreenAgain()
        {
            _thisMod.Title = "Fluxx";
            _thisMod.FluxxScreenUsed = EnumActionScreen.None;
        }
        private CustomBasicList<FluxxPlayerItem> PlayersLeftForEverybodyGetsOne()
        {
            int extras = _mainGame.ThisGlobal!.IncreaseAmount();
            int mosts = extras + 1;
            var tempLists = _mainGame.PlayerList.ToCustomBasicList();
            tempLists.RemoveAllOnly(thisPlayer =>
            {
                return _mainGame.ThisGlobal.EverybodyGetsOneList.Count(items => items.Player == thisPlayer.Id) == mosts;
            });
            return tempLists;
        }
        private void ShowCard(FluxxCardInformation thisCard)
        {
            if (thisCard.IsUnknown == true)
                return;
            if (thisCard.Deck == CurrentDetail!.CurrentCard.Deck)
                CurrentDetail.ResetCard();
            else
                CurrentDetail.ShowCard(thisCard);
        }
        #endregion
        void IAction.LoadSavedGame()
        {
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            YourCards!.HandList = _mainGame.SingleInfo.MainHandList;
            YourKeepers!.HandList = _mainGame.SingleInfo.KeeperList;
            _mainGame.SingleInfo = _mainGame.PlayerList.GetWhoPlayer();
            if (_mainGame.ThisGlobal!.CurrentAction == null)
                return;
            if (_mainGame.ThisGlobal.CurrentAction.Deck == EnumActionMain.UseWhatYouTake && _mainGame.SaveRoot!.SavedActionData.SelectedIndex > -1)
            {
                SimpleLoadPlayers(true);
                IndexPlayer = _mainGame.SaveRoot.SavedActionData.SelectedIndex;
                if (IndexPlayer == -1)
                    throw new BasicBlankException("Rethink for reloading game");
                Player1!.SelectSpecificItem(IndexPlayer);
            }
        }
        void IAction.LoadActionScreen()
        {
            _loads++;
            _thisMod.Title = "Action";
            _thisMod.FluxxScreenUsed = EnumActionScreen.ActionScreen;
            CurrentDetail!.ResetCard();
            ButtonChooseCardVisible = false;
            ButtonChoosePlayerVisible = false;
            OtherHand!.Visible = false;
            ActionFrameText = "Action Card Information";
            if (_mainGame.IsFirstPlayRandom())
            {
                ActionCategory = EnumActionCategory.FirstRandom;
                OtherHand.Visible = true;
                var turnPlayer = _mainGame.PlayerList!.GetWhoPlayer();
                LoadOtherCards(turnPlayer);
                ActionFrameText = $"{_mainGame.SingleInfo!.NickName} will choose a card for {turnPlayer.NickName}";
                return;
            }
            ActionDetail!.ShowCard(_mainGame.ThisGlobal!.CurrentAction!);
            switch (_mainGame.ThisGlobal.CurrentAction!.Deck)
            {
                case EnumActionMain.TrashANewRule:
                case EnumActionMain.LetsSimplify:
                    LoadRules();
                    break;
                case EnumActionMain.LetsDoThatAgain:
                    LoadDoAgainCards();
                    break;
                case EnumActionMain.RotateHands:
                    LoadDirections();
                    break;
                case EnumActionMain.TradeHands:
                    ActionCategory = EnumActionCategory.TradeHands;
                    LoadPlayers(false);
                    ButtonChoosePlayerVisible = true;
                    break;
                case EnumActionMain.UseWhatYouTake:
                    LoadUseTake();
                    break;
                case EnumActionMain.EverybodyGets1:
                    LoadEverybodyGetsOne();
                    break;
                case EnumActionMain.Draw3Play2OfThem:
                case EnumActionMain.Draw2AndUseEm:
                    LoadDrawUse();
                    break;
                default:
                    throw new BasicBlankException("Don't know what to do for status");
            }
        }
        void IAction.Init()
        {
            _mainGame.ThisGlobal!.CurrentAction = new ActionCard();
            ActionDetail = new DetailCardViewModel();
            CurrentDetail = new DetailCardViewModel();
            YourKeepers = new HandViewModel<KeeperCard>(_thisMod);
            YourKeepers.AutoSelect = HandViewModel<KeeperCard>.EnumAutoType.ShowObjectOnly;
            YourKeepers.ConsiderSelectOneAsync += YourKeepers_ConsiderSelectOneAsync;
            YourKeepers.Text = "Your Keepers";
            YourKeepers.Visible = true;
            YourKeepers.SendAlwaysEnable(_thisMod);
            PrivateGoals = new HandViewModel<GoalCard>(_thisMod);
            PrivateGoals.ConsiderSelectOneAsync += PrivateGoals_ConsiderSelectOneAsync;
            PrivateGoals.AutoSelect = HandViewModel<GoalCard>.EnumAutoType.ShowObjectOnly;
            PrivateGoals.Text = "Goal Cards";
            PrivateGoals.Maximum = 3;
            PrivateGoals.Visible = true;
            PrivateGoals.HandList = _thisMod.Goal1!.HandList; //this could require linking up for fluxx upon autoresume.
            YourCards = new HandViewModel<FluxxCardInformation>(_thisMod);
            YourCards.Text = "Your Cards";
            YourCards.SendAlwaysEnable(_thisMod);
            YourCards.ConsiderSelectOneAsync += YourCards_ConsiderSelectOneAsync;
            YourCards.AutoSelect = HandViewModel<FluxxCardInformation>.EnumAutoType.ShowObjectOnly;
            YourCards.Visible = true;
            OtherHand = new HandViewModel<FluxxCardInformation>(_thisMod);
            OtherHand.AutoSelect = HandViewModel<FluxxCardInformation>.EnumAutoType.SelectOneOnly;
            OtherHand.Text = "Other Player's Cards";
            OtherHand.ConsiderSelectOneAsync += YourCards_ConsiderSelectOneAsync;
            OtherHand.SendEnableProcesses(_thisMod, () =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                    return false;
                if (OtherHand.Visible == false)
                    return false;
                return ActionCategory == EnumActionCategory.FirstRandom || ActionCategory == EnumActionCategory.UseTake;
            });
            TempHand = new HandViewModel<FluxxCardInformation>(_thisMod);
            TempHand.AutoSelect = HandViewModel<FluxxCardInformation>.EnumAutoType.SelectOneOnly;
            TempHand.Text = "Temporary Cards";
            TempHand.ConsiderSelectOneAsync += YourCards_ConsiderSelectOneAsync;
            TempHand.SendEnableProcesses(_thisMod, () =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                    return false;
                return ActionCategory == EnumActionCategory.Everybody1 || ActionCategory == EnumActionCategory.DrawUse;
            });
            Direction1 = new ListViewPicker(_thisMod);
            Direction1.ItemSelectedAsync += Direction1_ItemSelectedAsync;
            Direction1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            Direction1.LoadTextList(new CustomBasicList<string> { "Left", "Right" });
            Direction1.Visible = true; //this could be it too.
            Direction1.SendEnableProcesses(_thisMod, () =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                    return false;
                return ActionCategory == EnumActionCategory.Directions;
            });
            Rule1 = new ListViewPicker(_thisMod);
            Rule1.ItemSelectedAsync += Rule1_ItemSelectedAsync;
            Rule1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            Rule1.Visible = true;
            Rule1.SendEnableProcesses(_thisMod, () =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                    return false;
                return ActionCategory == EnumActionCategory.Rules;
            });
            Player1 = new ListViewPicker(_thisMod);
            Player1.ItemSelectedAsync += Player1_ItemSelectedAsync;
            Player1.Visible = true;
            Player1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            Player1.SendEnableProcesses(_thisMod, () => CanEnableChoosePlayer());
            CardList1 = new ListViewPicker(_thisMod);
            CardList1.ItemSelectedAsync += CardList1_ItemSelectedAsync;
            CardList1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            CardList1.Visible = true;
            CardList1.SendEnableProcesses(_thisMod, () =>
            {
                if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                    return false;
                return ActionCategory == EnumActionCategory.DoAgain;
            });
            LoadOtherCommands();
        }
        private Task CardList1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            IndexCard = selectedIndex;
            return Task.CompletedTask;
        }
        private Task Player1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            IndexPlayer = selectedIndex;
            return Task.CompletedTask;
        }
        private Task Rule1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            if (_mainGame.ThisGlobal!.CurrentAction!.Deck == EnumActionMain.TrashANewRule)
                IndexRule = selectedIndex;
            else
                TempRuleList = Rule1!.GetAllSelectedItems();
            return Task.CompletedTask;
        }
        private Task Direction1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            IndexDirection = selectedIndex;
            return Task.CompletedTask;
        }
        private Task YourCards_ConsiderSelectOneAsync(FluxxCardInformation thisObject)
        {
            ShowCard(thisObject);
            return Task.CompletedTask;
        }
        private Task PrivateGoals_ConsiderSelectOneAsync(GoalCard thisObject)
        {
            ShowCard(thisObject);
            return Task.CompletedTask;
        }
        private Task YourKeepers_ConsiderSelectOneAsync(KeeperCard thisObject)
        {
            ShowCard(thisObject);
            return Task.CompletedTask;
        }
        public void SetUpGoals()
        {
            _mainGame!.SaveRoot!.SavedActionData.SelectedIndex = -1;
            IndexPlayer = -1;
            Player1!.UnselectAll();
        }
        void IAction.SetUpFrames() //i think this is needed even upon autoresume
        {
            if (_mainGame.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Needs to be set to self first");
            YourCards!.HandList = _mainGame.SingleInfo.MainHandList;
            YourKeepers!.HandList = _mainGame.SingleInfo.KeeperList;
            PrivateGoals!.HandList = _mainGame.SaveRoot!.GoalList; //i think this should be done too.
        }
        async Task IAction.ChoseOtherCardSelectedAsync(int deck)
        {
            OtherHand!.SelectOneFromDeck(deck);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            ShowMainScreenAgain();
        }
        public int GetPlayerIndex(int selectedIndex)
        {
            return _tempList[selectedIndex];
        }
        async Task IAction.ShowRulesSimplifiedAsync(CustomBasicList<int> tempList)
        {
            if (tempList.Count == 0)
                return;
            Rule1!.SelectSeveralItems(tempList);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(1);
            ShowMainScreenAgain();
        }
        async Task IAction.ShowRuleTrashedAsync(int selectedIndex)
        {
            Rule1!.SelectSpecificItem(selectedIndex);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.75);
            ShowMainScreenAgain();
        }
        public FluxxCardInformation GetCardToDoAgain(int selectedIndex)
        {
            var tempList = _mainGame.DeckList.Where(items => items.CardType == EnumCardType.Rule || items.CardType == EnumCardType.Action).ToRegularDeckDict();
            var thisText = CardList1!.GetText(selectedIndex);
            return tempList.Single(items => items.Text() == thisText);
        }
        async Task IAction.ShowLetsDoAgainAsync(int selectedIndex)
        {
            CardList1!.SelectSpecificItem(selectedIndex);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            ShowMainScreenAgain();
        }
        async Task IAction.ShowDirectionAsync(int selectedIndex)
        {
            Direction1!.SelectSpecificItem(selectedIndex);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            ShowMainScreenAgain();
        }
        public async Task ShowTradeHandAsync(int selectedIndex)
        {
            Player1!.SelectSpecificItem(selectedIndex);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.75);
            ShowMainScreenAgain();
        }
        async Task IAction.ShowPlayerForCardChosenAsync(int selectedIndex)
        {
            IndexPlayer = selectedIndex;
            await ShowTradeHandAsync(selectedIndex);
        }
        async Task IAction.ShowChosenForEverybodyGetsOneAsync(CustomBasicList<int> selectedList, int selectedIndex)
        {
            selectedList.ForEach(thisItem =>
            {
                TempHand!.SelectOneFromDeck(thisItem);
            });
            Player1!.SelectSpecificItem(selectedIndex);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(1);
            ShowMainScreenAgain();
        }
        bool IAction.CanLoadEverybodyGetsOne()
        {
            var temps = PlayersLeftForEverybodyGetsOne();
            if (temps.Count == 0)
                throw new BasicBlankException("Needs to have at least one player in order to figure out if everybody gets one");
            return temps.Count > 1;
        }
        async Task IAction.ShowCardUseAsync(int deck)
        {
            TempHand!.SelectOneFromDeck(deck);
            if (_mainGame.ThisTest!.NoAnimations == false)
                await _mainGame.Delay!.DelaySeconds(.5);
            ShowMainScreenAgain();
        }
        CustomBasicList<int> IAction.GetTempPlayerList()
        {
            return Enumerable.Range(0, Player1!.Count()).ToCustomBasicList();
        }
        CustomBasicList<int> IAction.GetTempRuleList()
        {
            return Enumerable.Range(0, Rule1!.Count()).ToCustomBasicList();
        }
        CustomBasicList<int> IAction.GetTempCardList()
        {
            return Enumerable.Range(0, CardList1!.Count()).ToCustomBasicList();
        }
        int IAction.MaxRulesToDiscard()
        {
            return RulesToDiscard;
        }
        void IAction.ResetPlayers()
        {
            SetUpGoals();
        }
        void IAction.VisibleChange()
        {
            OnPropertyChanged(nameof(EntireVisible));
        }
    }
}