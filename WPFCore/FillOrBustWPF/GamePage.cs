using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dice;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace FillOrBustWPF
{
    public class GamePage : MultiPlayerWindow<FillOrBustViewModel, FillOrBustPlayerItem, FillOrBustSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
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
        private BaseDeckWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private DiceListControlWPF<SimpleDice>? _diceControl;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _deckGPile = new BaseDeckWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<FillOrBustCardInformation, FillOrBustGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _diceControl = new DiceListControlWPF<SimpleDice>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Current Score", true, nameof(FillOrBustPlayerItem.CurrentScore), rightMargin: 10);
            _thisScore.AddColumn("Total Score", true, nameof(FillOrBustPlayerItem.TotalScore), rightMargin: 10);
            otherStack.Children.Add(_thisScore);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(_diceControl);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            var thisBut = GetGamingButton("Roll Dice", nameof(FillOrBustViewModel.RollDiceCommand));
            thisBut.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(thisBut);
            thisBut = GetGamingButton("Remove Dice", nameof(FillOrBustViewModel.ChooseDiceCommand));
            thisBut.Margin = new Thickness(0, 0, 5, 0);
            otherStack.Children.Add(thisBut);
            var endButton = GetGamingButton("End Turn", nameof(FillOrBustViewModel.EndTurnCommand));
            otherStack.Children.Add(endButton);
            SimpleLabelGrid TempInfo = new SimpleLabelGrid();
            TempInfo.AddRow("Temporary Score", nameof(FillOrBustViewModel.TempScore));
            TempInfo.AddRow("Score", nameof(FillOrBustViewModel.DiceScore));
            otherStack.Children.Add(TempInfo.GetContent);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Instructions", nameof(FillOrBustViewModel.Instructions));
            firstInfo.AddRow("Turn", nameof(FillOrBustViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(FillOrBustViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(firstInfo.GetContent);
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
            OurContainer.RegisterSingleton<IProportionImage, ProportionWPF>("");
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceWPF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, FillOrBustPlayerItem>>();
            //anything else that needs to be registered will be here.

        }
    }
}