using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicXFControlsAndPages.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CandylandCP.Data;
using CandylandCP.ViewModels;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;

namespace CandylandXF.Views
{
    public class CandylandMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>, IHandle<CandylandCardData>, IHandle<NewTurnEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly CardGraphicsXF _ourCard;
        private readonly GameBoardXF _ourBoard;
        private readonly PieceXF _ourPiece;
        public CandylandMainView(IEventAggregator aggregator,
            TestOptions test,
            IGamePackageRegister register
            )
        {
            BackgroundColor = Color.White;
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(CandylandMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            _ourBoard = new GameBoardXF();
            register.RegisterControl(_ourBoard.Element, "main");
            _ourCard = new CardGraphicsXF(); // bindings are finished
            _ourCard.SendSize("main", new CandylandCardData());
            otherStack.Margin = new Thickness(5, 5, 5, 5);
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout firstStack = new StackLayout();
            otherStack.Children.Add(firstStack);
            firstStack.Children.Add(_ourCard); //you already subscribed.  just hook up another event for this.
            _ourPiece = new PieceXF();
            _ourPiece.Margin = new Thickness(0, 5, 0, 0);
            _ourPiece.SetSizes();
            BaseLabelGrid firstInfo = new BaseLabelGrid();
            firstInfo.AddRow("Turn", nameof(CandylandMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(CandylandMainViewModel.Status));
            firstStack.Children.Add(firstInfo.GetContent);
            firstStack.Children.Add(_ourPiece);
            _ourBoard.HorizontalOptions = LayoutOptions.Start;
            _ourBoard.VerticalOptions = LayoutOptions.Start;
            _ourBoard.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(_ourBoard);
            mainStack.Children.Add(otherStack);


            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        void IHandle<CandylandCardData>.Handle(CandylandCardData message)
        {
            _ourCard!.BindingContext = null;
            _ourCard.BindingContext = message;
        }

        void IHandle<NewTurnEventModel>.Handle(NewTurnEventModel message)
        {
            _ourPiece!.MainColor = _ourBoard!.PieceForCurrentPlayer(); //hopefully that is it.
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _ourBoard!.LoadBoard();
            return Task.CompletedTask;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
