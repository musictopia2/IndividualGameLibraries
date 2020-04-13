using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using XactikaCP.Cards;
using XactikaCP.Data;
using XactikaCP.Logic;
namespace XactikaCP.ViewModels
{
    [InstanceGame]
    public class XactikaMainViewModel : TrickCardGamesVM<XactikaCardInformation, EnumShapes>
    {
        private readonly XactikaVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly XactikaGameContainer _gameContainer; //if not needed, delete.

        public XactikaMainViewModel(CommandContainer commandContainer,
            XactikaMainGameClass mainGame,
            XactikaVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            XactikaGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _model = viewModel;
            _resolver = resolver;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
            _gameContainer.LoadBiddingAsync = LoadBiddingAsync;
            _gameContainer.CloseBiddingAsync = CloseBiddingAsync;
            _gameContainer.LoadShapeButtonAsync = LoadShapeAsync;
            _gameContainer.CloseShapeButtonAsync = CloseShapeAsync;
        }
        public async Task LoadShapeAsync()
        {
            if (ShapeScreen != null)
            {
                return;
            }
            ShapeScreen = _resolver.Resolve<XactikaSubmitShapeViewModel>();
            await LoadScreenAsync(ShapeScreen);
        }
        public async Task CloseShapeAsync()
        {
            if (ShapeScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(ShapeScreen);
            ShapeScreen = null;
        }
        public XactikaSubmitShapeViewModel? ShapeScreen { get; set; }

        private async Task LoadBiddingAsync()
        {
            if (BidScreen != null)
            {
                return;
            }
            _model.TrickArea1.Visible = false;
            _model.ShapeChoose1.Visible = false;
            BidScreen = _resolver.Resolve<XactikaBidViewModel>();
            await LoadScreenAsync(BidScreen);
        }
        private async Task CloseBiddingAsync()
        {
            if (BidScreen == null)
            {
                return;
            }
            await CloseSpecificChildAsync(BidScreen);
            BidScreen = null;
            _model.TrickArea1.Visible = true;
        }

        public XactikaBidViewModel? BidScreen { get; set; }

        private string _gameModeText = "";
        [VM]
        public string GameModeText
        {
            get { return _gameModeText; }
            set
            {
                if (SetProperty(ref _gameModeText, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumShapes _shapeChosen;

        public EnumShapes ShapeChosen
        {
            get { return _shapeChosen; }
            set
            {
                if (SetProperty(ref _shapeChosen, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _roundNumber;
        [VM]
        public int RoundNumber
        {
            get { return _roundNumber; }
            set
            {
                if (SetProperty(ref _roundNumber, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
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
    }
}