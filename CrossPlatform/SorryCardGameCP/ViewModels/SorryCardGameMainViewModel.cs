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
using SorryCardGameCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using SorryCardGameCP.Data;
using SorryCardGameCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace SorryCardGameCP.ViewModels
{
    [InstanceGame]
    public class SorryCardGameMainViewModel : BasicCardGamesVM<SorryCardGameCardInformation>
    {
        private readonly SorryCardGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly SorryCardGameVMData _model;

        public SorryCardGameMainViewModel(CommandContainer commandContainer,
            SorryCardGameMainGameClass mainGame,
            SorryCardGameVMData viewModel, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            _model.OtherPile!.SendEnableProcesses(this, () =>
            {
                return _mainGame.SaveRoot!.GameStatus == EnumGameStatus.Regular;
            });
            _model.PlayerHand1!.Maximum = 8;
            CommandContainer!.ExecutingChanged += CommandContainer_ExecutingChanged;
            Stops = new CustomStopWatchCP();
            Stops.MaxTime = 7000;
            Stops.TimeUp += Stops_TimeUp;
            _model.OtherPile.PileClickedAsync += OtherPile_PileClickedAsync;
        }

        private async Task OtherPile_PileClickedAsync()
        {
            var thisList = _model.PlayerHand1!.ListSelectedObjects();
            if (thisList.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose at least one card to play");
                return;
            }
            bool rets = _mainGame!.IsValidMove(thisList);
            if (rets == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal Move");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("regularplay", thisList.GetDeckListFromObjectList());
            await _mainGame.PlaySeveralCards(thisList);
        }

        protected override Task TryCloseAsync()
        {
            CommandContainer!.ExecutingChanged -= CommandContainer_ExecutingChanged;
            Stops.TimeUp -= Stops_TimeUp;
            _model.OtherPile!.PileClickedAsync -= OtherPile_PileClickedAsync;
            return base.TryCloseAsync();
        }
        private async void Stops_TimeUp()
        {
            await StopTimerAsync();
        }
        private async Task StopTimerAsync()
        {
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true;
            int myPlayer = _mainGame!.PlayerList!.GetSelf().Id;
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("timeout", myPlayer);
            await _mainGame.NoSorryAsync(myPlayer);
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return;
            if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry || _mainGame.SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
            {
                _model.PlayerHand1!.AutoSelect = HandObservable<SorryCardGameCardInformation>.EnumAutoType.None;
                Stops!.StartTimer();
                return;
            }
        }

        public CustomStopWatchCP Stops; //since nobody refers to this, can be here this time.

        protected override bool CanEnableDeck()
        {
            return false;
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
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
        private int _upTo;
        [VM]
        public int UpTo
        {
            get { return _upTo; }
            set
            {
                if (SetProperty(ref _upTo, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private string _instructions = "";
        [VM]
        public string Instructions
        {
            get { return _instructions; }
            set
            {
                if (SetProperty(ref _instructions, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private async Task PlaySorryCardAsync(SorryCardGameCardInformation thisCard)
        {
            int myID = _mainGame!.PlayerList!.GetSelf().Id;
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SorryPlay thisPlay = new SorryPlay();
                thisPlay.Deck = thisCard.Deck;
                thisPlay.Player = myID;
                await _mainGame.Network!.SendAllAsync("sorrycard", thisPlay);
            }
            await _mainGame.PlaySorryCardAsync(thisCard, myID);
        }
        protected override async Task ProcessHandClickedAsync(SorryCardGameCardInformation thisCard, int index)
        {
            if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatus.WaitForSorry21)
            {
                Stops!.PauseTimer();
                if (thisCard.Sorry == EnumSorry.At21)
                {
                    await PlaySorryCardAsync(thisCard);
                    return;
                }
                await UIPlatform.ShowMessageAsync("Illegal Move");
                await StopTimerAsync();
                return;
            }
            if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
            {
                Stops!.PauseTimer();
                if (thisCard.Sorry == EnumSorry.Dont)
                {
                    await PlaySorryCardAsync(thisCard);
                    return;
                }
                await UIPlatform.ShowMessageAsync("Illegal Move");
                await StopTimerAsync();
                return;
            }
            throw new BasicBlankException("If the game status is not wait for sorry21 or has don't be sorry; then can't choose just one card to play");
        }

    }
}