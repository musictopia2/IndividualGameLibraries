using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace Phase10WPF
{
    public class GamePage : MultiPlayerWindow<Phase10ViewModel, Phase10PlayerItem, Phase10SaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            Phase10SaveInfo saveRoot = OurContainer!.Resolve<Phase10SaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private TempRummySetsWPF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>? _tempG;
        private MainRummySetsWPF<EnumColorTypes, EnumColorTypes,
            Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF, PhaseSet, SavedSet>? _mainG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _tempG = new TempRummySetsWPF<EnumColorTypes, EnumColorTypes, Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF>();
            _mainG = new MainRummySetsWPF<EnumColorTypes, EnumColorTypes,
                Phase10CardInformation, Phase10GraphicsCP, CardGraphicsWPF, PhaseSet, SavedSet>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            Grid finalGrid = new Grid();
            AddLeftOverRow(finalGrid, 20); // has to be this way because of scoreboard.
            AddLeftOverRow(finalGrid, 80);
            thisStack.Children.Add(finalGrid);
            Grid firstGrid = new Grid();
            AddLeftOverColumn(firstGrid, 40); // 50 was too much.  if there is scrolling, i guess okay.
            AddLeftOverColumn(firstGrid, 10); // for buttons (can change if necessary)
            AddAutoColumns(firstGrid, 1); // maybe 1 (well see)
            AddLeftOverColumn(firstGrid, 15); // for other details
            AddLeftOverColumn(firstGrid, 30); // for scoreboard
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            AddControlToGrid(firstGrid, otherStack, 0, 2); // i think
            _playerHandWPF.HandType = HandViewModel<Phase10CardInformation>.EnumHandList.Horizontal;
            AddControlToGrid(firstGrid, _playerHandWPF, 0, 0); // i think
            var thisBut = GetGamingButton("Complete" + Constants.vbCrLf + "Phase", nameof(Phase10ViewModel.CompletedPhaseCommand));
            AddControlToGrid(firstGrid, thisBut, 0, 1);
            _thisScore.AddColumn("Score", true, nameof(Phase10PlayerItem.TotalScore));
            _thisScore.AddColumn("Cards Left", true, nameof(Phase10PlayerItem.ObjectCount));
            _thisScore.AddColumn("Phase", true, nameof(Phase10PlayerItem.Phase));
            _thisScore.AddColumn("Skipped", true, nameof(Phase10PlayerItem.MissNextTurn), useTrueFalse: true);
            _thisScore.AddColumn("Completed", true, nameof(Phase10PlayerItem.Completed), useTrueFalse: true);
            AddControlToGrid(firstGrid, _thisScore, 0, 4);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(Phase10ViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(Phase10ViewModel.Status));
            firstInfo.AddRow("Phase", nameof(Phase10ViewModel.CurrentPhase));
            AddControlToGrid(firstGrid, firstInfo.GetContent, 0, 3);
            AddControlToGrid(finalGrid, firstGrid, 0, 0); // i think
            _tempG.Height = 700;
            _tempG.Divider = 1.3;
            StackPanel thirdStack = new StackPanel();
            thirdStack.Orientation = Orientation.Horizontal;
            _mainG.Height = 700; // try this way.
            thirdStack.Children.Add(_tempG);
            thirdStack.Children.Add(_mainG);
            AddControlToGrid(finalGrid, thirdStack, 1, 0); // i think
            MainGrid!.Children.Add(thisStack); //i think.  could be wrong.  well see.
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<Phase10ViewModel>();
            OurContainer!.RegisterType<DeckViewModel<Phase10CardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<Phase10PlayerItem, Phase10SaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<Phase10CardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, Phase10UnoDeck>();
            OurContainer.RegisterSingleton<IProportionImage, SmallDrawableProportion>(""); //has to be small this time.
        }
    }
}