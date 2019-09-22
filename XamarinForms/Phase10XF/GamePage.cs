using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using Phase10CP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace Phase10XF
{
    public class GamePage : MultiPlayerPage<Phase10ViewModel, Phase10PlayerItem, Phase10SaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            Phase10SaveInfo saveRoot = OurContainer!.Resolve<Phase10SaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _mainG!.Init(ThisMod.MainSets!, "");
            _tempG!.Init(ThisMod.TempSets!, "");
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            Phase10SaveInfo saveRoot = OurContainer!.Resolve<Phase10SaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>? _playerHand;
        private TempRummySetsXF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>? _tempG;
        private MainRummySetsXF<EnumColorTypes, EnumColorTypes,
            Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF, PhaseSet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _tempG = new TempRummySetsXF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF>();
            _mainG = new MainRummySetsXF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsXF, PhaseSet, SavedSet>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 3); // has to be this way because of scoreboard.
            finalGrid.Children.Add(GameButton);
            finalGrid.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40); // 50 was too much.  if there is scrolling, i guess okay.
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 15); // for other details
            AddLeftOverColumn(firstGrid, 30); // for scoreboard
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHand);
            var thisBut = GetSmallerButton("Complete" + Constants.vbCrLf + "Phase", nameof(Phase10ViewModel.CompletedPhaseCommand));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(Phase10ViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Phase10ViewModel.Status));
            firstInfo.AddRow("Phase", nameof(Phase10ViewModel.CurrentPhase));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Score", true, nameof(Phase10PlayerItem.TotalScore));
            _thisScore.AddColumn("Cards Left", true, nameof(Phase10PlayerItem.ObjectCount));
            _thisScore.AddColumn("Phase", true, nameof(Phase10PlayerItem.Phase));
            _thisScore.AddColumn("Skipped", true, nameof(Phase10PlayerItem.MissNextTurn), useTrueFalse: true);
            _thisScore.AddColumn("Completed", true, nameof(Phase10PlayerItem.Completed), useTrueFalse: true);
            AddControlToGrid(firstGrid, _thisScore, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            _tempG.Divider = 1.1;
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 2, 0);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            MainGrid!.Children.Add(finalGrid); //looks like everything needs to go here instead.  has to risk no restore this time.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<Phase10ViewModel>();
            OurContainer!.RegisterType<DeckViewModel<Phase10CardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<Phase10PlayerItem, Phase10SaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<Phase10CardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, Phase10UnoDeck>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>("");
        }
    }
}