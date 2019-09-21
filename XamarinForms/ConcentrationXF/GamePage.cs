using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using ConcentrationCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace ConcentrationXF
{
    public class GamePage : MultiPlayerPage<ConcentrationViewModel, ConcentrationPlayerItem, ConcentrationSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

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
        private ScoreBoardXF? _thisScore;
        private BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _thisBoard;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardXF();
            _thisBoard = new BasicMultiplePilesXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _thisScore.AddColumn("Pairs", false, nameof(ConcentrationPlayerItem.Pairs)); //very common.
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ConcentrationViewModel.NormalTurn));
            MainGrid!.Children.Add(thisStack);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_thisBoard);
            thisStack.Children.Add(otherStack);
            StackLayout finalStack = new StackLayout();
            otherStack.Children.Add(finalStack);
            finalStack.Children.Add(firstInfo.GetContent);
            finalStack.Children.Add(_thisScore);
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