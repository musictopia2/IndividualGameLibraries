using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.TrickUIs;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using XactikaCP.Cards;
using XactikaCP.Data;
using XactikaCP.MiscImages;
using XactikaCP.ViewModels;
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace XactikaXF.Views
{
    public class XactikaMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly XactikaVMData _model;
        private readonly IGamePackageResolver _resolver;
        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly SeveralPlayersTrickXF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF, XactikaPlayerItem> _trick1;
        private readonly ChooseShapeXF _shape1;
        private readonly StatBoardXF _stats1 = new StatBoardXF();


        public XactikaMainView(IEventAggregator aggregator,
            TestOptions test,
            XactikaVMData model,
            IGamePackageResolver resolver,
            IGamePackageRegister register,
            StatsBoardCP boardCP
            )
        {
            _aggregator = aggregator;
            _model = model;
            _resolver = resolver;
            _aggregator.Subscribe(this);
            _shape1 = new ChooseShapeXF();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF>();
            _trick1 = new SeveralPlayersTrickXF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsXF, XactikaPlayerItem>();
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(XactikaMainViewModel.RestoreScreen));
            }
            register.RegisterControl(_stats1.Element, "main");
            boardCP.LinkBoard();
            _score.AddColumn("Cards Left", false, nameof(XactikaPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Bid Amount", false, nameof(XactikaPlayerItem.BidAmount));
            _score.AddColumn("Tricks Won", false, nameof(XactikaPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(XactikaPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", false, nameof(XactikaPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(XactikaMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(XactikaMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(XactikaMainViewModel.RoundNumber));
            firstInfo.AddRow("Mode", nameof(XactikaMainViewModel.GameModeText));
            StackLayout shapeStack = new StackLayout();

            shapeStack.Children.Add(_shape1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(XactikaMainViewModel.ShapeScreen));
            parent.HorizontalOptions = LayoutOptions.Start;
            parent.VerticalOptions = LayoutOptions.Start;
            shapeStack.Children.Add(parent);
            Grid tempGrid = new Grid();
            AddAutoRows(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            AddAutoColumns(tempGrid, 2);
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            tempStack.Children.Add(_trick1);
            tempStack.Children.Add(shapeStack);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            parent = new ParentSingleUIContainer(nameof(XactikaMainViewModel.BidScreen));
            AddControlToGrid(tempGrid, parent, 0, 0); // if one is visible, then the other is not
            AddControlToGrid(tempGrid, _stats1, 0, 2);
            AddControlToGrid(tempGrid, _score, 0, 1); //problem is scoreboard.

            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 1);
            AddLeftOverRow(finalGrid, 1);
            AddControlToGrid(finalGrid, tempGrid, 0, 0);
            AddControlToGrid(finalGrid, mainStack, 1, 0);

            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            //tempGrid.BackgroundColor = Color.Red;
            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = finalGrid;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return this.RefreshBindingsAsync(_aggregator);
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }
        Task IUIView.TryActivateAsync()
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            XactikaSaveInfo save = cons!.Resolve<XactikaSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _trick1!.Init(_model.TrickArea1, (IMultiplayerTrick<EnumShapes, XactikaCardInformation, XactikaPlayerItem>)_model.TrickArea1, "");
            _stats1.LoadBoard();
            _shape1!.Init(_model, _resolver);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _playerHandWPF.Dispose(); //at least will help improve performance.
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
