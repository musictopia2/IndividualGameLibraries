using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows;
using System.Windows.Controls;
using TileRummyCP.Data;
using TileRummyCP.Logic;
using TileRummyCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace TileRummyWPF.Views
{
    public class TileRummyMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<TileInfo, TileCP, TileWPF> _hand1;
        private readonly BoneYardWPF<TileInfo, TileCP, TileWPF,
            TileShuffler> _pool1;
        private readonly TempRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF> _tempG;
        private readonly MainRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF, TileSet, SavedSet> _mainG;

        private readonly IEventAggregator _aggregator;
        private readonly TileRummyVMData _model;

        public TileRummyMainView(IEventAggregator aggregator,
            TileRummyVMData model
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);


            _score = new ScoreBoardWPF();
            _hand1 = new BaseHandWPF<TileInfo, TileCP, TileWPF>();
            _pool1 = new BoneYardWPF<TileInfo, TileCP, TileWPF, TileShuffler>();
            _tempG = new TempRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF>();
            _mainG = new MainRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF, TileSet, SavedSet>();


            var endButton = GetGamingButton("End Turn", nameof(TileRummyMainViewModel.EndTurnAsync));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            var firstButton = GetGamingButton("Create First Sets", nameof(TileRummyMainViewModel.CreateFirstSetsAsync));
            var otherButton = GetGamingButton("Create New Set", nameof(TileRummyMainViewModel.CreateNewSetAsync));
            var undoButton = GetGamingButton("Reset Moves", nameof(TileRummyMainViewModel.UndoMoveAsync));
            _score.AddColumn("Tiles Left", true, nameof(TileRummyPlayerItem.ObjectCount));
            _score.AddColumn("Score", true, nameof(TileRummyPlayerItem.Score));

            _tempG.Height = 250;
            _pool1.Width = 300; // well see.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(TileRummyMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TileRummyMainViewModel.Status));
            var firstContent = firstInfo.GetContent;
            Grid completeGrid = new Grid();
            AddLeftOverRow(completeGrid, 50);
            AddAutoRows(completeGrid, 1);
            AddLeftOverRow(completeGrid, 50);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_pool1);
            StackPanel stack = new StackPanel();
            stack.Children.Add(firstButton);
            stack.Children.Add(otherButton);
            stack.Children.Add(undoButton);
            stack.Children.Add(endButton);
            otherStack.Children.Add(stack);
            stack = new StackPanel();
            stack.Children.Add(_tempG);
            stack.Children.Add(firstContent);
            stack.Children.Add(_score);
            otherStack.Children.Add(stack);
            AddControlToGrid(completeGrid, otherStack, 0, 0);
            AddControlToGrid(completeGrid, _hand1, 1, 0);
            _pool1.Margin = new Thickness(2, 2, 2, 2);
            AddControlToGrid(completeGrid, _mainG, 2, 0); // used the wrong one.
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

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this); //maybe i have to do this each time.
            return Task.CompletedTask;
        }
    }
}
