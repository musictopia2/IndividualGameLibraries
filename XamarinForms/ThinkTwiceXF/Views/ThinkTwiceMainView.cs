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
using ThinkTwiceCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using ThinkTwiceCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.Misc;
using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using ThinkTwiceCP.Logic;

namespace ThinkTwiceXF.Views
{
    public class ThinkTwiceMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly ThinkTwiceVMData _model;
        readonly ScoreBoardXF _score;
        readonly DiceListControlXF<SimpleDice> _diceControl; //i think.
        private readonly IGamePackageResolver _resolver;
        private readonly ThinkTwiceGameContainer _gameContainer;
        private readonly StackLayout _multStack;

        public ThinkTwiceMainView(IEventAggregator aggregator,
            TestOptions test, ThinkTwiceVMData model,
            IGamePackageResolver resolver,
            ThinkTwiceGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _aggregator.Subscribe(this);
            _model = model;
            _resolver = resolver;
            _gameContainer = gameContainer;
            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(ThinkTwiceMainViewModel.ScoreScreen));
            _multStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal
            };

            if (ScreenUsed != EnumScreen.SmallPhone)
            {
                mainStack.Children.Add(parent);
                mainStack.Children.Add(_multStack);
            }
            else
            {
                StackLayout tempStack = new StackLayout();
                _multStack.VerticalOptions = LayoutOptions.End;
                tempStack.Orientation = StackOrientation.Horizontal;
                tempStack.Children.Add(parent);
                tempStack.Children.Add(_multStack);
                mainStack.Children.Add(tempStack);
                _multStack.Spacing = 2;
            }
            StackLayout firsts = new StackLayout();

            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(ThinkTwiceMainViewModel.RestoreScreen));
            }


            var thisRoll = GetSmallerButton("Roll Dice", nameof(ThinkTwiceMainViewModel.RollDiceAsync));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _diceControl = new DiceListControlXF<SimpleDice>();
            mainStack.Children.Add(_diceControl);
            otherStack.Children.Add(thisRoll);
            var endButton = GetSmallerButton("End Turn", nameof(ThinkTwiceMainViewModel.EndTurnAsync));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            var thisBut = GetSmallerButton("Roll Multiplier Dice", nameof(ThinkTwiceMainViewModel.RollMultAsync));
            otherStack.Children.Add(thisBut);

            mainStack.Children.Add(otherStack);
            _score = new ScoreBoardXF();
            _score.AddColumn("Score Round", true, nameof(ThinkTwicePlayerItem.ScoreRound));
            _score.AddColumn("Score Game", true, nameof(ThinkTwicePlayerItem.ScoreGame));


            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ThinkTwiceMainViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(ThinkTwiceMainViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Category", nameof(ThinkTwiceMainViewModel.CategoryChosen));
            firstInfo.AddRow("Score", nameof(ThinkTwiceMainViewModel.Score));


            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);

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
            ThinkTwiceSaveInfo save = cons!.Resolve<ThinkTwiceSaveInfo>(); //usually needs this part for multiplayer games.
            _score!.LoadLists(save.PlayerList);
            _diceControl!.LoadDiceViewModel(_model.Cup!);
            CategoriesDice thisCat = _resolver.Resolve<CategoriesDice>(); //i think
            Multiplier thisMul = _resolver.Resolve<Multiplier>();
            CategoryXF first = new CategoryXF();
            first.SendDiceInfo(thisCat, _gameContainer);
            first.Margin = new Thickness(0); //well see on tablets.
            _multStack!.Children.Add(first);
            MultXF second = new MultXF();
            second.SendDiceInfo(thisMul);
            second.Margin = new Thickness(0);
            _multStack.Children.Add(second);


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
