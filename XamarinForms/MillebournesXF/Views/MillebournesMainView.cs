using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.Helpers;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using MillebournesCP.Cards;
using MillebournesCP.Data;
using MillebournesCP.Logic;
using MillebournesCP.ViewModels;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace MillebournesXF.Views
{
    public class MillebournesMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly MillebournesVMData _model;
        private readonly MillebournesMainGameClass _mainGame;
        private readonly MillebournesGameContainer _gameContainer;
        private readonly BaseDeckXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly ScoreBoardXF _score;
        private readonly BaseHandXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF> _newCard;
        private readonly Grid? _pileGrid;
        private readonly CustomBasicList<BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>> _disList = new CustomBasicList<BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>>();


        public MillebournesMainView(IEventAggregator aggregator,
            TestOptions test,
            MillebournesVMData model,
            MillebournesMainGameClass mainGame,
            MillebournesGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _score = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _newCard = new BasePileXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
            _pileGrid = new Grid();

            ScrollView thisScroll = new ScrollView();
            thisScroll.Orientation = ScrollOrientation.Vertical;

            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(MillebournesMainViewModel.RestoreScreen));
            }

            StackLayout summaryStack = new StackLayout();
            summaryStack.Orientation = StackOrientation.Horizontal;
            if (ScreenUsed == EnumScreen.SmallPhone)
                thisScroll.Content = summaryStack;
            summaryStack.Children.Add(mainStack);


            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_newCard);
            ParentSingleUIContainer parent = new ParentSingleUIContainer(nameof(MillebournesMainViewModel.CoupeScreen));
            parent.HorizontalOptions = LayoutOptions.Start;
            parent.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(parent);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Team", true, nameof(MillebournesPlayerItem.Team), rightMargin: 10);
            _score.AddColumn("Miles", true, nameof(MillebournesPlayerItem.Miles), rightMargin: 10);
            _score.AddColumn("Other Points", true, nameof(MillebournesPlayerItem.OtherPoints), rightMargin: 10);
            _score.AddColumn("Total Points", true, nameof(MillebournesPlayerItem.TotalPoints), rightMargin: 10);
            _score.AddColumn("# 200s", true, nameof(MillebournesPlayerItem.Number200s), rightMargin: 10);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MillebournesMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MillebournesMainViewModel.Status));

            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);
            _pileGrid = new Grid();
            AddAutoColumns(_pileGrid, 1);
            3.Times(x =>
            {
                AddLeftOverRow(_pileGrid, 1);
            });
            summaryStack.Children.Add(_pileGrid);



            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _newCard.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }

            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                Content = thisScroll;
            }
            else
            {
                Content = summaryStack;
            }

        }
        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.

            MillebournesSaveInfo save = cons!.Resolve<MillebournesSaveInfo>(); //usually needs this part for multiplayer games.

            _score!.LoadLists(save.PlayerList);
            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile2!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            _newCard!.Init(_model.Pile1!, "");
            _disList.ForEach(thisD => thisD.Unregister());
            _disList.Clear();
            SetUpTeamPiles();


            return this.RefreshBindingsAsync(_aggregator);
        }

        private void SetUpTeamPiles()
        {
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetSelf();
            if (_mainGame.SingleInfo.MainHandList.Any(items => items.CompleteCategory == EnumCompleteCategories.None))
                throw new BasicBlankException("Cannot have category of none.  Rethink");
            _pileGrid!.Children.Clear();
            int x = 0;
            _gameContainer.TeamList.ForEach(thisTeam =>
            {
                x++;
                Grid tempGrid = new Grid();
                tempGrid.Margin = new Thickness(0, 0, 0, 5);
                Label ThisLabel = new Label();
                if (ScreenUsed == EnumScreen.SmallPhone)
                {
                    ThisLabel.FontSize = 8;
                    AddPixelRow(tempGrid, 12);
                    AddPixelRow(tempGrid, 68);
                }
                else
                {
                    AddAutoRows(tempGrid, 1);
                    AddLeftOverRow(tempGrid, 1);
                    if (ScreenUsed == EnumScreen.SmallTablet)
                        ThisLabel.FontSize = 12;
                    else
                        ThisLabel.FontSize = 20;
                }
                AddAutoColumns(tempGrid, 2);
                ThisLabel.Text = thisTeam.Text;
                ThisLabel.TextColor = Color.Aqua;
                ThisLabel.FontAttributes = FontAttributes.Bold;
                AddControlToGrid(tempGrid, ThisLabel, 0, 0);
                ThisLabel.HorizontalOptions = LayoutOptions.Center; // try this
                BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF> thisDis = new BasicMultiplePilesXF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsXF>();
                thisDis.Init(thisTeam.CardPiles, "");
                thisDis.StartAnimationListener("team" + thisTeam.TeamNumber);
                AddControlToGrid(tempGrid, thisDis, 1, 0);
                SafetiesXF thisS = new SafetiesXF();
                thisS.Init(thisTeam, _mainGame, _gameContainer.Command);
                _disList.Add(thisDis);
                if (x == 1)
                {
                    AddControlToGrid(tempGrid, thisS, 0, 1);
                    Grid.SetRowSpan(thisS, 2);
                }
                else
                    AddControlToGrid(tempGrid, thisS, 1, 1);
                AddControlToGrid(_pileGrid, tempGrid, x - 1, 0);
            });
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
            return Task.CompletedTask;
        }
    }
}
