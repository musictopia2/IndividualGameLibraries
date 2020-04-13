using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using Phase10CP.Cards;
using Phase10CP.Data;
using Phase10CP.Logic;
using Phase10CP.SetClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace Phase10CP.ViewModels
{
    [InstanceGame]
    public class Phase10MainViewModel : BasicCardGamesVM<Phase10CardInformation>
    {
        private readonly Phase10MainGameClass _mainGame; //if we don't need, delete.
        private readonly Phase10VMData _model;
        private readonly Phase10GameContainer _gameContainer; //if not needed, delete.

        public Phase10MainViewModel(CommandContainer commandContainer,
            Phase10MainGameClass mainGame,
            Phase10VMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            Phase10GameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _model.PlayerHand1.AutoSelect = HandObservable<Phase10CardInformation>.EnumAutoType.SelectAsMany;
            var player = _mainGame.PlayerList.GetSelf();
            mainGame.Aggregator.Subscribe(player); //hopefully this works now.
            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync += MainSets_SetClickedAsync;
            _model.MainSets.SendEnableProcesses(this, () => _gameContainer!.AlreadyDrew);
        }
        //anything else needed is here.
        protected override Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync -= MainSets_SetClickedAsync;
            return base.TryCloseAsync();
        }
        protected override bool CanEnableDeck()
        {
            return !_gameContainer!.AlreadyDrew;
        }

        protected override bool CanEnablePile1()
        {
            return true;
        }
        public bool CanCompletePhase()
        {
            if (_gameContainer!.AlreadyDrew == false)
                return false;
            return !_mainGame.SaveRoot!.CompletedPhase;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task CompletePhaseAsync()
        {
            bool rets;
            rets = _mainGame!.DidCompletePhase(out int Manys);
            if (Manys == _model.TempSets!.TotalObjects + _mainGame.SingleInfo!.MainHandList.Count)
            {
                await UIPlatform.ShowMessageAsync("Cannot complete the phase.  Otherwise; there is no card to discard");
                return;
            }
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you did not complete the phase");
                return;
            }
            var thisCol = _mainGame.ListValidSets();
            if (thisCol.Count > 2)
                throw new BasicBlankException("Can not have more than 2 sets");
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisCol.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    SendNewSet thisSend = new SendNewSet();
                    thisSend.CardListData = await js.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                    thisSend.WhatSet = thisTemp.WhatSet;
                    string newStr = await js.SerializeObjectAsync(thisSend);
                    newList.Add(newStr);
                }
                _mainGame.CreateNewSet(thisTemp);
            });
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendSeveralSetsAsync(newList, "phasecompleted");
            await _mainGame.ProcessCompletedPhaseAsync();
        }
        protected override async Task ProcessDiscardClickedAsync()
        {
            bool rets = _mainGame!.CanProcessDiscard(out bool pickUp, out _, out int deck, out string message);
            if (rets == false)
            {
                if (message != "")
                {
                    _model.PlayerHand1!.UnselectAllObjects();
                    _model.TempSets!.UnselectAllCards(); //its best to just unselect everything if you can't discard.  that will solve some issues.
                    await UIPlatform.ShowMessageAsync(message); //because on tablets, its possible it can't show message

                }
                return;
            }
            if (pickUp == true)
            {
                await _mainGame.PickupFromDiscardAsync();
                return;
            }
            await _gameContainer.SendDiscardMessageAsync(deck);
            await _mainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private string _currentPhase = "";
        [VM]
        public string CurrentPhase
        {
            get { return _currentPhase; }
            set
            {
                if (SetProperty(ref _currentPhase, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
            _model.TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }

        private async Task MainSets_SetClickedAsync(int setNumber, int section, int deck)
        {
            if (setNumber == 0)
                return;
            //has to wait to do more of main before doing this.
            if (_mainGame!.SaveRoot!.CompletedPhase == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry, the phase must be completed before expanding onto a set");
                return;
            }
            var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
            int position = section;
            bool rets;
            rets = _mainGame.CanHumanExpand(thisSet, ref position, out Phase10CardInformation? thisCard, out string message);
            if (rets == false)
            {
                if (message != "")
                    await UIPlatform.ShowMessageAsync(message);
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendExpandedSet expands = new SendExpandedSet();
                expands.Deck = thisCard!.Deck;
                expands.Position = position;
                expands.Number = setNumber;
                await _mainGame.Network!.SendAllAsync("expandrummy", expands);
            }
            await _mainGame.ExpandHumanRummyAsync(setNumber, thisCard!.Deck, position);
        }

    }
}