using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;
using LifeBoardGameCP.EventModels;
using LifeBoardGameCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace LifeBoardGameCP.ViewModels
{
    [InstanceGame]
    public class LifeBoardGameMainViewModel : SimpleBoardGameVM, IHandleAsync<ShowCardEventModel>
    {
        private readonly LifeBoardGameMainGameClass _mainGame; //if we don't need, delete.
        //private readonly BasicData _basicData;
        private readonly IGamePackageResolver _resolver;

        //this is going to be the hardest.  because this one is responsible for making sure the proper screens load.

        public LifeBoardGameMainViewModel(CommandContainer commandContainer,
            LifeBoardGameMainGameClass mainGame,
            LifeBoardGameVMData model,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            LifeBoardGameGameContainer gameContainer
            )
            : base(commandContainer, mainGame, model, basicData, test, resolver)
        {
            _mainGame = mainGame;
            if (_mainGame.SaveRoot.GameStatus == EnumWhatStatus.NeedChooseGender)
            {
                throw new BasicBlankException("Cannot load main view model because you need to choose gender.  Rethink");
            }
            if (_mainGame.PlayerList.Any(x => x.Gender == EnumGender.None))
            {
                throw new BasicBlankException("Cannot load the main screen if sombody did not choose gender.  Rethink");
            }
            //_basicData = basicData;
            _resolver = resolver;
            gameContainer.HideCardAsync = HideCardAsync;
            gameContainer.CardVisible = (() => ShowCardScreen != null);
            if (LifeBoardGameGameContainer.StartCollegeCareer && basicData.GamePackageMode == EnumGamePackageMode.Production)
            {
                throw new BasicBlankException("You cannot start college career because its in production");
            }
        }

        private async Task HideCardAsync()
        {
            if (ShowCardScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ShowCardScreen);
            ShowCardScreen = null;
        }
        private async Task ShowCardAsync(EnumWhatStatus oldStatus)
        {
            switch (oldStatus)
            {

                case EnumWhatStatus.NeedChooseFirstCareer:
                case EnumWhatStatus.NeedNewCareer:
                    await CloseCareerAsync();
                    await LoadShowCardAsync();
                    break;
                case EnumWhatStatus.NeedChooseHouse:
                case EnumWhatStatus.NeedSellHouse:
                case EnumWhatStatus.NeedSellBuyHouse:
                    await CloseHouseAsync();
                    await LoadShowCardAsync();
                    break;
                case EnumWhatStatus.NeedReturnStock:
                    await CloseReturnStockAsync();
                    await LoadShowCardAsync();
                    break;
                case EnumWhatStatus.NeedChooseStock:
                    await CloseChooseStockAsync();
                    await LoadShowCardAsync();
                    break;
                case EnumWhatStatus.NeedChooseSalary:
                    await CloseChooseSalaryAsync();
                    await LoadShowCardAsync();
                    break; //sometimes it shows up for a second.  other times, only for a while.
                default:
                    await LoadShowCardAsync(); //in this case, just show the card.
                    break;
            }
        }

        #region Load Screens
        private async Task LoadScoreAsync()
        {
            if (ScoreScreen != null)
            {
                return;
            }
            ScoreScreen = _resolver.Resolve<LifeScoreboardViewModel>();
            await LoadScreenAsync(ScoreScreen);
        }
        private async Task LoadCareerAsync()
        {
            if (CareerScreen != null)
            {
                return;
            }
            CareerScreen = _resolver.Resolve<ChooseCareerViewModel>();
            await LoadScreenAsync(CareerScreen);
        }
        private async Task LoadChooseSalaryAsync()
        {
            if (ChooseSalaryScreen != null)
            {
                return;
            }
            ChooseSalaryScreen = _resolver.Resolve<ChooseSalaryViewModel>();
            await LoadScreenAsync(ChooseSalaryScreen);
        }
        private async Task LoadChooseStockAsync()
        {
            if (ChooseStockScreen != null)
            {
                return;
            }
            ChooseStockScreen = _resolver.Resolve<ChooseStockViewModel>();
            await LoadScreenAsync(ChooseStockScreen);
        }
        private async Task LoadReturnStockAsync()
        {
            if (ReturnStockScreen != null)
            {
                return;
            }
            ReturnStockScreen = _resolver.Resolve<ReturnStockViewModel>();
            await LoadScreenAsync(ReturnStockScreen);
        }
        private async Task LoadSpinnerAsync()
        {
            if (SpinnerScreen != null)
            {
                return;
            }
            SpinnerScreen = _resolver.Resolve<SpinnerViewModel>();
            await LoadScreenAsync(SpinnerScreen);
        }
        private async Task LoadStealTilesAsync()
        {
            if (StealTilesScreen != null)
            {
                return;
            }
            StealTilesScreen = _resolver.Resolve<StealTilesViewModel>();
            await LoadScreenAsync(StealTilesScreen);
        }
        private async Task LoadTradeSalaryAsync()
        {
            if (TradeSalaryScreen != null)
            {
                return;
            }
            TradeSalaryScreen = _resolver.Resolve<TradeSalaryViewModel>();
            await LoadScreenAsync(TradeSalaryScreen);
        }
        private async Task LoadHouseAsync()
        {
            if (HouseScreen != null)
            {
                return;
            }
            HouseScreen = _resolver.Resolve<ChooseHouseViewModel>();
            await LoadScreenAsync(HouseScreen);
        }
        private Task LoadBoardAsync()
        {
            return Task.CompletedTask; //for now  taking some risks.
            //if (_basicData.IsXamarinForms == false)
            //{
            //    return; //hopefully no need to load board because only xamarin forms needs this part.
            //}
            //if (BoardScreen != null)
            //{
            //    return;
            //}
            //BoardScreen = _resolver.Resolve<GameboardViewModel>();
            //await LoadScreenAsync(BoardScreen);
        }
        private async Task LoadShowCardAsync()
        {
            if (ShowCardScreen != null)
            {
                return;
            }
            ShowCardScreen = _resolver.Resolve<ShowCardViewModel>();
            await LoadScreenAsync(ShowCardScreen);
        }
        #endregion
        #region Screens
        public LifeScoreboardViewModel? ScoreScreen { get; set; }
        public ChooseCareerViewModel? CareerScreen { get; set; }
        public ChooseSalaryViewModel? ChooseSalaryScreen { get; set; }
        public ChooseStockViewModel? ChooseStockScreen { get; set; }
        public ReturnStockViewModel? ReturnStockScreen { get; set; }
        public SpinnerViewModel? SpinnerScreen { get; set; }
        public StealTilesViewModel? StealTilesScreen { get; set; }
        public TradeSalaryViewModel? TradeSalaryScreen { get; set; }
        public ChooseHouseViewModel? HouseScreen { get; set; }

        //decided to risk not having boardscreen since too many problems doing portrait on tablets.


        //public GameboardViewModel? BoardScreen { get; set; } //desktop can always show up.
        public ShowCardViewModel? ShowCardScreen { get; set; }
        //for return, if i don't have a choice, it should show the card.
        #endregion
        #region Unload Screens
        private async Task CloseScoreAsync()
        {
            if (ScoreScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ScoreScreen);
            ScoreScreen = null;
        }
        private async Task CloseCareerAsync()
        {
            if (CareerScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(CareerScreen);
            CareerScreen = null;
        }
        private async Task CloseChooseSalaryAsync()
        {
            if (ChooseSalaryScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ChooseSalaryScreen);
            ChooseSalaryScreen = null;
        }
        private async Task CloseChooseStockAsync()
        {
            if (ChooseStockScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ChooseStockScreen);
            ChooseStockScreen = null;
        }
        private async Task CloseReturnStockAsync()
        {
            if (ReturnStockScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ReturnStockScreen);
            ReturnStockScreen = null;
        }
        private async Task CloseSpinnerAsync()
        {
            if (SpinnerScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(SpinnerScreen);
            SpinnerScreen = null;
        }
        private async Task CloseStealTilesAsync()
        {
            if (StealTilesScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(StealTilesScreen);
            StealTilesScreen = null;
        }
        private async Task CloseTradeSalaryAsync()
        {
            if (TradeSalaryScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(TradeSalaryScreen);
            TradeSalaryScreen = null;
        }
        private async Task CloseHouseAsync()
        {
            if (HouseScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(HouseScreen);
            HouseScreen = null;
        }
        private Task CloseBoardAsync()
        {
            return Task.CompletedTask;
            //if (_basicData.IsXamarinForms == false)
            //{
            //    return; //because always needs to show up.  this is the best way to handle this
            //}
            //if (BoardScreen == null)
            //{
            //    return;
            //}
            //await CloseSpecificChildAsync(BoardScreen);
            //BoardScreen = null;
        }
        #endregion
        private bool _didInit;
        private async Task LoadSpinnerBoardScoresAsync()
        {
            await LoadScoreAsync();
            await LoadSpinnerAsync();
            await LoadBoardAsync();
        }


        private async Task LoadProperScreensAsync()
        {
            switch (_mainGame.SaveRoot.GameStatus)
            {
                case EnumWhatStatus.NeedChooseFirstOption:
                case EnumWhatStatus.NeedNight: //hopefully this is fine (?)
                case EnumWhatStatus.NeedToChooseSpace:
                case EnumWhatStatus.NeedChooseRetirement:
                case EnumWhatStatus.NeedToEndTurn:
                    await LoadScoreAsync();
                    await LoadBoardAsync();
                    break;
                case EnumWhatStatus.None:
                    throw new BasicBlankException("The status cannot be done.  Rethink");
                case EnumWhatStatus.NeedToSpin:
                case EnumWhatStatus.LastSpin: //i think.
                case EnumWhatStatus.MakingMove:
                    await LoadSpinnerBoardScoresAsync();
                    break;
                case EnumWhatStatus.NeedChooseStock:
                    //await LoadScoreAsync();
                    //await LoadBoardAsync();
                    await LoadChooseStockAsync(); //i think this is it for this case.
                    break;
                case EnumWhatStatus.NeedChooseFirstCareer:

                case EnumWhatStatus.NeedNewCareer:
                    //await LoadScoreAsync();
                    //await LoadBoardAsync();
                    await LoadCareerAsync(); //i think.
                    break;
                case EnumWhatStatus.NeedChooseSalary:
                    //await LoadScoreAsync();
                    //await LoadBoardAsync();
                    await LoadChooseSalaryAsync();
                    break;
                case EnumWhatStatus.NeedChooseGender:
                    throw new BasicBlankException("Cannot choose gender on main view model.  Rethink");
                case EnumWhatStatus.NeedChooseHouse:
                    await LoadSpinnerAsync();
                    await LoadHouseAsync();
                    break;
                case EnumWhatStatus.NeedTradeSalary: //hint.  no gameboard.
                    await LoadTradeSalaryAsync();
                    await LoadScoreAsync(); //i think just those 2
                    break;
                case EnumWhatStatus.NeedStealTile: //hint.  no gameboard.
                    await LoadScoreAsync();
                    await LoadStealTilesAsync();
                    break;




                case EnumWhatStatus.NeedReturnStock:
                    //well see about the return stocks (?)
                    await LoadReturnStockAsync();
                    await LoadScoreAsync();
                    await LoadBoardAsync();
                    break;
                case EnumWhatStatus.NeedSellBuyHouse: //hint.  no gameboard this time.

                case EnumWhatStatus.NeedSellHouse:

                case EnumWhatStatus.NeedFindSellPrice: //both require same parts.
                    await LoadBoardAsync(); //try to risk showing the board.  because otherwise, no easy way to sell house.
                    await LoadSpinnerAsync(); //for sure needed so can spin if necessary as well.
                    await LoadScoreAsync(); //could be iffy.
                    await LoadShowCardAsync();


                    break;




                default:
                    break;
            }
        }

        protected override async Task ActivateAsync()
        {
            await base.ActivateAsync();
            await LoadProperScreensAsync();
            //there are other things needed though.  this is the beginnings.
            //beginning screens here.

            _didInit = true;
        }

        private async void ChangeScreensAsync(EnumWhatStatus oldStatus)
        {
            //try to not worry about old status.
            //most likely, for some things, you have to manually close out.
            //best way to do so manually is via delegates.
            if (oldStatus == EnumWhatStatus.NeedTradeSalary)
            {
                await CloseTradeSalaryAsync();
            }
            if (oldStatus == EnumWhatStatus.NeedStealTile)
            {
                await CloseStealTilesAsync();
            }
            if (oldStatus == EnumWhatStatus.NeedChooseHouse || oldStatus == EnumWhatStatus.NeedSellBuyHouse || oldStatus == EnumWhatStatus.NeedSellHouse)
            {
                await CloseHouseAsync();
            }
            switch (GameStatus)
            {
                case EnumWhatStatus.NeedChooseFirstOption:
                case EnumWhatStatus.NeedNight: //hopefully this is fine (?)
                case EnumWhatStatus.NeedToChooseSpace:
                case EnumWhatStatus.NeedChooseRetirement:
                case EnumWhatStatus.NeedToEndTurn:
                    await CloseSpinnerAsync(); //hopefully this simple.
                    break;

                case EnumWhatStatus.NeedChooseSalary:
                case EnumWhatStatus.NeedChooseStock:

                case EnumWhatStatus.NeedChooseFirstCareer:

                case EnumWhatStatus.NeedNewCareer:
                    await CloseBoardAsync();
                    await CloseSpinnerAsync();
                    await CloseScoreAsync();
                    break;
                case EnumWhatStatus.NeedSellBuyHouse: //hint.  no gameboard this time.
                    await CloseScoreAsync();
                    break;
                case EnumWhatStatus.NeedSellHouse:

                case EnumWhatStatus.NeedFindSellPrice: //both require same parts.
                    await CloseHouseAsync();
                    await CloseScoreAsync();
                    break; //hopefully can keep scoreboard even for tablets now.
                           //if we find anything else, will put in there as well (?)
                case EnumWhatStatus.NeedTradeSalary:
                case EnumWhatStatus.NeedStealTile:
                    await CloseSpinnerAsync();
                    break;
                default:
                    break;
            }
            await LoadProperScreensAsync();
        }

        Task IHandleAsync<ShowCardEventModel>.HandleAsync(ShowCardEventModel message)
        {
            return ShowCardAsync(message.GameStatus);
        }

        private EnumWhatStatus _gameStatus = EnumWhatStatus.None;
        [VM]
        public EnumWhatStatus GameStatus
        {
            get { return _gameStatus; }
            set
            {
                EnumWhatStatus oldStatus = GameStatus;
                if (SetProperty(ref _gameStatus, value))
                {
                    if (_didInit == false)
                    {
                        return; //because something else will process.
                    }
                    ChangeScreensAsync(oldStatus);
                }
            }
        }
        private string _gameDetails = "";
        [VM]
        public string GameDetails
        {
            get { return _gameDetails; }
            set
            {
                if (SetProperty(ref _gameDetails, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}