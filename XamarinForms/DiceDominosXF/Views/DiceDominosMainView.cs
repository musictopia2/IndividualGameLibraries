using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using static BasicXFControlsAndPages.Helpers.GridHelper;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using DiceDominosCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using DiceDominosCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.GameGraphics.Dominos;
using BasicGameFrameworkLibrary.Dominos;
using ts = BasicGameFrameworkLibrary.GameGraphicsCP.Dominos.DominosCP;
using DiceDominosCP.Logic;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;

namespace DiceDominosXF.Views
{
    public class DiceDominosMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly DiceDominosVMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        private readonly CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>> _gameBoard1;
        private readonly GameBoardCP _gameBoard;
        public DiceDominosMainView(IEventAggregator aggregator,
            TestOptions test, DiceDominosVMData model,
            GameBoardCP gameBoard
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            _gameBoard = gameBoard;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(DiceDominosMainViewModel.RestoreScreen));
            }
            _gameBoard1 = new CardBoardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            mainStack.Children.Add(_gameBoard1);

            var thisRoll = GetGamingButton("Roll Dice", nameof(DiceDominosMainViewModel.RollDiceAsync));
            thisRoll.VerticalOptions = LayoutOptions.Start;
            thisRoll.HorizontalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            var endButton = GetGamingButton("End Turn", nameof(DiceDominosMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardXF();
            _score.AddColumn("Dominos Won", true, nameof(DiceDominosPlayerItem.DominosWon), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DiceDominosMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Status", nameof(DiceDominosMainViewModel.Status));

            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                otherStack.Children.Add(_score);
                otherStack.Children.Add(firstInfo.GetContent);
                otherStack.Children.Add(thisRoll);
                otherStack.Children.Add(endButton);
                otherStack.Children.Add(_diceControl);
            }
            else
            {
                otherStack.Children.Add(thisRoll);
                otherStack.Children.Add(endButton);
                otherStack.Children.Add(_diceControl);
                mainStack.Children.Add(firstInfo.GetContent);
                mainStack.Children.Add(_score);
            }

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            DiceDominosSaveInfo save = cons!.Resolve<DiceDominosSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            _gameBoard1!.LoadList(_gameBoard, ts.TagUsed);
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
