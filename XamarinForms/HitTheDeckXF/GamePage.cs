using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
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
using HitTheDeckCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace HitTheDeckXF
{
    public class GamePage : MultiPlayerPage<HitTheDeckViewModel, HitTheDeckPlayerItem, HitTheDeckSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            HitTheDeckSaveInfo saveRoot = OurContainer!.Resolve<HitTheDeckSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>? _playerHand;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<HitTheDeckCardInformation, HitTheDeckGraphicsCP, CardGraphicsXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            StackLayout tempStack = new StackLayout();
            otherStack.Children.Add(tempStack);
            Button otherButs;
            otherButs = GetGamingButton("Flip Deck", nameof(HitTheDeckViewModel.FlipDeckCommand));
            tempStack.Children.Add(otherButs);
            otherButs = GetGamingButton("Cut Deck", nameof(HitTheDeckViewModel.CutDeckCommand));
            tempStack.Children.Add(otherButs);
            var endButton = GetGamingButton("End Turn", nameof(HitTheDeckViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            tempStack.Children.Add(endButton);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(HitTheDeckPlayerItem.ObjectCount), rightMargin: 5);
            _thisScore.AddColumn("Total Points", true, nameof(HitTheDeckPlayerItem.TotalPoints), rightMargin: 5);
            _thisScore.AddColumn("Previous Points", true, nameof(HitTheDeckPlayerItem.PreviousPoints), rightMargin: 5);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(HitTheDeckViewModel.NormalTurn));
            firstInfo.AddRow("Next", nameof(HitTheDeckViewModel.NextPlayer));
            firstInfo.AddRow("Status", nameof(HitTheDeckViewModel.Status));
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
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