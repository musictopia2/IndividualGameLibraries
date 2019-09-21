using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using RackoCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace RackoWPF
{
    public class GamePage : MultiPlayerWindow<RackoViewModel, RackoPlayerItem, RackoSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            RackoSaveInfo saveRoot = OurContainer!.Resolve<RackoSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _discardGPile!.Init(ThisMod!.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _handWPF!.Init(_mainGame!);
            _currentWPF!.Init(ThisMod.Pile2!, "");
            _currentWPF.StartAnimationListener("otherpile");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            RackoSaveInfo saveRoot = OurContainer!.Resolve<RackoSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _deckGPile!.UpdateDeck(ThisMod!.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _handWPF!.Update(_mainGame!);
            _currentWPF!.UpdatePile(ThisMod.Pile2!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>? _currentWPF;
        private ScoreBoardWPF? _thisScore;
        private RackoUI? _handWPF; //use this instead.
        private RackoMainGameClass? _mainGame;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<RackoMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _handWPF = new RackoUI();
            _currentWPF = new BasePileWPF<RackoCardInformation, RackoGraphicsCP, CardGraphicsWPF>();
            Grid finalGrid = new Grid();
            AddAutoRows(finalGrid, 1);
            AddAutoColumns(finalGrid, 2);
            MainGrid!.Children.Add(finalGrid); // forgot this.
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            thisStack.Children.Add(_deckGPile);
            thisStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(_currentWPF);
            var thisBut = GetGamingButton("Discard Current Card", nameof(RackoViewModel.DiscardCurrentCommand));
            thisStack.Children.Add(thisBut);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(RackoViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RackoViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            thisBut = GetGamingButton("Racko", nameof(RackoViewModel.RackoCommand));
            thisStack.Children.Add(thisBut);
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            _thisScore.AddColumn("Score Round", true, nameof(RackoPlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", true, nameof(RackoPlayerItem.TotalScore));
            int x;
            for (x = 1; x <= 10; x++)
                _thisScore.AddColumn("Section" + x, false, "Value" + x, nameof(RackoPlayerItem.CanShowValues));// 2 bindings.
            thisStack.Children.Add(_thisScore);
            _handWPF = new RackoUI();
            AddControlToGrid(finalGrid, _handWPF, 0, 1); // first column
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RackoViewModel>();
            OurContainer!.RegisterType<DeckViewModel<RackoCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<RackoPlayerItem, RackoSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<RackoCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RackoDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
        }
    }
}