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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using YahtzeeHandsDownCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace YahtzeeHandsDownWPF
{
    public class GamePage : MultiPlayerWindow<YahtzeeHandsDownViewModel, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            YahtzeeHandsDownSaveInfo saveRoot = OurContainer!.Resolve<YahtzeeHandsDownSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _combo1!.LoadList(ThisMod.ComboHandList!, "combo");
            _chance1!.Init(ThisMod.ChancePile!, "");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            YahtzeeHandsDownSaveInfo saveRoot = OurContainer!.Resolve<YahtzeeHandsDownSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _combo1!.UpdateList(ThisMod.ComboHandList!);
            _chance1!.UpdatePile(ThisMod.ChancePile!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        private ComboHandWPF? _combo1;
        private ChanceSinglePileWPF? _chance1;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsWPF>();
            _combo1 = new ComboHandWPF();
            _chance1 = new ChanceSinglePileWPF();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            _chance1.Margin = new Thickness(5, 5, 5, 5);
            _chance1.HorizontalAlignment = HorizontalAlignment.Left;
            _chance1.VerticalAlignment = VerticalAlignment.Top;
            otherStack.Children.Add(_chance1);
            _thisScore.AddColumn("Cards Left", true, nameof(YahtzeeHandsDownPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Total Score", true, nameof(YahtzeeHandsDownPlayerItem.TotalScore));
            _thisScore.AddColumn("Won Last Round", true, nameof(YahtzeeHandsDownPlayerItem.WonLastRound));
            _thisScore.AddColumn("Score Round", true, nameof(YahtzeeHandsDownPlayerItem.ScoreRound));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(YahtzeeHandsDownViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(YahtzeeHandsDownViewModel.Status));
            thisStack.Children.Add(_playerHandWPF);
            var otherButton = GetGamingButton("Go Out", nameof(YahtzeeHandsDownViewModel.GoOutCommand));
            thisStack.Children.Add(otherButton);
            var endButton = GetGamingButton("End Turn", nameof(YahtzeeHandsDownViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            thisStack.Children.Add(endButton);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(thisStack);
            _combo1.HandType = HandViewModel<ComboCardInfo>.EnumHandList.Vertical;
            otherStack.Children.Add(_combo1);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            MainGrid!.Children.Add(otherStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<YahtzeeHandsDownViewModel>();
            OurContainer!.RegisterType<DeckViewModel<YahtzeeHandsDownCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<YahtzeeHandsDownCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, YahtzeeHandsDownDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterSingleton<IProportionImage, ComboProportion>("combo");
        }
    }
}