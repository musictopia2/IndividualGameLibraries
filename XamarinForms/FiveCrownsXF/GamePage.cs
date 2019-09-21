using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
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
using FiveCrownsCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace FiveCrownsXF
{
    public class GamePage : MultiPlayerPage<FiveCrownsViewModel, FiveCrownsPlayerItem, FiveCrownsSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            FiveCrownsSaveInfo saveRoot = OurContainer!.Resolve<FiveCrownsSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _tempG!.Init(ThisMod!.TempSets!, "");
            _mainG!.Init(ThisMod!.MainSets!, "");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            FiveCrownsSaveInfo saveRoot = OurContainer!.Resolve<FiveCrownsSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod!.TempSets!);
            _mainG!.Update(ThisMod!.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>? _playerHand;
        private TempRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>? _tempG;
        private MainRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF, PhaseSet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            BasicSetUp();
            _deckGPile = new BaseDeckXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _tempG = new TempRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF>();
            _mainG = new MainRummySetsXF<EnumSuitList, EnumColorList, FiveCrownsCardInformation, FiveCrownsGraphicsCP, CardGraphicsXF, PhaseSet, SavedSet>();
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 3);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 15);
            AddLeftOverColumn(firstGrid, 30);
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            AddControlToGrid(finalGrid, GameButton, 0, 0);
            AddControlToGrid(finalGrid, RoundButton, 0, 0);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 1);
            StackLayout firstStack = new StackLayout();
            firstStack.Children.Add(_playerHand);
            StackLayout secondStack = new StackLayout();
            secondStack.Orientation = StackOrientation.Horizontal;
            firstStack.Children.Add(secondStack);
            var thisBut = GetSmallerButton("Lay Down", nameof(FiveCrownsViewModel.LayDownSetsCommand));
            firstStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Back", nameof(FiveCrownsViewModel.BackCommand));
            firstStack.Children.Add(thisBut);
            AddControlToGrid(firstGrid, firstStack, 0, 0);
            _thisScore.AddColumn("Cards Left", true, nameof(FiveCrownsPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Current Score", true, nameof(FiveCrownsPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", true, nameof(FiveCrownsPlayerItem.TotalScore));
            AddControlToGrid(firstGrid, _thisScore, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 1, 0);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(FiveCrownsViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FiveCrownsViewModel.Status));
            firstInfo.AddRow("Up To", nameof(FiveCrownsViewModel.UpTo));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 2);
            _tempG.Divider = 1.1;
            StackLayout thirdStack = new StackLayout();
            thirdStack.Orientation = StackOrientation.Horizontal;
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 2, 0); // i think
            MainGrid!.Children.Add(finalGrid);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FiveCrownsViewModel>();
            OurContainer!.RegisterType<DeckViewModel<FiveCrownsCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<FiveCrownsPlayerItem, FiveCrownsSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<FiveCrownsCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FiveCrownsDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>("");
            //anything else that needs to be registered will be here.

        }
    }
}