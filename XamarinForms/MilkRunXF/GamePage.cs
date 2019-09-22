using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace MilkRunXF
{
    public class GamePage : MultiPlayerPage<MilkRunViewModel, MilkRunPlayerItem, MilkRunSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
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
            _yourChocolateDeliveries!.BindingContext = _mainGame.SingleInfo;
            _yourStrawberryDeliveries!.BindingContext = _mainGame.SingleInfo;
            _otherPlayer = _mainGame.PlayerList.Single(items => items.PlayerCategory != BasicGameFramework.MultiplayerClasses.BasicPlayerClasses.EnumPlayerCategory.Self);
            _opponentChocolateDeliveries!.BindingContext = _otherPlayer;
            _opponentStrawberryDeliveries!.BindingContext = _otherPlayer;
        }
        private BaseDeckXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _discardGPile;
        private BaseHandXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _playerHand;
        private BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _opponentChocolatePiles;
        private BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _opponentStrawberryPiles;
        private BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _yourChocolatePiles;
        private BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>? _yourStrawberryPiles;
        private Label? _opponentStrawberryDeliveries;
        private Label? _opponentChocolateDeliveries;
        private Label? _yourStrawberryDeliveries;
        private Label? _yourChocolateDeliveries;
        private MilkRunMainGameClass? _mainGame;
        private MilkRunPlayerItem? _otherPlayer;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<MilkRunMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _playerHand = new BaseHandXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _opponentChocolateDeliveries = new Label();
            _opponentStrawberryDeliveries = new Label();
            _opponentChocolatePiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _opponentStrawberryPiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _yourChocolateDeliveries = new Label();
            _yourStrawberryDeliveries = new Label();
            _yourChocolatePiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            _yourStrawberryPiles = new BasicMultiplePilesXF<MilkRunCardInformation, MilkRunGraphicsCP, CardGraphicsXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddPlayArea(otherStack);
            thisStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MilkRunViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MilkRunViewModel.Status));
            StackLayout newStack = new StackLayout();
            thisStack.Children.Add(newStack);
            newStack.HorizontalOptions = LayoutOptions.Center;
            newStack.Margin = new Thickness(0, 10, 0, 0);
            newStack.Children.Add(_playerHand);
            newStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
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
                thisText.TextColor = Color.DeepPink;// to represent strawberry
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