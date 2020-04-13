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
using BingoCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using System.Timers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BingoCP.Data;

//i think this is the most common things i like to do
namespace BingoCP.ViewModels
{
    [InstanceGame]
    public class BingoMainViewModel : BasicMultiplayerMainVM
    {
        private readonly BingoMainGameClass _mainGame; //if we don't need, delete.
        private readonly BasicData _basicData;
        private readonly TestOptions _test;
        private readonly Timer _timer;

        public BingoMainViewModel(CommandContainer commandContainer,
            BingoMainGameClass mainGame,
            IViewModelData viewModel, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _basicData = basicData;
            _test = test;
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Interval = 6000;
            _timer.Elapsed += TimerElapsed;
            _mainGame.SetTimerEnabled = ((rets =>
            {
                _timer.Enabled = rets;
            }));
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Execute.OnUIThreadAsync(RunTimerAsync); //hopefully this works.  then i don't need the iprogress anymore.
        }
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            if (_basicData.MultiPlayer && _basicData.Client)
            {
                await _mainGame.CallNextNumberAsync();
            }
        }

        private async Task RunTimerAsync()
        {
            CommandContainer!.ManuelFinish = true;
            CommandContainer.IsExecuting = true;
            _timer.Enabled = false;
            if (_basicData.MultiPlayer == true)
            {
                if (_basicData.Client == true)
                {
                    _mainGame!.Check!.IsEnabled = true; //maybe needs to be after you checked.  or it could get hosed.
                    return; //has to wait for host.
                }
                if (_mainGame!.PlayerList.Any(Items => Items.PlayerCategory == EnumPlayerCategory.Computer))
                {
                    await _mainGame.FinishAsync();
                    return;
                }
                await _mainGame.Network!.SendAllAsync("callnextnumber");
                await _mainGame.CallNextNumberAsync();
                return;
            }
            await _mainGame!.FinishAsync();
        }

        private string _numberCalled = "";
        [VM]
        public string NumberCalled
        {
            get { return _numberCalled; }
            set
            {
                if (SetProperty(ref _numberCalled, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task BingoAsync()
        {
            BingoPlayerItem selfPlayer = _mainGame!.PlayerList!.GetSelf();
            if (selfPlayer.BingoList.HasBingo == false)
            {
                string oldStatus = Status;
                Status = "No Bingos Here";
                await _mainGame!.Delay!.DelayMilli(500);
                Status = oldStatus;
                return;
            }
            if (_basicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("bingo", selfPlayer.Id);
            await _mainGame.GameOverAsync(selfPlayer.Id);
        }
        public bool CanSelectSpace(SpaceInfoCP space)
        {
            if (space.IsEnabled == false)
                return false;
            if (space.AlreadyMarked == true)
                return false;
            if (space.Text == "Free")
                return true;
            if (space.Text == _mainGame.CurrentInfo!.WhatValue.ToString())
                return true;
            if (_test.AllowAnyMove == true)
                return true; //to quickly test bingos
            return false;
        }
        [Command(EnumCommandCategory.Game)]
        public void SelectSpace(SpaceInfoCP space)
        {
            space.AlreadyMarked = true;
            BingoPlayerItem selfPlayer = _mainGame.PlayerList!.GetSelf();
            var thisBingo = selfPlayer.BingoList[space.Vector.Row - 1, space.Vector.Column];
            thisBingo.DidGet = true; //hopefully this simple.
        }
    }
}