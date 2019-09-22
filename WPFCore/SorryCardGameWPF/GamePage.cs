using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.GamePieces;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace SorryCardGameWPF
{
    public class GamePage : MultiPlayerWindow<SorryCardGameViewModel, SorryCardGamePlayerItem, SorryCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            SorryCardGameSaveInfo saveRoot = OurContainer!.Resolve<SorryCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _otherPileWPF!.UpdatePile(ThisMod.Pile2!);
            LoadBoard();
            return Task.CompletedTask;
        }
        private void LoadBoard()
        {
            var thisList = _mainGame!.PlayerList!.GetAllPlayersStartingWithSelf();
            StackPanel tempStack = new StackPanel();
            tempStack.Orientation = Orientation.Horizontal;
            _boardStack!.Children.Clear();
            _boardStack.Children.Add(tempStack);
            int x = 0;
            thisList.ForEach(thisPlayer =>
            {
                x++;
                var thisControl = new BoardWPF();
                thisControl.LoadList(thisPlayer);
                if (x == 3)
                {
                    tempStack = new StackPanel();
                    tempStack.Orientation = Orientation.Horizontal;
                    _boardStack.Children.Add(tempStack);
                }
                tempStack.Children.Add(thisControl);
            });
        }
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _thisColor = new EnumPickerWPF<PawnPiecesCP<EnumColorChoices>, PawnPiecesWPF<EnumColorChoices>, EnumColorChoices
                , ColorListChooser<EnumColorChoices>>();
            _chooseColorStack = new StackPanel();
            thisGrid.Children.Add(_chooseColorStack);
            SimpleLabelGrid colorTurn = new SimpleLabelGrid();
            colorTurn.AddRow("Turn", nameof(SorryCardGameViewModel.NormalTurn));
            colorTurn.AddRow("Instructions", nameof(SorryCardGameViewModel.Instructions));
            Binding thisBind = GetVisibleBinding(nameof(SorryCardGameViewModel.ColorVisible));
            _chooseColorStack.SetBinding(VisibilityProperty, thisBind);
            _chooseColorStack.Children.Add(_thisColor);
            _chooseColorStack.Children.Add(colorTurn.GetContent);
        }
        private BaseDeckWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private StackPanel? _chooseColorStack;
        private EnumPickerWPF<PawnPiecesCP<EnumColorChoices>, PawnPiecesWPF<EnumColorChoices>,
            EnumColorChoices, ColorListChooser<EnumColorChoices>>? _thisColor;
        private StackPanel? _boardStack;
        private BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>? _otherPileWPF;
        private SorryCardGameMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<SorryCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();
            _otherPileWPF = new BasePileWPF<SorryCardGameCardInformation, SorryCardGameGraphicsCP, CardGraphicsWPF>();
            _boardStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            _thisScore.AddColumn("Cards Left", true, nameof(SorryCardGamePlayerItem.ObjectCount)); //very common.
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            otherStack.Children.Add(_otherPileWPF);
            var tempLabel = GetDefaultLabel();
            tempLabel.FontSize = 40;
            tempLabel.FontWeight = FontWeights.Bold;
            tempLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(SorryCardGameViewModel.UpTo)));
            otherStack.Children.Add(tempLabel);
            thisStack.Children.Add(otherStack);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SorryCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SorryCardGameViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SorryCardGameViewModel.Instructions));
            otherStack.Children.Add(_playerHandWPF);
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
        }
    }
}