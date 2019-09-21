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
using BasicGameFramework.MainViewModels;
using BasicGameFramework.DIContainers;
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using BasicGameFramework.CommandClasses; //often times we will need commands.
using BasicGameFramework.BasicGameDataClasses;
using System.Timers;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
namespace BingoCP
{
    public class BingoViewModel : BasicMultiplayerVM<BingoPlayerItem, BingoMainGameClass>
    {
        public BingoViewModel(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
        public BasicGameCommand<SpaceInfoCP>? SelectSpaceCommand { get; set; }
        public BasicGameCommand? BingoCommand { get; set; }
        protected override void EndInit()
        {
            base.EndInit(); //best to keep this to be safe.
            Timer1 = new Timer();
            Timer1.Enabled = false;
            Timer1.Interval = 6000;
            Timer1.Elapsed += Timer1_Elapsed;
            TimerProgress = new Progress<int>(async x =>
            {
                CommandContainer!.ManuelFinish = true;
                CommandContainer.IsExecuting = true;
                Timer1.Enabled = false;
                if (ThisData!.MultiPlayer == true)
                {
                    if (ThisData.Client == true)
                    {
                        MainGame!.ThisCheck!.IsEnabled = true; //maybe needs to be after you checked.  or it could get hosed.
                        return; //has to wait for host.
                    }
                    if (MainGame!.PlayerList.Any(Items => Items.PlayerCategory == EnumPlayerCategory.Computer))
                    {
                        await MainGame.FinishAsync();
                        return;
                    }
                    await ThisNet!.SendAllAsync("callnextnumber");
                    await MainGame.CallNextNumberAsync();
                    return;
                }
                await MainGame!.FinishAsync();
            });
            SelectSpaceCommand = new BasicGameCommand<SpaceInfoCP>(this, thisSpace =>
            {
                thisSpace.AlreadyMarked = true;
                BingoPlayerItem selfPlayer = MainGame!.PlayerList!.GetSelf();
                var thisBingo = selfPlayer.BingoList[thisSpace.Vector.Row - 1, thisSpace.Vector.Column];
                thisBingo.DidGet = true; //hopefully this simple.
            }, thisSpace =>
            {
                if (thisSpace.IsEnabled == false)
                    return false;
                if (thisSpace.AlreadyMarked == true)
                    return false;
                if (thisSpace.Text == "Free")
                    return true;
                if (thisSpace.Text == MainGame!.CurrentInfo!.WhatValue.ToString())
                    return true;
                if (ThisTest!.AllowAnyMove == true)
                    return true; //to quickly test bingos
                return false;
            }, this, CommandContainer!);
            BingoCommand = new BasicGameCommand(this, async items =>
            {
                BingoPlayerItem selfPlayer = MainGame!.PlayerList!.GetSelf();
                if (selfPlayer.BingoList.HasBingo == false)
                {
                    string oldStatus = Status;
                    Status = "No Bingos Here";
                    await MainGame!.Delay!.DelayMilli(500);
                    Status = oldStatus;
                    return;
                }
                if (ThisData!.MultiPlayer == true)
                    await ThisNet!.SendAllAsync("bingo", selfPlayer.Id);
                await MainGame.GameOverAsync(selfPlayer.Id);
            }, Items =>
            {
                return true;
            }, this, CommandContainer!);

        }

        internal Timer? Timer1;
        private IProgress<int>? TimerProgress;

        private void Timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerProgress!.Report(0);
        }

        private string _NumberCalled = "";

        public string NumberCalled
        {
            get { return _NumberCalled; }
            set
            {
                if (SetProperty(ref _NumberCalled, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
    }
}