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
using SorryCardGameCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using SorryCardGameCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using SorryCardGameCP.Cards;

namespace SorryCardGameXF.Views
{
    public class SorryCardGameMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly SorryCardGameVMData _model;
        private readonly SorryCardGameGameContainer _gameContainer;
        private readonly BaseDeckXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly StackLayout _boardStack;
        private readonly BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF> _otherPileXF;
        private readonly CustomBasicList<BoardXF> _boardList = new CustomBasicList<BoardXF>();

        public SorryCardGameMainView(IEventAggregator aggregator,
            TestOptions test,
            SorryCardGameVMData model,
            SorryCardGameGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _deckGPile = new BaseDeckXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _boardStack = new StackLayout();
            _otherPileXF = new BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _boardList = new CustomBasicList<BoardXF>();

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(SorryCardGameMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_score);
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPileXF);
            var tempLabel = GetDefaultLabel();
            tempLabel.FontSize = 35;
            tempLabel.FontAttributes = FontAttributes.Bold;
            tempLabel.SetBinding(Label.TextProperty, new Binding(nameof(SorryCardGameMainViewModel.UpTo)));
            otherStack.Children.Add(tempLabel);
            mainStack.Children.Add(otherStack);

            _score.AddColumn("Cards Left", false, nameof(SorryCardGamePlayerItem.ObjectCount)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SorryCardGameMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SorryCardGameMainViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SorryCardGameMainViewModel.Instructions));

            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_playerHandWPF);
            otherStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(otherStack);
            mainStack.Children.Add(_boardStack);



            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _otherPileXF.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }


        private void LoadBoard()
        {
            var thisList = _gameContainer!.PlayerList!.GetAllPlayersStartingWithSelf();
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            _boardStack!.Children.Clear();
            _boardStack.Children.Add(tempStack);
            int x = 0;
            thisList.ForEach(thisPlayer =>
            {
                x++;
                var thisControl = new BoardXF();
                thisControl.LoadList(thisPlayer, _gameContainer.Command);
                if (x == 3)
                {
                    tempStack = new StackLayout();
                    tempStack.Orientation = StackOrientation.Horizontal;
                    _boardStack.Children.Add(tempStack);
                }
                tempStack.Children.Add(thisControl);
            });
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            SorryCardGameSaveInfo save = cons!.Resolve<SorryCardGameSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _otherPileXF!.Init(_model.OtherPile!, "");
            _otherPileXF.StartAnimationListener("otherpile");
            LoadBoard();

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
            _playerHandWPF.Dispose(); //at least will help improve performance.
            _boardList.ForEach(x => x.Dispose());
            return Task.CompletedTask;
        }
    }
}
