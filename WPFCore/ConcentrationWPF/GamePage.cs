using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using ConcentrationCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace ConcentrationWPF
{
    public class GamePage : MultiPlayerWindow<ConcentrationViewModel, ConcentrationPlayerItem, ConcentrationSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            ConcentrationSaveInfo saveRoot = OurContainer!.Resolve<ConcentrationSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _thisBoard!.Init(ThisMod!.GameBoard1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ConcentrationSaveInfo saveRoot = OurContainer!.Resolve<ConcentrationSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _thisBoard!.UpdateLists(ThisMod!.GameBoard1!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _thisBoard;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisBoard = new BasicMultiplePilesWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardWPF();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Pairs", true, nameof(ConcentrationPlayerItem.Pairs)); //very common.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ConcentrationViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ConcentrationViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_thisBoard);
            thisStack.Children.Add(otherStack);
            StackPanel finalStack = new StackPanel();
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_thisScore);
            AddRestoreCommand(finalStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ConcentrationPlayerItem, ConcentrationSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<ConcentrationViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
        }
    }
}