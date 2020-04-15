using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using RummyDiceCP.Data;
using RummyDiceCP.Logic;
using RummyDiceCP.ViewModels;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace RummyDiceXF.Views
{
    public class RummyDiceMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly RummyDiceMainGameClass _mainGame;
        readonly CustomBasicList<RummyDiceHandXF> _tempList = new CustomBasicList<RummyDiceHandXF>();
        private readonly ScoreBoardXF _thisScore;
        private readonly RummyDiceListXF _diceControl;
        public RummyDiceMainView(IEventAggregator aggregator,
            TestOptions test,
            RummyDiceMainGameClass mainGame
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _mainGame = mainGame;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(RummyDiceMainViewModel.RestoreScreen));
            }


            Grid tempGrid = new Grid();
            AddLeftOverRow(tempGrid, 50);
            AddLeftOverRow(tempGrid, 50);
            AddAutoRows(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            AddAutoColumns(tempGrid, 1);

            RummyDiceHandXF thisTemp = new RummyDiceHandXF();
            thisTemp.Index = 1;
            AddControlToGrid(tempGrid, thisTemp, 0, 0);
            _tempList!.Add(thisTemp); //forgot this too.
            thisTemp = new RummyDiceHandXF();
            thisTemp.Index = 2;
            AddControlToGrid(tempGrid, thisTemp, 1, 0);
            _tempList.Add(thisTemp);

            _thisScore = new ScoreBoardXF();
            AddControlToGrid(tempGrid, _thisScore, 0, 1);
            Grid.SetRowSpan(_thisScore, 3);
            mainStack.Children.Add(tempGrid);

            SimpleLabelGridXF otherScore = new SimpleLabelGridXF();
            otherScore.AddRow("Roll", nameof(RummyDiceMainViewModel.RollNumber), new RollConverter());
            otherScore.AddRow("Score", nameof(RummyDiceMainViewModel.Score));
            tempGrid = otherScore.GetContent;
            tempGrid.VerticalOptions = LayoutOptions.End; // try this way
            var otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(tempGrid);
            Button thisBut;
            thisBut = GetGamingButton("Put Back", nameof(RummyDiceMainViewModel.BoardAsync));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(thisBut);
            mainStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _thisScore.AddColumn("Score Round", false, nameof(RummyDicePlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", false, nameof(RummyDicePlayerItem.ScoreGame));
            _thisScore.AddColumn("Phase", false, nameof(RummyDicePlayerItem.Phase));
            _thisScore.VerticalOptions = LayoutOptions.Start;
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            mainStack.Children.Add(otherStack);
            _diceControl = new RummyDiceListXF();
            thisBut = GetGamingButton("Roll", nameof(RummyDiceMainViewModel.RollAsync));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_diceControl);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RummyDiceMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RummyDiceMainViewModel.Status));
            firstInfo.AddRow("Phase", nameof(RummyDiceMainViewModel.CurrentPhase));
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout bottomStack = new StackLayout();
            otherStack.Children.Add(bottomStack);
            var endButton = GetGamingButton("End Turn", nameof(RummyDiceMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.Margin = new Thickness(5, 0, 0, 20);
            bottomStack.Children.Add(endButton);
            thisBut = GetGamingButton("Score Hand", nameof(RummyDiceMainViewModel.CheckAsync));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            bottomStack.Children.Add(thisBut);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            if (_mainGame!.TempSets!.Count == 0)
                throw new BasicBlankException("You need to have 2 sets, not none for tempsets");
            _mainGame.TempSets.ForEach(tempCP =>
            {
                RummyDiceHandXF thisTemp = _tempList.Single(items => items.Index == tempCP.Index);
                thisTemp.LoadList(tempCP, _mainGame);
            });
            _diceControl!.LoadDiceViewModel(_mainGame);
            _thisScore!.LoadLists(_mainGame.PlayerList!); // i think


            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
             //hopefully can do this way to speed up loading.
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
