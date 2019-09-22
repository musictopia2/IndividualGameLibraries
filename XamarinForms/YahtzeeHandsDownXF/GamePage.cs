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
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using YahtzeeHandsDownCP;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace YahtzeeHandsDownXF
{
    public class GamePage : MultiPlayerPage<YahtzeeHandsDownViewModel, YahtzeeHandsDownPlayerItem, YahtzeeHandsDownSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            YahtzeeHandsDownSaveInfo saveRoot = OurContainer!.Resolve<YahtzeeHandsDownSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _combo1!.UpdateList(ThisMod.ComboHandList!);
            _chance1!.UpdatePile(ThisMod.ChancePile!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>? _deckGPile;
        private BasePileXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>? _playerHand;
        private ComboHandXF? _combo1;
        private ChanceSinglePileXF? _chance1;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>();
            _discardGPile = new BasePileXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<YahtzeeHandsDownCardInformation, YahtzeeHandsDownGraphicsCP, CardGraphicsXF>();
            _combo1 = new ComboHandXF();
            _chance1 = new ChanceSinglePileXF();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            _chance1.Margin = new Thickness(5, 5, 5, 5);
            _chance1.HorizontalOptions = LayoutOptions.Start;
            _chance1.VerticalOptions = LayoutOptions.Start;
            otherStack.Children.Add(_chance1);
            _thisScore.AddColumn("Cards Left", true, nameof(YahtzeeHandsDownPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Total Score", true, nameof(YahtzeeHandsDownPlayerItem.TotalScore));
            _thisScore.AddColumn("Won Last Round", true, nameof(YahtzeeHandsDownPlayerItem.WonLastRound));
            _thisScore.AddColumn("Score Round", true, nameof(YahtzeeHandsDownPlayerItem.ScoreRound));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(YahtzeeHandsDownViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(YahtzeeHandsDownViewModel.Status));
            thisStack.Children.Add(_playerHand);
            var otherButton = GetGamingButton("Go Out", nameof(YahtzeeHandsDownViewModel.GoOutCommand));
            thisStack.Children.Add(otherButton);
            var endButton = GetGamingButton("End Turn", nameof(YahtzeeHandsDownViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            thisStack.Children.Add(endButton);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
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