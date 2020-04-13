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
using PaydayCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using PaydayCP.Data;
using BasicGameFrameworkLibrary.Dice;
using PaydayCP.Cards;
//i think this is the most common things i like to do
namespace PaydayCP.ViewModels
{

    //try to take a risk.  because another view model has to handle the rolling since it may or may not even be visible.


    [InstanceGame]
    public class PaydayMainViewModel : SimpleBoardGameVM
    {
        private readonly PaydayMainGameClass _mainGame; //if we don't need, delete.
        private readonly PaydayVMData _model; //if we don't need, delete.
        private readonly IGamePackageResolver _resolver;
        private readonly IBuyProcesses _processes;

        public PaydayMainViewModel(CommandContainer commandContainer,
            PaydayMainGameClass mainGame,
            PaydayVMData model, 
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IBuyProcesses processes
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = model;
            _resolver = resolver;
            _processes = processes;
        }


        //anything else needed is here.
        private bool _didInit;
        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            _model.DealPile.SendEnableProcesses(this, () => _mainGame.SaveRoot.GameStatus == EnumStatus.ChooseBuy);
            if (_mainGame.BasicData.IsXamarinForms == false)
            {
                _model.CurrentDealList.ObjectClickedAsync += CurrentDealList_ObjectClickedAsync;
            }
            LoadProperScreensAsync();
        }

        private async Task CurrentDealList_ObjectClickedAsync(DealCard payLoad, int index)
        {
            await _processes.BuyerSelectedAsync(payLoad.Deck);
        }



        //has to figure out all possible screens.


        private async void LoadProperScreensAsync()
        {
            switch (_mainGame.SaveRoot.GameStatus)
            {


                case EnumStatus.Starts:
                case EnumStatus.None:
                case EnumStatus.MakeMove:
                case EnumStatus.RollLottery:
                case EnumStatus.RollRadio:
                case EnumStatus.EndingTurn:
                case EnumStatus.RollCharity:
                    await LoadMainScreensAsync(); //hopefully this simple.
                    break;
                case EnumStatus.ChooseDeal:
                    await CloseRollerScreenAsync();
                    await CloseMailListScreenAsync();
                    await LoadDealPileScreenAsync();
                    await LoadChooseDealScreenAsync(); //i think.
                    break;
                case EnumStatus.ChoosePlayer:
                    await CloseMailListScreenAsync();
                    await CloseRollerScreenAsync();
                    await LoadMailPileScreenAsync();
                    await LoadPlayerScreenAsync(); //this is the only time this is loaded i think.
                    break;
                case EnumStatus.ChooseLottery:
                    await CloseMailListScreenAsync();
                    await CloseRollerScreenAsync();
                    await LoadLotteryScreenAsync();
                    break;
                case EnumStatus.ChooseBuy:


                    if (_mainGame.BasicData.IsXamarinForms)
                    {
                        await CloseMailListScreenAsync();
                        await CloseDealOrBuyScreenAsync();
                        await LoadBuyDealScreenAsync();
                    }
                    else
                    {
                        await LoadMainScreensAsync(); //desktop can show all.
                    }
                    break;
                case EnumStatus.DealOrBuy:
                    await CloseRollerScreenAsync();
                    await LoadMailPileScreenAsync();
                    await CloseMailListScreenAsync();
                    await LoadDealOrBuyScreenAsync();
                    break;
                case EnumStatus.ViewMail:
                    await CloseRollerScreenAsync();
                    await LoadMailPileScreenAsync(); //hopefully this simple.
                    break;
                case EnumStatus.ViewYardSale:
                    await CloseRollerScreenAsync();
                    await LoadDealPileScreenAsync(); //hopefully this simple.
                    break;
                default:
                    break;
            }
            _didInit = true;
        }
        //private async void ChangeScreensAsync(EnumStatus oldStatus)
        //{

        //}

        #region Screen Processes

        private async Task CloseBuyDealScreenAsync()
        {
            if (BuyDealScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BuyDealScreen);
            BuyDealScreen = null;
        }
        private async Task CloseChooseDealScreenAsync()
        {
            if (ChooseDealScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ChooseDealScreen);
            ChooseDealScreen = null;
        }
        private async Task CloseDealOrBuyScreenAsync()
        {
            if (DealOrBuyScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(DealOrBuyScreen);
            DealOrBuyScreen = null;
        }
        private async Task CloseDealPileScreenAsync()
        {
            if (DealPileScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(DealPileScreen);
            DealPileScreen = null;
        }
        private async Task CloseLotteryScreenAsync()
        {
            if (LotteryScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(LotteryScreen);
            LotteryScreen = null;
        }
        private async Task CloseMailPileScreenAsync()
        {
            if (MailPileScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(MailPileScreen);
            MailPileScreen = null;
        }
        private async Task ClosePlayerScreenAsync()
        {
            if (PlayerScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(PlayerScreen);
            PlayerScreen = null;
        }
        private async Task CloseRollerScreenAsync()
        {
            if (RollerScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(RollerScreen);
            RollerScreen = null;
        }
        private async Task CloseMailListScreenAsync()
        {
            if (MailListScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(MailListScreen);
            MailListScreen = null;
        }
        public MailListViewModel? MailListScreen { get; set; }
        public BuyDealViewModel? BuyDealScreen { get; set; }
        public ChooseDealViewModel? ChooseDealScreen { get; set; }
        public DealOrBuyViewModel? DealOrBuyScreen { get; set; }
        public DealPileViewModel? DealPileScreen { get; set; }
        public LotteryViewModel? LotteryScreen { get; set; }
        public MailPileViewModel? MailPileScreen { get; set; }
        public PlayerPickerViewModel? PlayerScreen { get; set; }
        public RollerViewModel? RollerScreen { get; set; }
        private async Task LoadMailListScreenAsync()
        {
            if (MailListScreen != null)
            {
                return;
            }
            MailListScreen = _resolver.Resolve<MailListViewModel>();
            await LoadScreenAsync(MailListScreen);
        }
        private async Task LoadBuyDealScreenAsync()
        {
            if (BuyDealScreen != null)
            {
                return;
            }
            BuyDealScreen = _resolver.Resolve<BuyDealViewModel>();
            await LoadScreenAsync(BuyDealScreen);
        }
        private async Task LoadChooseDealScreenAsync()
        {
            if (ChooseDealScreen != null)
            {
                return;
            }
            ChooseDealScreen = _resolver.Resolve<ChooseDealViewModel>();
            await LoadScreenAsync(ChooseDealScreen);
        }
        private async Task LoadDealOrBuyScreenAsync()
        {
            if (DealOrBuyScreen != null)
            {
                return;
            }
            DealOrBuyScreen = _resolver.Resolve<DealOrBuyViewModel>();
            await LoadScreenAsync(DealOrBuyScreen);
        }
        private async Task LoadDealPileScreenAsync()
        {
            if (DealPileScreen != null)
            {
                return;
            }
            DealPileScreen = _resolver.Resolve<DealPileViewModel>();
            await LoadScreenAsync(DealPileScreen);
        }
        private async Task LoadLotteryScreenAsync()
        {
            if (LotteryScreen != null)
            {
                return;
            }
            LotteryScreen = _resolver.Resolve<LotteryViewModel>();
            await LoadScreenAsync(LotteryScreen);
        }
        private async Task LoadMailPileScreenAsync()
        {
            if (MailPileScreen != null)
            {
                return;
            }
            MailPileScreen = _resolver.Resolve<MailPileViewModel>();
            await LoadScreenAsync(MailPileScreen);
        }
        private async Task LoadPlayerScreenAsync()
        {
            if (PlayerScreen != null)
            {
                return;
            }
            PlayerScreen = _resolver.Resolve<PlayerPickerViewModel>();
            await LoadScreenAsync(PlayerScreen);
        }
        private async Task LoadRollerScreenAsync()
        {
            if (RollerScreen != null)
            {
                return;
            }
            RollerScreen = _resolver.Resolve<RollerViewModel>();
            await LoadScreenAsync(RollerScreen);
        }

        private async Task LoadMainScreensAsync()
        {
            await CloseBuyDealScreenAsync();
            await CloseChooseDealScreenAsync();
            await CloseDealOrBuyScreenAsync();
            await CloseDealPileScreenAsync();
            await CloseLotteryScreenAsync();
            await CloseMailPileScreenAsync();
            await ClosePlayerScreenAsync();
            await LoadRollerScreenAsync();
            await LoadMailListScreenAsync();
        }


        #endregion
        private EnumStatus _gameStatus;
        [VM]
        public EnumStatus GameStatus
        {
            get { return _gameStatus; }
            set
            {
                //EnumStatus oldStatus = _gameStatus;
                if (SetProperty(ref _gameStatus, value))
                {
                    //can decide what to do when property changes
                    //this will decide what to do.  since this determines like game of life the screens.
                    if (_didInit == false)
                    {
                        return;
                    }
                    LoadProperScreensAsync(); //hopefully this simple.
                    //ChangeScreensAsync(oldStatus);
                }
            }
        }
        private string _monthLabel = "";
        [VM]
        public string MonthLabel
        {
            get
            {
                return _monthLabel;
            }
            set
            {
                if (SetProperty(ref _monthLabel, value) == true)
                {
                }
            }
        }

        private string _otherLabel = "";
        [VM]
        public string OtherLabel
        {
            get
            {
                return _otherLabel;
            }
            set
            {
                if (SetProperty(ref _otherLabel, value) == true)
                {
                }
            }
        }

        

    }
}