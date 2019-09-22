using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using OldMaidCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace OldMaidXF
{
    public class GamePage : MultiPlayerPage<OldMaidViewModel, OldMaidPlayerItem, OldMaidSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _opponent!.LoadList(ThisMod.OpponentCards1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _opponent!.UpdateList(ThisMod.OpponentCards1!);
            return Task.CompletedTask;
        }
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _discardGPile;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _playerHand;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _opponent;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _playerHand = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponent = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponent.Divider = 2;
            _playerHand.Divider = 2;
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            var endButton = GetGamingButton("End Turn", nameof(OldMaidViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            thisStack.Children.Add(otherStack);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(OldMaidViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(OldMaidViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_opponent);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<OldMaidPlayerItem, OldMaidSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<OldMaidViewModel, RegularSimpleCard>(customDeck: true);
            OurContainer.RegisterSingleton<IDeckCount, OldMaidDeck>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>();
        }
    }
}