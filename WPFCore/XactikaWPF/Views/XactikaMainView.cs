using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.BasicControls.TrickUIs;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using XactikaCP.Cards;
using XactikaCP.Data;
using XactikaCP.MiscImages;
using XactikaCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace XactikaWPF.Views
{
    public class XactikaMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly XactikaVMData _model;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly SeveralPlayersTrickWPF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF, XactikaPlayerItem> _trick1;
        private readonly ChooseShapeWPF _shape1;
        private readonly StatBoardWPF _stats1 = new StatBoardWPF();
        public XactikaMainView(IEventAggregator aggregator,
            TestOptions test,
            XactikaVMData model,
            IGamePackageRegister register,
            StatsBoardCP boardCP
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            register.RegisterControl(_stats1.Element, "main");
            boardCP.LinkBoard();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF>();
            _shape1 = new ChooseShapeWPF();
            _trick1 = new SeveralPlayersTrickWPF<EnumShapes, XactikaCardInformation, XactikaGraphicsCP, CardGraphicsWPF, XactikaPlayerItem>();

            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(XactikaMainViewModel.RestoreScreen)
                };
            }


            _score.AddColumn("Cards Left", false, nameof(XactikaPlayerItem.ObjectCount)); //very common.
            _score.AddColumn("Bid Amount", false, nameof(XactikaPlayerItem.BidAmount));
            _score.AddColumn("Tricks Won", false, nameof(XactikaPlayerItem.TricksWon));
            _score.AddColumn("Current Score", false, nameof(XactikaPlayerItem.CurrentScore));
            _score.AddColumn("Total Score", false, nameof(XactikaPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(XactikaMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(XactikaMainViewModel.Status));
            firstInfo.AddRow("Round", nameof(XactikaMainViewModel.RoundNumber));
            firstInfo.AddRow("Mode", nameof(XactikaMainViewModel.GameModeText));
            StackPanel shapeStack = new StackPanel();

            shapeStack.Children.Add(_shape1);
            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(XactikaMainViewModel.ShapeScreen)
            };
            shapeStack.Children.Add(parent);
            Grid tempGrid = new Grid();
            AddAutoRows(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            AddAutoColumns(tempGrid, 2);
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            tempStack.Children.Add(_trick1);
            tempStack.Children.Add(shapeStack);
            AddControlToGrid(tempGrid, tempStack, 0, 0);
            parent = new ParentSingleUIContainer()
            {
                Name = nameof(XactikaMainViewModel.BidScreen)
            };
            AddControlToGrid(tempGrid, parent, 0, 0); // if one is visible, then the other is not
            AddControlToGrid(tempGrid, _stats1, 0, 2);
            AddControlToGrid(tempGrid, _score, 0, 1);
            mainStack.Children.Add(tempGrid);
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }



        Task IUIView.TryActivateAsync()
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            XactikaSaveInfo save = cons!.Resolve<XactikaSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _trick1!.Init(_model.TrickArea1, (IMultiplayerTrick<EnumShapes, XactikaCardInformation, XactikaPlayerItem>)_model.TrickArea1, "");
            _stats1.LoadBoard();
            _shape1!.Init(_model);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _trick1.Dispose();
            return Task.CompletedTask;
        }
    }
}
