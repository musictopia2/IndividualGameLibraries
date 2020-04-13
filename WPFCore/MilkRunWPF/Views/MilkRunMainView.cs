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
using MilkRunCP.Data;
using BasicGameFrameworkLibrary.TestUtilities;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using MilkRunCP.ViewModels;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using BasicGamingUIWPFLibrary.Helpers;
using System.Windows;
using BasicGamingUIWPFLibrary.BasicControls.SingleCardFrames;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.GameGraphics.Cards;
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using MilkRunCP.Cards;
using BasicGamingUIWPFLibrary.BasicControls.MultipleFrameContainers;
using System.Windows.Data;
using System.Windows.Media;

namespace MilkRunWPF.Views
{
    public class MilkRunMainView : UserControl, IUIView, IHandleAsync<LoadEventModel>
    {
        private readonly IEventAggregator _aggregator;
        private readonly MilkRunVMData _model;
        private readonly MilkRunGameContainer _gameContainer;
        private readonly BaseDeckWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _deckGPile;
        private readonly BasePileWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _discardGPile;

        private readonly BaseHandWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _playerHandWPF;

        private readonly BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _opponentChocolatePiles;
        private readonly BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _opponentStrawberryPiles;
        private readonly BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _yourChocolatePiles;
        private readonly BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF> _yourStrawberryPiles;
        private  TextBlock _opponentStrawberryDeliveries;
        private TextBlock _opponentChocolateDeliveries;
        private TextBlock _yourStrawberryDeliveries;
        private TextBlock _yourChocolateDeliveries;
        private MilkRunPlayerItem? _otherPlayer;

        public MilkRunMainView(IEventAggregator aggregator,
            TestOptions test,
            MilkRunVMData model,
            MilkRunGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _gameContainer = gameContainer;
            _aggregator.Subscribe(this);

            _deckGPile = new BaseDeckWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();
            _playerHandWPF = new BaseHandWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();

            _opponentChocolateDeliveries = new TextBlock();
            _opponentStrawberryDeliveries = new TextBlock();
            _opponentChocolatePiles = new BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();
            _opponentStrawberryPiles = new BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();
            _yourChocolateDeliveries = new TextBlock();
            _yourStrawberryDeliveries = new TextBlock();
            _yourChocolatePiles = new BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();
            _yourStrawberryPiles = new BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>();


            StackPanel mainStack = new StackPanel();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer()
                {
                    Name = nameof(MilkRunMainViewModel.RestoreScreen)
                };
            }


            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddPlayArea(otherStack);
            mainStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MilkRunMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MilkRunMainViewModel.Status));

            StackPanel newStack = new StackPanel();
            mainStack.Children.Add(newStack);
            newStack.HorizontalAlignment = HorizontalAlignment.Center;
            newStack.Margin = new Thickness(0, 10, 0, 0);
            newStack.Children.Add(_playerHandWPF);
            newStack.Children.Add(firstInfo.GetContent);


            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);


            if (restoreP != null)
            {
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;

        }

        private void AddPlayArea(StackPanel otherStack)
        {
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2);
            AddAutoRows(thisGrid, 6);
            var thisLabel = GetDefaultLabel();
            thisLabel.Text = "Opponent:";
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            thisLabel.Margin = new Thickness(0, 0, 0, 10);
            AddControlToGrid(thisGrid, thisLabel, 0, 0);
            Grid.SetColumnSpan(thisLabel, 2);
            AddControlToGrid(thisGrid, _opponentStrawberryPiles!, 1, 0);
            _opponentStrawberryPiles!.Margin = new Thickness(0, 0, 20, 20);
            AddControlToGrid(thisGrid, _opponentChocolatePiles!, 1, 1);
            StackPanel thisStack;
            thisStack = GetLabelGroup(EnumMilkType.Strawberry, true);
            AddControlToGrid(thisGrid, thisStack, 2, 0);
            thisStack = GetLabelGroup(EnumMilkType.Chocolate, true);
            AddControlToGrid(thisGrid, thisStack, 2, 1);
            thisLabel = GetDefaultLabel();
            thisLabel.Text = "Yours:";
            thisLabel.Margin = new Thickness(0, 20, 0, 10);
            thisLabel.HorizontalAlignment = HorizontalAlignment.Center;
            AddControlToGrid(thisGrid, thisLabel, 3, 0);
            Grid.SetColumnSpan(thisLabel, 2);
            AddControlToGrid(thisGrid, _yourStrawberryPiles!, 4, 0);
            _yourStrawberryPiles!.Margin = new Thickness(0, 0, 20, 20);
            AddControlToGrid(thisGrid, _yourChocolatePiles!, 4, 1);
            thisStack = GetLabelGroup(EnumMilkType.Strawberry, false);
            AddControlToGrid(thisGrid, thisStack, 5, 0);
            thisStack = GetLabelGroup(EnumMilkType.Chocolate, false);
            AddControlToGrid(thisGrid, thisStack, 5, 1);
            otherStack.Children.Add(thisGrid);
        }
        private StackPanel GetLabelGroup(EnumMilkType milk, bool isOpponent)
        {
            StackPanel thisStack = new StackPanel();
            thisStack.Orientation = Orientation.Horizontal;
            thisStack.HorizontalAlignment = HorizontalAlignment.Center;
            var thisLabel = GetSpecialLabel(milk);
            if (milk == EnumMilkType.Chocolate)
                thisLabel.Text = "Chocolate Deliveries ";
            else
                thisLabel.Text = "Strawberry Deliveries ";
            thisStack.Children.Add(thisLabel);
            thisLabel = GetSpecialLabel(milk);
            string path;
            if (milk == EnumMilkType.Chocolate)
                path = nameof(MilkRunPlayerItem.ChocolateDeliveries);
            else
                path = nameof(MilkRunPlayerItem.StrawberryDeliveries);
            thisLabel.SetBinding(TextBlock.TextProperty, new Binding(path));
            thisStack.Children.Add(thisLabel);
            if (isOpponent == true && milk == EnumMilkType.Chocolate)
                _opponentChocolateDeliveries = thisLabel;
            else if (isOpponent == true && milk == EnumMilkType.Strawberry)
                _opponentStrawberryDeliveries = thisLabel;
            else if (isOpponent == false && milk == EnumMilkType.Chocolate)
                _yourChocolateDeliveries = thisLabel;
            else if (isOpponent == false && milk == EnumMilkType.Strawberry)
                _yourStrawberryDeliveries = thisLabel;
            else
                throw new Exception("Not supported");
            return thisStack;
        }
        private TextBlock GetSpecialLabel(EnumMilkType milk)
        {
            TextBlock thisText = new TextBlock();
            thisText.FontWeight = FontWeights.Bold;
            if (milk == EnumMilkType.Chocolate)
                thisText.Foreground = Brushes.Chocolate;
            else
                thisText.Foreground = Brushes.DeepPink;// to represent strawberry
            return thisText;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {

            GamePackageViewModelBinder.ManuelElements.Clear(); //often times i have to add manually.


            _playerHandWPF!.LoadList(_model.PlayerHand1!, ""); // i think
            _discardGPile!.Init(_model.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(_model.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();

            FirstHookUp();
            _yourChocolatePiles!.Init(_gameContainer!.SingleInfo!.ChocolatePiles!, "");
            _yourStrawberryPiles!.Init(_gameContainer.SingleInfo.StrawberryPiles!, "");
            _yourChocolatePiles.StartAnimationListener(EnumMilkType.Chocolate + _gameContainer.SingleInfo.NickName);
            _yourStrawberryPiles.StartAnimationListener(EnumMilkType.Strawberry + _gameContainer.SingleInfo.NickName);
            _opponentChocolatePiles!.Init(_otherPlayer!.ChocolatePiles!, "");
            _opponentStrawberryPiles!.Init(_otherPlayer!.StrawberryPiles!, "");
            _opponentChocolatePiles.StartAnimationListener(EnumMilkType.Chocolate + _otherPlayer.NickName);
            _opponentStrawberryPiles.StartAnimationListener(EnumMilkType.Strawberry + _otherPlayer.NickName);

            return this.RefreshBindingsAsync(_aggregator);
        }

        private void FirstHookUp()
        {
            _gameContainer!.SingleInfo = _gameContainer.PlayerList!.GetSelf();
            _yourChocolateDeliveries!.DataContext = _gameContainer.SingleInfo;
            _yourStrawberryDeliveries!.DataContext = _gameContainer.SingleInfo;
            _otherPlayer = _gameContainer.PlayerList.GetOnlyOpponent();
            _opponentChocolateDeliveries!.DataContext = _otherPlayer;
            _opponentStrawberryDeliveries!.DataContext = _otherPlayer;
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
