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
using SinisterSixCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using SinisterSixCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;

namespace SinisterSixXF.Views
{
    public class SinisterSixMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly SinisterSixVMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<EightSidedDice> _diceControl; //i think.
        public SinisterSixMainView(IEventAggregator aggregator,
            TestOptions test, SinisterSixVMData model
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SinisterSixMainViewModel.RestoreScreen));
            }


            var thisRoll = GetGamingButton("Roll Dice", nameof(SinisterSixMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<EightSidedDice>();
            otherStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetGamingButton("End Turn", nameof(SinisterSixMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            mainStack.Children.Add(endButton);
            mainStack.Children.Add(otherStack);

            var otherButton = GetGamingButton("Remove Selected Dice", nameof(SinisterSixMainViewModel.RemoveDiceAsync));
            otherButton.HorizontalOptions = LayoutOptions.Start;
            mainStack.Children.Add(otherButton);

            _score = new ScoreBoardXF();
            _score.AddColumn("Score", true, nameof(SinisterSixPlayerItem.Score));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SinisterSixMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(SinisterSixMainViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Status", nameof(SinisterSixMainViewModel.Status));




            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.
            SinisterSixSaveInfo save = cons!.Resolve<SinisterSixSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
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
