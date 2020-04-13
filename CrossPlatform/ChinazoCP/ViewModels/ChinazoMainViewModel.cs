using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using ChinazoCP.Data;
using ChinazoCP.Logic;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
namespace ChinazoCP.ViewModels
{
    [InstanceGame]
    public class ChinazoMainViewModel : BasicCardGamesVM<ChinazoCard>
    {
        private readonly ChinazoMainGameClass _mainGame; //if we don't need, delete.
        private readonly ChinazoVMData _model;
        private readonly ChinazoGameContainer _gameContainer;

        public ChinazoMainViewModel(CommandContainer commandContainer,
            ChinazoMainGameClass mainGame,
            ChinazoVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            ChinazoGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _model.PlayerHand1.AutoSelect = HandObservable<ChinazoCard>.EnumAutoType.SelectAsMany;
            var player = _mainGame.PlayerList.GetSelf();
            mainGame.Aggregator.Subscribe(player); //hopefully this works now.


            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync += MainSets_SetClickedAsync;
            _model.MainSets.SendEnableProcesses(this, () =>
            {
                if (_mainGame!.OtherTurn > 0)
                    return false;
                return _mainGame.SingleInfo!.LaidDown;
            });
        }
        protected override Task TryCloseAsync()
        {
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            _model.MainSets.SetClickedAsync -= MainSets_SetClickedAsync;
            return base.TryCloseAsync();
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return _mainGame!.OtherTurn == 0;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int counts = _model.PlayerHand1!.HowManySelectedObjects;
            int others = _model.TempSets!.HowManySelectedObjects;
            if (counts + others > 1)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you can only select one card to discard");
                return;
            }
            if (counts + others == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, you must select a card to discard");
                return;
            }
            int index;
            int deck;
            if (counts == 0)
            {
                index = _model.TempSets.PileForSelectedObject;
                deck = _model.TempSets.DeckForSelectedObjected(index);
            }
            else
            {
                deck = _model.PlayerHand1.ObjectSelected();
            }
            await _gameContainer!.SendDiscardMessageAsync(deck);
            await _mainGame.DiscardAsync(deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private string _phaseData = "";
        [VM]
        public string PhaseData
        {
            get { return _phaseData; }
            set
            {
                if (SetProperty(ref _phaseData, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _otherLabel = "";
        [VM]
        public string OtherLabel
        {
            get { return _otherLabel; }
            set
            {
                if (SetProperty(ref _otherLabel, value))
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
            var thisSet = _model.MainSets!.GetIndividualSet(setNumber);
            bool rets;
            rets = _mainGame!.CanAddToSet(thisSet, out ChinazoCard? thisCard, section, out string message);
            if (rets == false)
            {
                if (message != "")
                    await UIPlatform.ShowMessageAsync(message);
                return;
            }
            int nums = setNumber;
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                SendExpandedSet thiss = new SendExpandedSet();
                thiss.Deck = thisCard!.Deck;
                thiss.Number = nums;
                thiss.Position = section;
                await _mainGame.Network!.SendAllAsync("expandrummy", thiss);
            }
            await _mainGame.AddToSetAsync(nums, thisCard!.Deck, section);
        }
        public bool CanPass => !CanEnablePile1();
        [Command(EnumCommandCategory.Game)]
        public async Task PassAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("pass");
            await _mainGame!.PassAsync();
        }
        public bool CanTake => !CanEnablePile1();
        [Command(EnumCommandCategory.Game)]
        public async Task TakeAsync()
        {
            await _mainGame!.PickupFromDiscardAsync();
        }
        public bool CanFirstSets
        {
            get
            {
                if (_mainGame!.OtherTurn > 0)
                    return false;
                return !_mainGame.SingleInfo!.LaidDown;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task FirstSetsAsync()
        {
            if (_mainGame!.CanLayDownInitialSets() == false)
            {
                await UIPlatform.ShowMessageAsync("Sorry; you do not have the required sets yet");
                return;
            }
            var thisCol = _mainGame.ListValidSets();
            CustomBasicList<string> newList = new CustomBasicList<string>();
            await thisCol.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    SendNewSet thiss = new SendNewSet();
                    thiss.CardListData = await js.SerializeObjectAsync(thisTemp.CardList.GetDeckListFromObjectList());
                    thiss.UseSecond = thisTemp.UseSecond;
                    thiss.WhatSet = thisTemp.WhatSet;
                    var thisStr = await js.SerializeObjectAsync(thiss);
                    newList.Add(thisStr);
                }
                _mainGame.CreateNewSet(thisTemp);
            });
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
            await _mainGame!.LaidDownInitialSetsAsync();
        }
    }
}