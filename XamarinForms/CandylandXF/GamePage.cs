using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicDrawables.MiscClasses;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicXFControlsAndPages.Helpers;
using CandylandCP;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace CandylandXF
{
    public class GamePage : MultiPlayerPage<CandylandViewModel, CandylandPlayerItem, CandylandSaveInfo>, IHandle<CandylandCardData>, IHandle<NewTurnEventModel>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _ourBoard!.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private CardGraphicsXF? _ourCard;
        private GameBoardXF? _ourBoard;
        private PieceXF? _ourPiece;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            BasicSetUp();
            MainGrid!.BackgroundColor = Color.White;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _ourBoard = new GameBoardXF();
            OurContainer!.RegisterSingleton(_ourBoard.ThisElement, "main"); //i think
            _ourCard = new CardGraphicsXF(); // bindings are finished
            _ourCard.SendSize("main", new CandylandCardData());
            _ourCard.Margin = new Thickness(5, 5, 5, 5);
            Grid tempGrid = new Grid();
            AddAutoColumns(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            StackLayout firstStack = new StackLayout();
            AddControlToGrid(tempGrid, firstStack, 0, 1);
            firstStack.Children.Add(_ourCard); //you already subscribed.  just hook up another event for this.
            _ourPiece = new PieceXF();
            _ourPiece.Margin = new Thickness(5, 5, 5, 5);
            _ourPiece.SetSizes();
            BaseLabelGrid firstInfo = new BaseLabelGrid();
            firstInfo.AddRow("Turn", nameof(CandylandViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CandylandViewModel.Status));
            firstStack.Children.Add(firstInfo.GetContent);
            firstStack.Children.Add(_ourPiece);
            _ourBoard.HorizontalOptions = LayoutOptions.Start;
            _ourBoard.VerticalOptions = LayoutOptions.Start;
            _ourBoard.Margin = new Thickness(5, 5, 0, 0);
            AddControlToGrid(tempGrid, _ourBoard, 0, 0);
            thisStack.Children.Add(tempGrid);
            MainGrid.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<CandylandPlayerItem, CandylandSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<CandylandViewModel>();
            OurContainer.RegisterSingleton<IProportionBoard, ProportionXF>("main"); //here too.
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("main");
            OurContainer.RegisterType<DrawShuffleClass<CandylandCardData, CandylandPlayerItem>>();
            OurContainer.RegisterType<GenericCardShuffler<CandylandCardData>>();
            OurContainer.RegisterSingleton<IDeckCount, CandylandCount>();
        }
        public void Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _ourBoard!.PieceForCurrentPlayer(); //hopefully that is it.
        }
        public void Handle(CandylandCardData message)
        {
            _ourCard!.BindingContext = null;
            _ourCard.BindingContext = message;
        }
    }
}