using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using MilkRunCP;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace MilkRunWPF
{
    public class GamePage : MultiPlayerWindow<MilkRunViewModel, MilkRunPlayerItem, MilkRunSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            FirstHookUp();
            _yourChocolatePiles!.Init(_mainGame!.SingleInfo!.ChocolatePiles!, "");
            _yourStrawberryPiles!.Init(_mainGame.SingleInfo.StrawberryPiles!, "");
            _yourChocolatePiles.StartAnimationListener(EnumMilkType.Chocolate + _mainGame.SingleInfo.NickName);
            _yourStrawberryPiles.StartAnimationListener(EnumMilkType.Strawberry + _mainGame.SingleInfo.NickName);
            _opponentChocolatePiles!.Init(_otherPlayer!.ChocolatePiles!, "");
            _opponentStrawberryPiles!.Init(_otherPlayer!.StrawberryPiles!, "");
            _opponentChocolatePiles.StartAnimationListener(EnumMilkType.Chocolate + _otherPlayer.NickName);
            _opponentStrawberryPiles.StartAnimationListener(EnumMilkType.Strawberry + _otherPlayer.NickName);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            FirstHookUp();
            _yourChocolatePiles!.UpdateLists(_mainGame!.SingleInfo!.ChocolatePiles!);
            _yourStrawberryPiles!.UpdateLists(_mainGame.SingleInfo.StrawberryPiles!);
            _opponentChocolatePiles!.UpdateLists(_otherPlayer!.ChocolatePiles!);
            _opponentStrawberryPiles!.UpdateLists(_otherPlayer.StrawberryPiles!);
            return Task.CompletedTask;
        }
        private void FirstHookUp()
        {
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetSelf();
            _yourChocolateDeliveries!.DataContext = _mainGame.SingleInfo;
            _yourStrawberryDeliveries!.DataContext = _mainGame.SingleInfo;
            _otherPlayer = _mainGame.PlayerList.Single(items => items.PlayerCategory != BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Self);
            _opponentChocolateDeliveries!.DataContext = _otherPlayer;
            _opponentStrawberryDeliveries!.DataContext = _otherPlayer;
        }
        private BaseDeckWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private BaseHandWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _opponentChocolatePiles;
        private BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _opponentStrawberryPiles;
        private BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _yourChocolatePiles;
        private BasicMultiplePilesWPF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsWPF>? _yourStrawberryPiles;
        private TextBlock? _opponentStrawberryDeliveries;
        private TextBlock? _opponentChocolateDeliveries;
        private TextBlock? _yourStrawberryDeliveries;
        private TextBlock? _yourChocolateDeliveries;
        private MilkRunMainGameClass? _mainGame;
        private MilkRunPlayerItem? _otherPlayer;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<MilkRunMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
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
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddPlayArea(otherStack);
            thisStack.Children.Add(otherStack);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(MilkRunViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MilkRunViewModel.Status));
            StackPanel newStack = new StackPanel();
            thisStack.Children.Add(newStack);
            newStack.HorizontalAlignment = HorizontalAlignment.Center;
            newStack.Margin = new Thickness(0, 10, 0, 0);
            newStack.Children.Add(_playerHandWPF);
            newStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _deckGPile.VerticalAlignment = VerticalAlignment.Top;
            _discardGPile.VerticalAlignment = VerticalAlignment.Top;
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
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
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<MilkRunViewModel>();
            OurContainer!.RegisterType<DeckViewModel<MilkRunCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<MilkRunPlayerItem, MilkRunSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<MilkRunCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, MilkRunDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
        }
    }
}