using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
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
using HitTheDeckCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace HitTheDeckWPF
{
    public class GamePage : MultiPlayerWindow<HitTheDeckViewModel, HitTheDeckPlayerItem, HitTheDeckSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            HitTheDeckSaveInfo saveRoot = OurContainer!.Resolve<HitTheDeckSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ""); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.

            _deckGPile!.Init(ThisMod.Deck1!, ""); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            HitTheDeckSaveInfo saveRoot = OurContainer!.Resolve<HitTheDeckSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            return Task.CompletedTask;
        }
        private BaseDeckWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>? _deckGPile;
        private BasePileWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>? _discardGPile;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>? _playerHandWPF;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>();
            _discardGPile = new BasePileWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsWPF>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            StackPanel tempStack = new StackPanel();
            otherStack.Children.Add(tempStack);
            Button otherButs;
            otherButs = GetGamingButton("Flip Deck", nameof(HitTheDeckViewModel.FlipDeckCommand));
            tempStack.Children.Add(otherButs);
            otherButs = GetGamingButton("Cut Deck", nameof(HitTheDeckViewModel.CutDeckCommand));
            tempStack.Children.Add(otherButs);
            var endButton = GetGamingButton("End Turn", nameof(HitTheDeckViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            tempStack.Children.Add(endButton);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", true, nameof(HitTheDeckPlayerItem.ObjectCount), rightMargin: 5);
            _thisScore.AddColumn("Total Points", true, nameof(HitTheDeckPlayerItem.TotalPoints), rightMargin: 5);
            _thisScore.AddColumn("Previous Points", true, nameof(HitTheDeckPlayerItem.PreviousPoints), rightMargin: 5);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(HitTheDeckViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(HitTheDeckViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(HitTheDeckViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterTests()
        {
            ThisTest!.AllowAnyMove = true;
            OurContainer!.RegisterType<TestConfig>();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<HitTheDeckViewModel>();
            OurContainer!.RegisterType<DeckViewModel<HitTheDeckCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<HitTheDeckPlayerItem, HitTheDeckSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<HitTheDeckCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, HitTheDeckDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
        }
    }
}