using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.ChoicePickers;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using RookCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace RookWPF
{
    public class GamePage : MultiPlayerWindow<RookViewModel, RookPlayerItem, RookSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            RookSaveInfo saveRoot = OurContainer!.Resolve<RookSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame!.TrickArea1!, "");
            _dummy1!.LoadList(ThisMod.Dummy1!, "");
            _bid1!.LoadLists(ThisMod);
            _trump1!.LoadLists(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            RookSaveInfo saveRoot = OurContainer!.Resolve<RookSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }

        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private SeveralPlayersTrickWPF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsWPF, RookPlayerItem>? _trick1;
        private RookMainGameClass? _mainGame;
        private BidControl? _bid1;
        private TrumpControl? _trump1;
        private BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF>? _dummy1;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<RookMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF>();
            _trick1 = new SeveralPlayersTrickWPF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsWPF, RookPlayerItem>();
            _bid1 = new BidControl();
            _trump1 = new TrumpControl();
            _dummy1 = new BaseHandWPF<RookCardInformation, RookGraphicsCP, CardGraphicsWPF>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Bid Amount", true, nameof(RookPlayerItem.BidAmount));
            _thisScore.AddColumn("Tricks Won", false, nameof(RookPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(RookPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", false, nameof(RookPlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RookViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RookViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(RookViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            var thisBut = GetGamingButton("Choose Nest Cards", nameof(RookViewModel.NestCommand));
            var thisBind = GetVisibleBinding(nameof(RookViewModel.NestVisible));
            thisBut.SetBinding(VisibilityProperty, thisBind);
            thisStack.Children.Add(thisBut);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(_dummy1);
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_bid1);
            otherStack.Children.Add(_trump1);
            otherStack.Children.Add(_trick1);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        //protected override void RegisterTests()
        //{
        //    ThisTest!.SaveOption = BasicGameFramework.TestUtilities.EnumTestSaveCategory.RestoreOnly;
        //}
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RookViewModel>();
            OurContainer!.RegisterType<DeckViewModel<RookCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<RookPlayerItem, RookSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<RookCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RookDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterType<StandardWidthHeight>();
        }
    }
}