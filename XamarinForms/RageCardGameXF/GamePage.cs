using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.ChoicePickers;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
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
using RageCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace RageCardGameXF
{
    public class GamePage : MultiPlayerPage<RageCardGameViewModel, RageCardGamePlayerItem, RageCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
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
        private ScoreBoardXF? _thisScore;
        private ScoreBoardXF? _colorScore;
        private ScoreBoardXF? _bidScore;
        private BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>? _playerHandWPF;
        private BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>? _colorHandWPF;
        private BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>? _bidHandWPF;
        private SeveralPlayersTrickXF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF, RageCardGamePlayerItem>? _trick1;
        private RageCardGameMainGameClass? _mainGame;
        private NumberChooserXF? _bidUI;
        private EnumPickerXF<CheckerChoiceCP<EnumColor>, CheckerChooserXF<EnumColor>, EnumColor, ColorListChooser<EnumColor>>? _thisColor;
        private StackLayout? _colorStack;
        private StackLayout? _bidStack;
        protected override void ComplexStartControls(Grid thisGrid)
        {
            _colorStack = new StackLayout();
            thisGrid.Children.Add(_colorStack);
            _bidStack = new StackLayout();
            thisGrid.Children.Add(_bidStack);
        }
        private void CreateBasics()
        {
            _thisScore = new ScoreBoardXF();
            _playerHandWPF = new BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            _trick1 = new SeveralPlayersTrickXF<EnumColor, RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF, RageCardGamePlayerItem>();
            _bidUI = new NumberChooserXF();
            _bidUI.TotalRows = 1;
            _thisColor = new EnumPickerXF<CheckerChoiceCP<EnumColor>, CheckerChooserXF<EnumColor>, EnumColor, ColorListChooser<EnumColor>>();
            _colorHandWPF = new BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            _bidHandWPF = new BaseHandXF<RageCardGameCardInformation, RageCardGameGraphicsCP, CardGraphicsXF>();
            _colorScore = new ScoreBoardXF();
            _bidScore = new ScoreBoardXF();
            PopulateScores(_bidScore);
            PopulateScores(_colorScore);
        }
        private void PopulateScores(ScoreBoardXF tempScore)
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
            SimpleLabelGridXF colorInfo = new SimpleLabelGridXF();
            colorInfo.AddRow("Trump", nameof(RageCardGameViewModel.TrumpSuit));
            colorInfo.AddRow("Lead", nameof(RageCardGameViewModel.Lead));
            _colorStack.Children.Add(colorInfo.GetContent);
            _colorStack.Children.Add(_colorScore);
            _bidStack!.Children.Add(_bidUI);
            Button thisBut = GetGamingButton("Submit", nameof(RageCardGameViewModel.BidCommand));
            _bidStack.Children.Add(thisBut);
            SimpleLabelGridXF bidInfo = new SimpleLabelGridXF();
            bidInfo.AddRow("Trump", nameof(RageCardGameViewModel.TrumpSuit));
            bidInfo.AddRow("Turn", nameof(RageCardGameViewModel.NormalTurn));
            _bidStack.Children.Add(_bidHandWPF);
            _bidStack.Children.Add(bidInfo.GetContent);
            _bidStack.Children.Add(_bidScore);
            Binding thisBind;
            thisBind = new Binding(nameof(RageCardGameViewModel.ColorVisible));
            _colorStack.SetBinding(IsVisibleProperty, thisBind);
            thisBind = new Binding(nameof(RageCardGameViewModel.BiddingVisible));
            _bidStack.SetBinding(IsVisibleProperty, thisBind);
        }
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<RageCardGameMainGameClass>();
            StackLayout ThisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            CreateBasics();
            BuildExtraScreens();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            ThisStack.Children.Add(RoundButton); //since most games are in rounds.
            ThisStack.Children.Add(GameButton);
            ThisStack.Children.Add(_trick1);
            _thisScore = new ScoreBoardXF();
            PopulateScores(_thisScore);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
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
            OurContainer.RegisterType<StandardPickerSizeClass>(); //i think this too.
        }
    }
}