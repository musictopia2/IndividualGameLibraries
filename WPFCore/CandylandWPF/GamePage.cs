using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BasicControlsAndWindowsCore.Helpers;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicDrawables.MiscClasses;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CandylandCP;
using CommonBasicStandardLibraries.Messenging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace CandylandWPF
{
    public class GamePage : MultiPlayerWindow<CandylandViewModel, CandylandPlayerItem, CandylandSaveInfo>, IHandle<CandylandCardData>, IHandle<NewTurnEventModel>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _ourBoard!.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private CardGraphicsWPF? _ourCard;
        private GameboardWPF? _ourBoard;
        private PieceWPF? _ourPiece;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            BasicSetUp();
            MainGrid!.Background = Brushes.White;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            _ourBoard = new GameboardWPF();
            OurContainer!.RegisterSingleton(_ourBoard.ThisElement, "main"); //i think
            _ourCard = new CardGraphicsWPF(); // bindings are finished
            _ourCard.SendSize("main", new CandylandCardData());
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel firstStack = new StackPanel();
            otherStack.Children.Add(firstStack);
            firstStack.Children.Add(_ourCard); //you already subscribed.  just hook up another event for this.
            _ourPiece = new PieceWPF();
            _ourPiece.Margin = new Thickness(0, 5, 0, 0);
            _ourPiece.SetSizes();
            BaseLabelGrid firstInfo = new BaseLabelGrid();
            firstInfo.AddRow("Turn", nameof(CandylandViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CandylandViewModel.Status));
            firstStack.Children.Add(firstInfo.GetContent);
            firstStack.Children.Add(_ourPiece);
            _ourBoard.HorizontalAlignment = HorizontalAlignment.Left;
            _ourBoard.VerticalAlignment = VerticalAlignment.Top;
            _ourBoard.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(_ourBoard);
            thisStack.Children.Add(otherStack);
            MainGrid.Children.Add(thisStack);
            AddRestoreCommand(otherStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<CandylandPlayerItem, CandylandSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<CandylandViewModel>();
            OurContainer.RegisterSingleton<IProportionBoard, ProportionWPF>("main"); //here too.
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
            _ourCard!.DataContext = null;
            _ourCard.DataContext = message;
        }
    }
}