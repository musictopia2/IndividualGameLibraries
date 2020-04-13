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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.Messenging;
using FluxxCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using FluxxCP.Data;
using FluxxCP.Cards;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.NetworkingClasses.Extensions;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using FluxxCP.Containers;
//i think this is the most common things i like to do
namespace FluxxCP.ViewModels
{
    [InstanceGame]
    public class FluxxMainViewModel : BasicCardGamesVM<FluxxCardInformation>
    {
        private readonly FluxxMainGameClass _mainGame; //if we don't need, delete.
        private readonly FluxxVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly FluxxGameContainer _gameContainer; //if not needed, delete.
        private readonly IDiscardProcesses _discardProcesses;
        private readonly IAnalyzeProcesses _analyzeQueProcesses;
        private readonly KeeperContainer _keeperContainer;
        private readonly FluxxDelegates _delegates;

        public FluxxMainViewModel(CommandContainer commandContainer,
            FluxxMainGameClass mainGame,
            FluxxVMData viewModel, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            FluxxGameContainer gameContainer,
            IDiscardProcesses discardProcesses,
            IAnalyzeProcesses analyzeQueProcesses,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _resolver = resolver;
            _gameContainer = gameContainer;
            _discardProcesses = discardProcesses;
            _analyzeQueProcesses = analyzeQueProcesses;
            _keeperContainer = keeperContainer;
            _delegates = delegates;
            _model.Deck1.NeverAutoDisable = true;
            _gameContainer.LoadGiveAsync = LoadGiveAsync;
            _gameContainer.LoadPlayAsync = LoadPlayAsync;
            _model.Keeper1.ConsiderSelectOneAsync += OnConsiderSelectOneCardAsync;
            _model.Goal1.ConsiderSelectOneAsync += OnConsiderSelectOneCardAsync;
        }
        protected override Task TryCloseAsync()
        {
            _model.Keeper1.ConsiderSelectOneAsync -= OnConsiderSelectOneCardAsync;
            _model.Goal1.ConsiderSelectOneAsync -= OnConsiderSelectOneCardAsync;
            return base.TryCloseAsync();
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            if (_gameContainer!.CurrentAction != null && _gameContainer.CurrentAction.Deck == EnumActionMain.Taxation)
            {
                await LoadGiveAsync();
            }
            else
            {
                await LoadPlayAsync();
            }
        }
        private async Task ClosePlayGiveAsync()
        {
            if (PlayGiveScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(PlayGiveScreen);
            PlayGiveScreen = null;
        }
        private async Task LoadPlayAsync()
        {
            await ClosePlayGiveAsync();
            PlayGiveScreen = _resolver.Resolve<PlayViewModel>();
            await LoadScreenAsync(PlayGiveScreen);
        }
        private async Task LoadGiveAsync()
        {
            await ClosePlayGiveAsync();
            PlayGiveScreen = _resolver.Resolve<GiveViewModel>();
            await LoadScreenAsync(PlayGiveScreen);
        }

        public IScreen? PlayGiveScreen { get; set; }

        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false;
        }

        protected override bool CanEnablePile1()
        {
            return false;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }


        private int _playsLeft;
        [VM]
        public int PlaysLeft
        {
            get
            {
                return _playsLeft;
            }

            set
            {
                if (SetProperty(ref _playsLeft, value) == true)
                {
                }
            }
        }
        private int _handLimit;
        [VM]
        public int HandLimit
        {
            get
            {
                return _handLimit;
            }

            set
            {
                if (SetProperty(ref _handLimit, value) == true)
                {
                }
            }
        }
        private int _keeperLimit;
        [VM]
        public int KeeperLimit
        {
            get
            {
                return _keeperLimit;
            }

            set
            {
                if (SetProperty(ref _keeperLimit, value) == true)
                {
                }
            }
        }
        private int _playLimit;
        [VM]
        public int PlayLimit
        {
            get
            {
                return _playLimit;
            }

            set
            {
                if (SetProperty(ref _playLimit, value) == true)
                {
                }
            }
        }
        private bool _anotherTurn; // i think that otherturn is already built in.
        [VM]
        public bool AnotherTurn
        {
            get
            {
                return _anotherTurn;
            }

            set
            {
                if (SetProperty(ref _anotherTurn, value) == true)
                {
                }
            }
        }
        private int _drawBonus;
        [VM]
        public int DrawBonus
        {
            get
            {
                return _drawBonus;
            }

            set
            {
                if (SetProperty(ref _drawBonus, value) == true)
                {
                }
            }
        }
        private int _playBonus;
        [VM]
        public int PlayBonus
        {
            get
            {
                return _playBonus;
            }

            set
            {
                if (SetProperty(ref _playBonus, value) == true)
                {
                }
            }
        }
        private int _cardsDrawn;
        [VM]
        public int CardsDrawn
        {
            get
            {
                return _cardsDrawn;
            }

            set
            {
                if (SetProperty(ref _cardsDrawn, value) == true)
                {
                }
            }
        }
        private int _drawRules;
        [VM]
        public int DrawRules
        {
            get
            {
                return _drawRules;
            }

            set
            {
                if (SetProperty(ref _drawRules, value) == true)
                {
                }
            }
        }
        private int _previousBonus;
        [VM]
        public int PreviousBonus
        {
            get
            {
                return _previousBonus;
            }

            set
            {
                if (SetProperty(ref _previousBonus, value) == true)
                {
                }
            }
        }
        private int _cardsPlayed;
        [VM]
        public int CardsPlayed
        {
            get
            {
                return _cardsPlayed;
            }

            set
            {
                if (SetProperty(ref _cardsPlayed, value) == true)
                {
                }
            }
        }

        private string _otherTurn = "";
        [VM]
        public string OtherTurn
        {
            get { return _otherTurn; }
            set
            {
                if (SetProperty(ref _otherTurn, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public override bool CanEndTurn()
        {
            //if (FluxxScreenUsed == EnumActionScreen.ActionScreen || FluxxScreenUsed == EnumActionScreen.KeeperScreen)
            //    return false;
            return _mainGame.OtherTurn == 0;
        }
        public override async Task EndTurnAsync()
        {
            var ends = _mainGame!.StatusEndRegularTurn();
            if (ends == EnumEndTurnStatus.Goal)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you must get rid of excess goals");
                return;
            }
            if (ends == EnumEndTurnStatus.Play)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you have plays remaining");
                return;
            }
            if (ends == EnumEndTurnStatus.Hand)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you have too many cards in your hand");
                return;
            }
            if (ends == EnumEndTurnStatus.Keeper)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you have too many keepers");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendEndTurnAsync();
            await _mainGame.EndTurnAsync();
        }
        protected override Task OnConsiderSelectOneCardAsync(FluxxCardInformation thisObject)
        {
            if (thisObject.Deck == _model.CardDetail!.CurrentCard.Deck)
                _model.CardDetail.ResetCard();
            else
                _model.CardDetail.ShowCard(thisObject);
            return Task.CompletedTask;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task DiscardAsync()
        {
            int goalDiscarded = _model.Goal1!.ObjectSelected();
            int keepers = _model.Keeper1!.HowManySelectedObjects;
            int yours = _model.PlayerHand1!.HowManySelectedObjects;
            if (goalDiscarded == 0 && keepers == 0 && yours == 0)
            {
                await UIPlatform.ShowMessageAsync("There is no cards selected to discard");
                return;
            }
            CustomBasicList<int> tempList = new CustomBasicList<int> { keepers, goalDiscarded, yours };
            if (tempList.Count(item => item > 0) > 1)
            {
                await UIPlatform.ShowMessageAsync("Can choose a goal, keepers, or from your hand; not from more than one");
                return;
            }
            if (goalDiscarded > 0)
            {
                if (_gameContainer!.NeedsToRemoveGoal() == false)
                {
                    await UIPlatform.ShowMessageAsync("Cannot discard any goals");
                    _model.UnselectAllCards();
                    return;
                }
                if (goalDiscarded == (int)_mainGame.SaveRoot!.GoalList.Last().Deck && _mainGame.SaveRoot.GoalList.Count == 3)
                {
                    await UIPlatform.ShowMessageAsync("Cannot discard the third goal on the list.  Must choose one of the 2 that was there before");
                    _model.UnselectAllCards();
                    return;
                }
                await _discardProcesses.DiscardGoalAsync(goalDiscarded);
                await _analyzeQueProcesses.AnalyzeQueAsync();
                return;
            }
            if (_gameContainer!.NeedsToRemoveGoal())
            {
                await UIPlatform.ShowMessageAsync("Must discard a goal before discarding anything else");
                _model.UnselectAllCards();
                return;
            }
            if (yours > 0)
            {
                if (_mainGame.SaveRoot!.HandLimit == -1)
                {
                    await UIPlatform.ShowMessageAsync("There is no hand limit.  Therefore cannot discard any cards from your hand");
                    _model.UnselectAllCards();
                    return;
                }
                int newCount = _mainGame.SingleInfo!.MainHandList.Count - yours;
                if (newCount < _mainGame.SaveRoot.HandLimit)
                {
                    await UIPlatform.ShowMessageAsync($"Cannot discard from hand {yours} cards because that will cause you to discard too many cards");
                    _model.UnselectAllCards();
                    return;
                }
                var firstList = _model.PlayerHand1.ListSelectedObjects();
                await _discardProcesses.DiscardFromHandAsync(firstList);
                return;
            }
            if (keepers > 0)
            {
                if (_mainGame.SaveRoot!.KeeperLimit == -1)
                {
                    await UIPlatform.ShowMessageAsync("There is no keeper limit.  Therefore; cannot discard any keepers");
                    _model.UnselectAllCards();
                    return;
                }
                int newCount = _mainGame.SingleInfo!.KeeperList.Count - keepers;
                if (newCount < _mainGame.SaveRoot.KeeperLimit)
                {
                    await UIPlatform.ShowMessageAsync($"Cannot discard from keepers {keepers} cards because that will cause you to discard too many keepers");
                    _model.UnselectAllCards();
                    return;
                }
                var firstList = _model.Keeper1.ListSelectedObjects();
                DeckRegularDict<FluxxCardInformation> finList = new DeckRegularDict<FluxxCardInformation>(firstList);
                await _discardProcesses.DiscardKeepersAsync(finList);
                return;
            }
            throw new BasicBlankException("Don't know how to discard from here");
        }

        [Command(EnumCommandCategory.Game)]
        public void SelectHandCards()
        {
            _model.PlayerHand1.SelectAllObjects();
        }
        [Command(EnumCommandCategory.Game)]
        public void UnselectHandCards()
        {
            _model.PlayerHand1.UnselectAllObjects();
        }

        [Command(EnumCommandCategory.Game)]
        public async Task ShowKeepersAsync()
        {
            _keeperContainer.ShowKeepers();
            if (_delegates.LoadKeeperScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the keeper screen when main needs to load it");
            }
            await _delegates.LoadKeeperScreenAsync.Invoke(_keeperContainer);
        }

        //private bool IsOtherTurn => _mainGame!.OtherTurn > 0;
        
    }
}