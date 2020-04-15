using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TileRummyCP.Data;
using TileRummyCP.Logic;
using TileRummyCP.ViewModels;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace TileRummyXF.Views
{
    public class TileRummyMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<TileInfo, TileCP, TileXF> _hand1;
        private readonly BoneYardXF<TileInfo, TileCP, TileXF,
            TileShuffler> _pool1;
        private readonly TempRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF> _tempG;
        private readonly MainRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF, TileSet, SavedSet> _mainG;

        private readonly IEventAggregator _aggregator;
        private readonly TileRummyVMData _model;
        public TileRummyMainView(IEventAggregator aggregator,
            TileRummyVMData model,
            TestOptions test
            )
        {

            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);


            _score = new ScoreBoardXF();
            _hand1 = new BaseHandXF<TileInfo, TileCP, TileXF>();
            _pool1 = new BoneYardXF<TileInfo, TileCP, TileXF, TileShuffler>();
            _tempG = new TempRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF>();
            _mainG = new MainRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF, TileSet, SavedSet>();

            var endButton = GetSmallerButton("End Turn", nameof(TileRummyMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            var firstButton = GetSmallerButton("Create First Sets", nameof(TileRummyMainViewModel.CreateFirstSetsAsync));
            var otherButton = GetSmallerButton("Create New Set", nameof(TileRummyMainViewModel.CreateNewSetAsync));
            var undoButton = GetSmallerButton("Reset Moves", nameof(TileRummyMainViewModel.UndoMoveAsync));
            _score.AddColumn("Tiles Left", true, nameof(TileRummyPlayerItem.ObjectCount));
            _score.AddColumn("Score", true, nameof(TileRummyPlayerItem.Score));

            if (ScreenUsed == EnumScreen.SmallTablet)
                _tempG.HeightRequest = 120;
            else
                _tempG.HeightRequest = 200;
            _pool1.WidthRequest = 250;

            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TileRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TileRummyMainViewModel.Status));
            var firstContent = firstInfo.GetContent;
            Grid completeGrid = new Grid();
            AddLeftOverRow(completeGrid, 55);
            AddAutoRows(completeGrid, 1);
            AddLeftOverRow(completeGrid, 50);
            AddAutoRows(completeGrid, 1);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_pool1);
            StackLayout stack = new StackLayout();
            stack.Children.Add(firstButton);
            stack.Children.Add(otherButton);
            stack.Children.Add(undoButton);
            stack.Children.Add(endButton);
            otherStack.Children.Add(stack);
            stack = new StackLayout();
            stack.Children.Add(_tempG);
            stack.Children.Add(firstContent);
            stack.Children.Add(_score);
            otherStack.Children.Add(stack);
            AddControlToGrid(completeGrid, otherStack, 0, 0);
            AddControlToGrid(completeGrid, _hand1, 1, 0);
            _pool1.Margin = new Thickness(2, 2, 2, 2);
            AddControlToGrid(completeGrid, _mainG, 2, 0); // used the wrong one.

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(TileRummyMainViewModel.RestoreScreen));
            }
            if (restoreP != null)
            {
                AddControlToGrid(completeGrid, restoreP, 3, 0); //default add to grid but does not have to.
            }


            Content = completeGrid;

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            TileRummySaveInfo save = cons!.Resolve<TileRummySaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList); // the data should come from the playeritem.
            _hand1!.LoadList(_model!.PlayerHand1!, "");
            _model.Pool1!.MaxSize = new SKSize(1000, 1000);
            _model.Pool1.ScatterPieces();
            _pool1!.LoadList(_model.Pool1, "");
            _tempG!.Init(_model.TempSets!, "");
            _mainG!.Init(_model.MainSets1!, "");

            return this.RefreshBindingsAsync(_aggregator);
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
