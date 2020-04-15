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
using MilkRunCP.Data;
using Xamarin.Forms;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicGameFrameworkLibrary.TestUtilities;
using MilkRunCP.ViewModels;
using BasicGamingUIXFLibrary.Helpers;
using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicGamingUIXFLibrary.GameGraphics.Cards;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using MilkRunCP.Cards;
using BasicGamingUIXFLibrary.BasicControls.MultipleFrameContainers;

namespace MilkRunXF.Views
{
    public class MilkRunMainView : ContentView, IUIView, IHandleAsync<LoadEventModel>
    {
        //TODO:  has to build the main page.  no need for new game since that is completely separated out into another class.
        private readonly IEventAggregator _aggregator;
        private readonly MilkRunVMData _model;
        private readonly MilkRunGameContainer _gameContainer;
        private readonly BaseDeckXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _deckGPile;
        private readonly BasePileXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _discardGPile;

        private readonly BaseHandXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _playerHandWPF;

        private readonly BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _opponentChocolatePiles;
        private readonly BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _opponentStrawberryPiles;
        private readonly BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _yourChocolatePiles;
        private readonly BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF> _yourStrawberryPiles;
        private Label _opponentStrawberryDeliveries;
        private Label _opponentChocolateDeliveries;
        private Label _yourStrawberryDeliveries;
        private Label _yourChocolateDeliveries;
        private MilkRunPlayerItem? _otherPlayer;

        public MilkRunMainView(IEventAggregator aggregator,
            TestOptions test,
            MilkRunVMData model,
            MilkRunGameContainer gameContainer
            )
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);
            _gameContainer = gameContainer;
            _deckGPile = new BaseDeckXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _playerHandWPF = new BaseHandXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();

            _opponentChocolateDeliveries = new Label();
            _opponentChocolatePiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _opponentStrawberryDeliveries = new Label();
            _opponentStrawberryPiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _yourChocolateDeliveries = new Label();
            _yourChocolatePiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _yourStrawberryDeliveries = new Label();
            _yourStrawberryPiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();


            StackLayout mainStack = new StackLayout();
            ParentSingleUIContainer? restoreP = null;
            if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
            {
                restoreP = new ParentSingleUIContainer(nameof(MilkRunMainViewModel.RestoreScreen));
            }

            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddPlayArea(otherStack);
            mainStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MilkRunMainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MilkRunMainViewModel.Status));


            StackLayout newStack = new StackLayout();
            mainStack.Children.Add(newStack);
            newStack.HorizontalOptions = LayoutOptions.Center;
            newStack.Margin = new Thickness(0, 10, 0, 0);
            newStack.Children.Add(_playerHandWPF);
            newStack.Children.Add(firstInfo.GetContent);



            _deckGPile.Margin = new Thickness(5, 5, 5, 5);

            _discardGPile.Margin = new Thickness(5, 5, 5, 5);

            if (restoreP != null)
            {
                //todo:  figure out where to place the restore ui if there is a restore option.
                mainStack.Children.Add(restoreP); //default add to grid but does not have to.
            }
            Content = mainStack;
        }
        private void AddPlayArea(StackLayout otherStack)
        {
            Grid thisGrid = new Grid();
            AddAutoColumns(thisGrid, 2);
            AddAutoRows(thisGrid, 6);
            var thisLabel = GetDefaultLabel();
            thisLabel.Text = "Opponent:";
            thisLabel.HorizontalOptions = LayoutOptions.Center;
            thisLabel.Margin = new Thickness(0, 0, 0, 10);
            AddControlToGrid(thisGrid, thisLabel, 0, 0);
            Grid.SetColumnSpan(thisLabel, 2);
            AddControlToGrid(thisGrid, _opponentStrawberryPiles!, 1, 0);
            _opponentStrawberryPiles!.Margin = new Thickness(0, 0, 20, 20);
            AddControlToGrid(thisGrid, _opponentChocolatePiles!, 1, 1);
            StackLayout thisStack;
            thisStack = GetLabelGroup(EnumMilkType.Strawberry, true);
            AddControlToGrid(thisGrid, thisStack, 2, 0);
            thisStack = GetLabelGroup(EnumMilkType.Chocolate, true);
            AddControlToGrid(thisGrid, thisStack, 2, 1);
            thisLabel = GetDefaultLabel();
            thisLabel.Text = "Yours:";
            thisLabel.Margin = new Thickness(0, 20, 0, 10);
            thisLabel.HorizontalOptions = LayoutOptions.Center;
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
        private StackLayout GetLabelGroup(EnumMilkType milk, bool isOpponent)
        {
            StackLayout thisStack = new StackLayout();
            thisStack.Orientation = StackOrientation.Horizontal;
            thisStack.HorizontalOptions = LayoutOptions.Center;
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
            thisLabel.SetBinding(Label.TextProperty, new Binding(path));
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
        private Label GetSpecialLabel(EnumMilkType milk)
        {
            Label thisText = new Label();
            thisText.FontAttributes = FontAttributes.Bold;
            if (milk == EnumMilkType.Chocolate)
                thisText.TextColor = Color.Chocolate;
            else
                thisText.TextColor = Color.DeepPink;
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
            _yourChocolateDeliveries!.BindingContext = _gameContainer.SingleInfo;
            _yourStrawberryDeliveries!.BindingContext = _gameContainer.SingleInfo;
            _otherPlayer = _gameContainer.PlayerList.GetOnlyOpponent();
            _opponentChocolateDeliveries!.BindingContext = _otherPlayer;
            _opponentStrawberryDeliveries!.BindingContext = _otherPlayer;
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
