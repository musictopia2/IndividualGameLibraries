using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
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
using RageCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace RageCardGameWPF
{
    public class GamePage : MultiPlayerWindow<RageCardGameViewModel, RageCardGamePlayerItem, RageCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel Message)
        {
            _thisScore!.LoadLists(_mainGame!.SaveRoot!.PlayerList); // the data should come from the playeritem.
            _colorScore!.LoadLists(_mainGame.SaveRoot.PlayerList);
            _bidScore!.LoadLists(_mainGame.SaveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _colorHandWPF!.LoadList(ThisMod!.PlayerHand1!, "");
            _bidHandWPF!.LoadList(ThisMod.PlayerHand1!, "");
            _bidUI!.LoadLists(ThisMod.Bid1!);
            _thisColor!.LoadLists(ThisMod.ColorVM!);
            _trick1!.Init(_mainGame.TrickArea1!, _mainGame.TrickArea1!, "");
            return Task.CompletedTask;
        }

        public override Task HandleAsync(UpdateEventModel Message)
        {
            _thisScore!.UpdateLists(_mainGame!.SaveRoot!.PlayerList);
            _colorScore!.UpdateLists(_mainGame.SaveRoot.PlayerList);
            _bidScore!.UpdateLists(_mainGame.SaveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _colorHandWPF!.UpdateList(ThisMod.PlayerHand1!);
            _bidHandWPF!.UpdateList(ThisMod.PlayerHand1!);
            _trick1!.Update(_mainGame.TrickArea1!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private ScoreBoardWPF? _colorScore;
        private ScoreBoardWPF? _bidScore;
        private BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>? _colorHandWPF;
        private BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>? _bidHandWPF;
        private SeveralPlayersTrickWPF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF, RageCardGamePlayerItem>? _trick1;
        private RageCardGameMainGameClass? _mainGame;
        private NumberChooserWPF? _bidUI;
        private EnumPickerWPF<CheckerChoiceCP<EnumColor>, CheckerChooserWPF<EnumColor>, EnumColor, ColorListChooser<EnumColor>>? _thisColor;
        private StackPanel? _colorStack;
        private StackPanel? _bidStack;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _colorStack = new StackPanel();
            thisGrid.Children.Add(_colorStack);
            _bidStack = new StackPanel();
            thisGrid.Children.Add(_bidStack);
        }
        private void CreateBasics()
        {
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>();
            _trick1 = new SeveralPlayersTrickWPF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF, RageCardGamePlayerItem>();
            _bidUI = new NumberChooserWPF();
            _thisColor = new EnumPickerWPF<CheckerChoiceCP<EnumColor>, CheckerChooserWPF<EnumColor>, EnumColor, ColorListChooser<EnumColor>>();
            _colorHandWPF = new BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>();
            _bidHandWPF = new BaseHandWPF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsWPF>();
            _colorScore = new ScoreBoardWPF();
            _bidScore = new ScoreBoardWPF();
            PopulateScores(_bidScore);
            PopulateScores(_colorScore);
        }
        private void PopulateScores(ScoreBoardWPF tempScore)
        {
            tempScore.AddColumn("Cards Left", true, nameof(RageCardGamePlayerItem.ObjectCount));
            tempScore.AddColumn("Bid Amount", true, nameof(RageCardGamePlayerItem.BidAmount), visiblePath: nameof(RageCardGamePlayerItem.RevealBid));
            tempScore.AddColumn("Tricks Won", true, nameof(RageCardGamePlayerItem.TricksWon));
            tempScore.AddColumn("Correctly Bidded", true, nameof(RageCardGamePlayerItem.CorrectlyBidded));
            tempScore.AddColumn("Score Round", true, nameof(RageCardGamePlayerItem.ScoreRound));
            tempScore.AddColumn("Score Game", true, nameof(RageCardGamePlayerItem.ScoreGame));
        }
        private void BuildExtraScreens()
        {
            _colorStack!.Children.Add(_thisColor);
            _colorStack.Children.Add(_colorHandWPF);
            SimpleLabelGrid colorInfo = new SimpleLabelGrid();
            colorInfo.AddRow("Trump", nameof(RageCardGameViewModel.TrumpSuit));
            colorInfo.AddRow("Lead", nameof(RageCardGameViewModel.Lead));
            _colorStack.Children.Add(colorInfo.GetContent);
            _colorStack.Children.Add(_colorScore);
            _bidStack!.Children.Add(_bidUI);
            Button thisBut = GetGamingButton("Submit", nameof(RageCardGameViewModel.BidCommand));
            _bidStack.Children.Add(thisBut);
            SimpleLabelGrid bidInfo = new SimpleLabelGrid();
            bidInfo.AddRow("Trump", nameof(RageCardGameViewModel.TrumpSuit));
            bidInfo.AddRow("Turn", nameof(RageCardGameViewModel.NormalTurn));
            _bidStack.Children.Add(_bidHandWPF);
            _bidStack.Children.Add(bidInfo.GetContent);
            _bidStack.Children.Add(_bidScore);
            Binding thisBind;
            thisBind = GetVisibleBinding(nameof(RageCardGameViewModel.ColorVisible));
            _colorStack.SetBinding(VisibilityProperty, thisBind);
            thisBind = GetVisibleBinding(nameof(RageCardGameViewModel.BiddingVisible));
            _bidStack.SetBinding(VisibilityProperty, thisBind);
        }
        protected override async void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<RageCardGameMainGameClass>();
            StackPanel ThisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            CreateBasics();
            BuildExtraScreens();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            ThisStack.Children.Add(RoundButton); //since most games are in rounds.
            ThisStack.Children.Add(GameButton);
            ThisStack.Children.Add(_trick1);
            _thisScore = new ScoreBoardWPF();
            PopulateScores(_thisScore);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RageCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RageCardGameViewModel.Status));
            firstInfo.AddRow("Trump", nameof(RageCardGameViewModel.TrumpSuit));
            firstInfo.AddRow("Lead", nameof(RageCardGameViewModel.Lead));
            ThisStack.Children.Add(_playerHandWPF);
            ThisStack.Children.Add(firstInfo.GetContent);
            ThisStack.Children.Add(_thisScore);
            MainGrid!.Children.Add(ThisStack); //try this too.
            AddRestoreCommand(ThisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RageCardGameViewModel>();
            OurContainer!.RegisterType<StandardWidthHeight>();
            OurContainer.RegisterType<DeckViewModel<RageCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<RageCardGamePlayerItem, RageCardGameSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<RageCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RageCardGameDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
        }
    }
}