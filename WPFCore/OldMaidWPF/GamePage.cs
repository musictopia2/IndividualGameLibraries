using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using OldMaidCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace OldMaidWPF
{
    public class GamePage : MultiPlayerWindow<OldMaidViewModel, OldMaidPlayerItem, OldMaidSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _opponentWPF!.LoadList(ThisMod.OpponentCards1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _opponentWPF!.UpdateList(ThisMod.OpponentCards1!);
            return Task.CompletedTask;
        }
        private BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _discardGPile;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _playerHandWPF;
        private BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>? _opponentWPF;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _discardGPile = new BasePileWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _playerHandWPF = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _playerHandWPF.Divider = 2;
            _opponentWPF = new BaseHandWPF<RegularSimpleCard, ts, DeckOfCardsWPF<RegularSimpleCard>>();
            _opponentWPF.Divider = 2;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var endButton = GetGamingButton("End Turn", nameof(OldMaidViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            otherStack.Children.Add(endButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(OldMaidViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OldMaidViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_opponentWPF);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(otherStack);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        //protected override void RegisterTests()
        //{
        //    ThisTest.PlayCategory = BasicGameFramework.TestUtilities.EnumPlayCategory.NoShuffle;
        //    ThisTest!.WhoStarts = 2;
        //}
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<OldMaidPlayerItem, OldMaidSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<OldMaidViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, OldMaidDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
        }
    }
}