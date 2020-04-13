using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.Extensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FluxxCP.Cards;
using FluxxCP.Data;
using FluxxCP.UICP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxxCP.Containers
{
    [SingletonGame]
    [AutoReset]
    public class ActionContainer : ObservableObject, IEnableAlways, IBasicEnableProcess
    {
        private readonly CommandContainer _command;

        //this may have delegates so can still keep separate.
        //all don't have to use all.
        //Task ShowCardUseAsync(int deck);

        //instead of init, just do here.  hopefully i don't regret.
        private readonly FluxxGameContainer _gameContainer;

        public ActionContainer(CommandContainer command, IGamePackageResolver resolver, FluxxGameContainer gameContainer, FluxxDelegates delegates)
        {
            delegates.RefreshEnables = RefreshEnables;
            _command = command;
            _gameContainer = gameContainer;
            ActionDetail = new DetailCardObservable();
            CurrentDetail = new DetailCardObservable();
            YourKeepers = new HandObservable<KeeperCard>(command);
            YourKeepers.AutoSelect = HandObservable<KeeperCard>.EnumAutoType.ShowObjectOnly;
            YourKeepers.Text = "Your Keepers";
            YourKeepers.SendAlwaysEnable(this);
            YourKeepers.ConsiderSelectOneAsync += YourKeepers_ConsiderSelectOneAsync;
            PrivateGoals = new HandObservable<GoalCard>(command);
            PrivateGoals.AutoSelect = HandObservable<GoalCard>.EnumAutoType.ShowObjectOnly;
            PrivateGoals.Text = "Goal Cards";
            PrivateGoals.Maximum = 3;
            PrivateGoals.ConsiderSelectOneAsync += PrivateGoals_ConsiderSelectOneAsync;
            //handlist hook up when necessary.
            YourCards = new HandObservable<FluxxCardInformation>(command);
            YourCards.Text = "Your Cards";
            YourCards.ConsiderSelectOneAsync += YourCards_ConsiderSelectOneAsync;
            YourCards.AutoSelect = HandObservable<FluxxCardInformation>.EnumAutoType.ShowObjectOnly;
            OtherHand = new HandObservable<FluxxCardInformation>(command);
            OtherHand.AutoSelect = HandObservable<FluxxCardInformation>.EnumAutoType.SelectOneOnly;
            OtherHand.Text = "Other Player's Cards";
            OtherHand.ConsiderSelectOneAsync += YourCards_ConsiderSelectOneAsync;
            OtherHand.SendEnableProcesses(this, () =>
            {
                //if (_thisMod.FluxxScreenUsed != EnumActionScreen.ActionScreen)
                //    return false;
                if (OtherHand.Visible == false)
                    return false;
                return ActionCategory == EnumActionCategory.FirstRandom || ActionCategory == EnumActionCategory.UseTake;
            });

            TempHand = new HandObservable<FluxxCardInformation>(command);
            TempHand.AutoSelect = HandObservable<FluxxCardInformation>.EnumAutoType.SelectOneOnly;
            TempHand.Text = "Temporary Cards";
            TempHand.ConsiderSelectOneAsync += YourCards_ConsiderSelectOneAsync;
            TempHand.SendEnableProcesses(this, () =>
            {
                return ActionCategory == EnumActionCategory.Everybody1 || ActionCategory == EnumActionCategory.DrawUse;
            });
            Direction1 = new ListViewPicker(command, resolver);
            Direction1.ItemSelectedAsync += Direction1_ItemSelectedAsync;
            Direction1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            Direction1.LoadTextList(new CustomBasicList<string> { "Left", "Right" });
            Rule1 = new ListViewPicker(command, resolver);
            Rule1.ItemSelectedAsync += Rule1_ItemSelectedAsync;
            Rule1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            Rule1.SendEnableProcesses(this, () =>
            {
                return ActionCategory == EnumActionCategory.Rules;
            });
            Player1 = new ListViewPicker(command, resolver);
            Player1.ItemSelectedAsync += Player1_ItemSelectedAsync;
            Player1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            Player1.SendEnableProcesses(this, () => CanEnableChoosePlayer());

            CardList1 = new ListViewPicker(command, resolver);
            CardList1.ItemSelectedAsync += CardList1_ItemSelectedAsync;
            CardList1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
            CardList1.SendEnableProcesses(this, () =>
            {
                return ActionCategory == EnumActionCategory.DoAgain;
            });


        }
        internal CustomBasicList<int> PlayerIndexList { get; set; } = new CustomBasicList<int>();
        internal int Loads { get; set; } //i think.  not sure (?) needed to be this way so could be extended.
        //internal Func<int, Task>? ShowCardUseAsync { get; set; }
        //doing this way means i don't have to worry how the interfaces are going to lay out.

        public DetailCardObservable ActionDetail;
        public DetailCardObservable CurrentDetail;

        public ListViewPicker Direction1;
        public ListViewPicker Rule1;
        public ListViewPicker Player1;
        public ListViewPicker CardList1;
        public CustomBasicList<int>? TempRuleList = new CustomBasicList<int>();

        public HandObservable<GoalCard> PrivateGoals;
        public HandObservable<FluxxCardInformation> YourCards;
        public HandObservable<KeeperCard> YourKeepers;
        public HandObservable<FluxxCardInformation> OtherHand;
        public HandObservable<FluxxCardInformation> TempHand;

        public int GetPlayerIndex(int selectedIndex)
        {
            return PlayerIndexList[selectedIndex];
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

        private Task Direction1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            IndexDirection = selectedIndex;
            return Task.CompletedTask;
        }
        private Task Rule1_ItemSelectedAsync(int selectedIndex, string selectedText)
        {
            if (_gameContainer.CurrentAction!.Deck == EnumActionMain.TrashANewRule)
                IndexRule = selectedIndex;
            else
                TempRuleList = Rule1!.GetAllSelectedItems();
            return Task.CompletedTask;
        }

        public void ShowCard(FluxxCardInformation card)
        {
            if (card.IsUnknown == true)
                return;
            if (card.Deck == CurrentDetail!.CurrentCard.Deck)
                CurrentDetail.ResetCard();
            else
                CurrentDetail.ShowCard(card);
        }

        private EnumActionCategory _actionCategory;
        public EnumActionCategory ActionCategory
        {
            get { return _actionCategory; }
            set
            {
                if (SetProperty(ref _actionCategory, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _rulesToDiscard;
        public int RulesToDiscard
        {
            get { return _rulesToDiscard; }
            set
            {
                if (SetProperty(ref _rulesToDiscard, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        #region "Filled Properties
        private int _indexPlayer = -1;
        public int IndexPlayer
        {
            get { return _indexPlayer; }
            set
            {
                if (SetProperty(ref _indexPlayer, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _indexDirection = -1;
        public int IndexDirection
        {
            get { return _indexDirection; }
            set
            {
                if (SetProperty(ref _indexDirection, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _indexRule = -1;
        public int IndexRule
        {
            get { return _indexRule; }
            set
            {
                if (SetProperty(ref _indexRule, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _indexCard;
        public int IndexCard
        {
            get { return _indexCard; }
            set
            {
                if (SetProperty(ref _indexCard, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _actionFrameText = "Action Card Information";

        public string ActionFrameText
        {
            get { return _actionFrameText; }
            set
            {
                if (SetProperty(ref _actionFrameText, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        //we may need the buttonchooseplayervisible after all.  otherwise, gets too complex on the screens.
        private bool _buttonChoosePlayerVisible;
        public bool ButtonChoosePlayerVisible
        {
            get { return _buttonChoosePlayerVisible; }
            set
            {
                if (SetProperty(ref _buttonChoosePlayerVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _buttonChooseCardVisible; //even this is now needed unfortunately.
        public bool ButtonChooseCardVisible
        {
            get { return _buttonChooseCardVisible; }
            set
            {
                if (SetProperty(ref _buttonChooseCardVisible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        #endregion

        bool IEnableAlways.CanEnableAlways()
        {
            return true;
        }

        bool IBasicEnableProcess.CanEnableBasics()
        {
            return true;
        }

        public void SetUpGoals()
        {
            _gameContainer!.SaveRoot!.SavedActionData.SelectedIndex = -1;
            IndexPlayer = -1;
            Player1!.UnselectAll();
        }
        public void SetUpFrames()
        {
            if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Needs to be set to self first");
            YourCards!.HandList = _gameContainer.SingleInfo.MainHandList;
            YourKeepers!.HandList = _gameContainer.SingleInfo.KeeperList;
            PrivateGoals!.HandList = _gameContainer.SaveRoot!.GoalList; //i think this should be done too.
        }
        public FluxxCardInformation GetCardToDoAgain(int selectedIndex)
        {
            var tempList = _gameContainer.DeckList.Where(items => items.CardType == EnumCardType.Rule || items.CardType == EnumCardType.Action).ToRegularDeckDict();
            var thisText = CardList1!.GetText(selectedIndex);
            return tempList.Single(items => items.Text() == thisText);
        }


        private bool CanEnableChoosePlayer()
        {
            if (ActionCategory == EnumActionCategory.TradeHands || ActionCategory == EnumActionCategory.UseTake || ActionCategory == EnumActionCategory.Everybody1)
            {
                if (ActionCategory == EnumActionCategory.UseTake && ButtonChoosePlayerVisible == false)
                    return false;
                if (_gameContainer.CurrentAction == null)
                {
                    return false;
                }
                var thisC = _gameContainer.CurrentAction!.Deck;
                if (thisC == EnumActionMain.UseWhatYouTake && _gameContainer.SaveRoot!.SavedActionData.SelectedIndex == -1 || thisC != EnumActionMain.UseWhatYouTake)
                    return true;
            }
            return false;
        }

        public async Task DoAgainProcessPart1Async(int selectedIndex)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
            {
                await _gameContainer.Network!.SendAllAsync("doagain", selectedIndex);
            }
        }
        public async Task ChosePlayerOnActionAsync(int selectedIndex)
        {
            if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
                await _gameContainer.Network!.SendAllAsync("choseplayerforcardchosen", selectedIndex);
        }

        public bool CanChoosePlayer()
        {
            if (ButtonChoosePlayerVisible == false || IndexPlayer == -1)
                return false;
            return CanEnableChoosePlayer();
        }
        public void RefreshEnables()
        {
            TempHand.ReportCanExecuteChange();
            OtherHand.ReportCanExecuteChange();
            YourKeepers.ReportCanExecuteChange();
            Rule1.ReportCanExecuteChange();
            _command.ManualReport(); //try this.
        }
    }
}
