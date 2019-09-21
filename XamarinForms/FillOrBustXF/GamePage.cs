using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.DataClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using FillOrBustCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static AndyCristinaGamePackageCP.DataClasses.GlobalStaticClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace FillOrBustXF
{
    public class GamePage : MultiPlayerPage<FillOrBustViewModel, FillOrBustPlayerItem, FillOrBustSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            FillOrBustSaveInfo saveRoot = OurContainer!.Resolve<FillOrBustSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _diceControl!.LoadDiceViewModel(ThisMod.ThisCup!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            FillOrBustSaveInfo saveRoot = OurContainer!.Resolve<FillOrBustSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _diceControl!.UpdateDice(ThisMod.ThisCup!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private DiceListControlXF<SimpleDice>? _diceControl;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsXF>();
            _diceControl = new DiceListControlXF<SimpleDice>();
            _thisScore = new ScoreBoardXF();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            Grid firstGrid = new Grid();
            AddAutoRows(firstGrid, 1);
            AddAutoColumns(firstGrid, 1);
            AddLeftOverColumn(firstGrid, 1);
            AddControlToGrid(firstGrid, otherStack, 0, 0);
            _thisScore = new ScoreBoardXF();
            _thisScore.HorizontalOptions = LayoutOptions.End;
            _thisScore.AddColumn("Current Score", true, nameof(FillOrBustPlayerItem.CurrentScore), rightMargin: 10);
            _thisScore.AddColumn("Total Score", true, nameof(FillOrBustPlayerItem.TotalScore), rightMargin: 10);
            Grid finGrid = new Grid();
            AddAutoColumns(finGrid, 1);
            AddAutoRows(finGrid, 2);
            AddLeftOverColumn(finGrid, 1);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            if (ScreenUsed != EnumScreen.SmallPhone)
                firstInfo.AddRow("Instructions", nameof(FillOrBustViewModel.Instructions));
            firstInfo.AddRow("Turn", nameof(FillOrBustViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FillOrBustViewModel.Status));
            AddControlToGrid(finGrid, _thisScore, 0, 0);
            var temps = firstInfo.GetContent;
            AddControlToGrid(finGrid, temps, 1, 0);
            Grid.SetColumnSpan(temps, 2);
            AddControlToGrid(firstGrid, finGrid, 0, 1);
            thisStack.Children.Add(firstGrid);
            thisStack.Children.Add(_diceControl);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            var thisBut = GetSmallerButton("Roll Dice", nameof(FillOrBustViewModel.RollDiceCommand));
            thisBut.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(thisBut);
            thisBut = GetSmallerButton("Remove Dice", nameof(FillOrBustViewModel.ChooseDiceCommand));
            thisBut.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(thisBut);
            var endButton = GetSmallerButton("End Turn", nameof(FillOrBustViewModel.EndTurnCommand));
            otherStack.Children.Add(endButton);
            SimpleLabelGridXF TempInfo = new SimpleLabelGridXF();
            TempInfo.AddRow("Temporary Score", nameof(FillOrBustViewModel.TempScore));
            TempInfo.AddRow("Score", nameof(FillOrBustViewModel.DiceScore));
            otherStack.Children.Add(TempInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<FillOrBustViewModel>();
            OurContainer!.RegisterType<DeckViewModel<FillOrBustCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<FillOrBustPlayerItem, FillOrBustSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<FillOrBustCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, FillOrBustDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, FillOrBustPlayerItem>>();
        }
    }
}