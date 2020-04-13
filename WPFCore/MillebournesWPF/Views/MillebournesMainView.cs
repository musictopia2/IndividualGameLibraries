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
using System.Windows.Controls;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.BasicEventModels;
using MillebournesCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using MillebournesCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using MillebournesCP.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using System.Windows.Media;
using MillebournesCP.Logic;

namespace MillebournesWPF.Views
{
    public class MillebournesMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly MillebournesVMData _model;
        private readonly MillebournesMainGameClass _mainGame;
        private readonly MillebournesGameContainer _gameContainer;
        private readonly BaseDeckWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly ScoreBoardWPF _score;
        private readonly BaseHandWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF> _newCard;
        private readonly StackPanel _pileStack;
        private readonly CustomBasicList<BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>> _disList = new CustomBasicList<BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>>();



        public MillebournesMainView(IEventAggregator aggregator,
            MillebournesVMData model,
            MillebournesMainGameClass mainGame,
            MillebournesGameContainer gameContainer,
            TestOptions test
            )
        {
            _aggregator = aggregator;
            _model = model;
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _score = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            StackPanel mainStack = new StackPanel();
            _newCard = new BasePileWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
            _pileStack = new StackPanel();
            StackPanel summaryStack = new StackPanel();
            summaryStack.Orientation = Orientation.Horizontal;
            summaryStack.Children.Add(mainStack);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_newCard);
            //var thisBut = GetGamingButton("Coupe Foure", nameof(MillebournesViewModel.CoupeCommand));
            // var thisBind = GetVisibleBinding(nameof(MillebournesViewModel.CoupeVisible));
            //thisBut.SetBinding(VisibilityProperty, thisBind);

            ParentSingleUIContainer parent = new ParentSingleUIContainer()
            {
                Name = nameof(MillebournesMainViewModel.CoupeScreen)
            };
            parent.HorizontalAlignment = HorizontalAlignment.Left;
            parent.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(parent);
            mainStack.Children.Add(otherStack);
            _score.AddColumn("Team", true, nameof(MillebournesPlayerItem.Team), rightMargin: 10);
            _score.AddColumn("Miles", true, nameof(MillebournesPlayerItem.Miles), rightMargin: 10);
            _score.AddColumn("Other Points", true, nameof(MillebournesPlayerItem.OtherPoints), rightMargin: 10);
            _score.AddColumn("Total Points", true, nameof(MillebournesPlayerItem.TotalPoints), rightMargin: 10);
            _score.AddColumn("# 200s", true, nameof(MillebournesPlayerItem.Number200s), rightMargin: 10);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MillebournesMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MillebournesMainViewModel.Status));
            mainStack.Children.Add(_playerHandWPF);
            mainStack.Children.Add(firstInfo.GetContent);
            mainStack.Children.Add(_score);
            summaryStack.Children.Add(_pileStack);

            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _newCard.Margin = new Thickness(5, 5, 5, 5);
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(MillebournesMainViewModel.RestoreScreen)
                };
            }

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = summaryStack;

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
            _mainGame!.SingleInfo = _gameContainer.PlayerList!.GetSelf();
            if (_gameContainer.SingleInfo!.MainHandList.Any(items => items.CompleteCategory == EnumCompleteCategories.None))
                throw new BasicBlankException("Cannot have category of none.  Rethink");
            _pileStack!.Children.Clear();
            _gameContainer.TeamList.ForEach(thisTeam =>
            {
                Grid tempGrid = new Grid();
                tempGrid.Margin = new Thickness(0, 0, 0, 20);
                AddAutoRows(tempGrid, 2);
                AddAutoColumns(tempGrid, 2);
                TextBlock ThisLabel = new TextBlock();
                ThisLabel.Text = thisTeam.Text;
                ThisLabel.Foreground = Brushes.Aqua;
                ThisLabel.FontWeight = FontWeights.Bold;
                AddControlToGrid(tempGrid, ThisLabel, 0, 0);
                Grid.SetColumnSpan(ThisLabel, 2);
                ThisLabel.HorizontalAlignment = HorizontalAlignment.Center; // try this
                BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF> thisDis = new BasicMultiplePilesWPF<MillebournesCardInformation, MillebournesGraphicsCP, CardGraphicsWPF>();
                thisDis.Init(thisTeam.CardPiles, "");
                thisDis.StartAnimationListener("team" + thisTeam.TeamNumber);
                AddControlToGrid(tempGrid, thisDis, 1, 0);
                SafetiesWPF thisS = new SafetiesWPF();
                thisS.Init(thisTeam, _mainGame, _gameContainer.Command);
                _disList.Add(thisDis);
                AddControlToGrid(tempGrid, thisS, 1, 1);
                _pileStack.Children.Add(tempGrid);
            });
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
