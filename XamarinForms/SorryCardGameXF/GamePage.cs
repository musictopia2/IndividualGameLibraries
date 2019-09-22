using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.GamePieces;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ChooserClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.GamePieces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using SorryCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace SorryCardGameXF
{
    public class GamePage : MultiPlayerPage<SorryCardGameViewModel, SorryCardGamePlayerItem, SorryCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            SorryCardGameSaveInfo saveRoot = OurContainer!.Resolve<SorryCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _thisColor!.LoadLists(ThisMod.ColorChooser!);
            _otherPileWPF!.Init(ThisMod.Pile2!, "");
            _otherPileWPF.StartAnimationListener("otherpile");
            LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            SorryCardGameSaveInfo saveRoot = OurContainer!.Resolve<SorryCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _otherPileWPF!.UpdatePile(ThisMod.Pile2!);
            LoadBoard();
            return Task.CompletedTask;
        }
        private void LoadBoard()
        {
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
            _boardStack!.Children.Clear();
            _boardStack.Children.Add(tempStack);
            int x = 0;
            thisList.ForEach(thisPlayer =>
            {
                x++;
                var thisControl = new BoardXF();
                thisControl.LoadList(thisPlayer);
                if (x == 3)
                {
                    tempStack = new StackLayout();
                    tempStack.Orientation = StackOrientation.Horizontal;
                    _boardStack.Children.Add(tempStack);
                }
                tempStack.Children.Add(thisControl);
            });
        }
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerXF<PawnPiecesCP<EnumColorChoices>, PawnPiecesXF<EnumColorChoices>, EnumColorChoices, ColorListChooser<EnumColorChoices>>();
            _chooseColorStack = new StackLayout();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGridXF colorTurn = new SimpleLabelGridXF();
            colorTurn.AddRow("Turn", nameof(SorryCardGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(SorryCardGameViewModel.Instructions));
            Binding thisBind = new Binding(nameof(SorryCardGameViewModel.ColorVisible));
            _chooseColorStack.SetBinding(IsVisibleProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        private BaseDeckXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>? _playerHand;
        private StackLayout? _chooseColorStack;
        private EnumPickerXF<PawnPiecesCP<EnumColorChoices>, PawnPiecesXF<EnumColorChoices>,
            EnumColorChoices, ColorListChooser<EnumColorChoices>>? _thisColor;
        private StackLayout? _boardStack;
        private BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>? _otherPileWPF;
        private SorryCardGameMainGameClass? _mainGame;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<SorryCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _otherPileWPF = new BasePileXF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsXF>();
            _boardStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _thisScore.AddColumn("Cards Left", true, nameof(SorryCardGamePlayerItem.ObjectCount)); //very common.
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPileWPF);
            var tempLabel = GetDefaultLabel();
            tempLabel.FontSize = 35;
            tempLabel.FontAttributes = FontAttributes.Bold;
            tempLabel.SetBinding(Label.TextProperty, new Binding(nameof(SorryCardGameViewModel.UpTo)));
            otherStack.Children.Add(tempLabel);
            thisStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SorryCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SorryCardGameViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SorryCardGameViewModel.Instructions));
            otherStack.Children.Add(_playerHand);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_boardStack);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            _otherPileWPF.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SorryCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<SorryCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<SorryCardGamePlayerItem, SorryCardGameSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<SorryCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, SorryCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterType<StandardPickerSizeClass>();
        }
    }
}