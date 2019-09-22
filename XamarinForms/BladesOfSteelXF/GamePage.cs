using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.SingleCardFrames;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using BladesOfSteelCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace BladesOfSteelXF
{
    public class GamePage : MultiPlayerPage<BladesOfSteelViewModel, BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            BladesOfSteelSaveInfo saveRoot = OurContainer!.Resolve<BladesOfSteelSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _discardGPile!.Init(ThisMod.Pile1!, ts.TagUsed); // may have to be here (well see)
            _discardGPile.StartListeningDiscardPile(); // its the main one.
            _deckGPile!.Init(ThisMod.Deck1!, ts.TagUsed); // try here.  may have to do something else as well (?)
            _deckGPile.StartListeningMainDeck();
            _yourFace!.Init(ThisMod.YourFaceOffCard!, ts.TagUsed);
            _opponentFace!.Init(ThisMod.OpponentFaceOffCard!, ts.TagUsed);
            _mainDefenseCards!.LoadList(ThisMod.MainDefense1!, ts.TagUsed);
            _yourAttack!.LoadList(ThisMod.YourAttackPile!, ts.TagUsed);
            _yourDefense!.LoadList(ThisMod.YourDefensePile!, ts.TagUsed);
            _opponentAttack!.LoadList(ThisMod.OpponentAttackPile!, ts.TagUsed);
            _opponentDefense!.LoadList(ThisMod.OpponentDefensePile!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            BladesOfSteelSaveInfo saveRoot = OurContainer!.Resolve<BladesOfSteelSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _deckGPile!.UpdateDeck(ThisMod.Deck1!);
            _discardGPile!.UpdatePile(ThisMod.Pile1!);
            _yourFace!.UpdatePile(ThisMod.YourFaceOffCard!);
            _opponentFace!.UpdatePile(ThisMod.OpponentFaceOffCard!);
            _mainDefenseCards!.UpdateList(ThisMod.MainDefense1!);
            _yourAttack!.UpdateList(ThisMod.YourAttackPile!);
            _yourDefense!.UpdateList(ThisMod.YourDefensePile!);
            _opponentAttack!.UpdateList(ThisMod.OpponentAttackPile!);
            _opponentDefense!.UpdateList(ThisMod.OpponentDefensePile!);
            return Task.CompletedTask;
        }
        private BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _deckGPile;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _discardGPile;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _playerHand;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _yourFace;
        private BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _opponentFace;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _mainDefenseCards;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _opponentDefense;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _opponentAttack;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _yourDefense;
        private BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>? _yourAttack;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _deckGPile = new BaseDeckXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _discardGPile = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _yourFace = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponentFace = new BasePileXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _mainDefenseCards = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponentDefense = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _opponentAttack = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _yourDefense = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            _yourAttack = new BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_deckGPile);
            otherStack.Children.Add(_discardGPile); // can reposition or not even have as well.
            thisStack.Children.Add(otherStack);
            StackLayout faceStack = new StackLayout();
            faceStack.Orientation = StackOrientation.Horizontal;
            var thisBind = new Binding(nameof(BladesOfSteelViewModel.IsFaceOff));
            faceStack.SetBinding(IsVisibleProperty, thisBind);
            _yourFace.HorizontalOptions = LayoutOptions.Start; //try without 30 margins.
            _yourFace.VerticalOptions = LayoutOptions.Start;
            faceStack.Children.Add(_yourFace);
            _opponentFace.HorizontalOptions = LayoutOptions.Start;
            _opponentFace.VerticalOptions = LayoutOptions.Start;
            faceStack.Children.Add(_opponentFace);
            otherStack.Children.Add(faceStack);
            thisBind = new Binding(nameof(BladesOfSteelViewModel.CommandsVisible));
            StackLayout finStack = new StackLayout();
            otherStack.Children.Add(finStack);
            finStack.SetBinding(IsVisibleProperty, thisBind);
            SetHorizontal(_mainDefenseCards);
            finStack.Children.Add(_mainDefenseCards);
            Grid playerArea = new Grid();
            AddAutoColumns(playerArea, 3); //2 instead of 3
            AddAutoRows(playerArea, 2);
            SetHorizontal(_opponentAttack);
            SetHorizontal(_opponentDefense);
            SetHorizontal(_yourAttack);
            SetHorizontal(_yourDefense);
            _opponentDefense.Margin = new Thickness(0, 0, 0, 20);
            AddControlToGrid(playerArea, _opponentDefense, 0, 2);
            AddControlToGrid(playerArea, _opponentAttack, 0, 1);
            _opponentAttack.Margin = new Thickness(0, 0, 0, 20);
            AddControlToGrid(playerArea, _mainDefenseCards, 1, 0);
            AddControlToGrid(playerArea, _yourAttack, 1, 1);
            AddControlToGrid(playerArea, _yourDefense, 1, 2);
            thisStack.Children.Add(playerArea);
            thisStack.Children.Add(_playerHand);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            thisBind = new Binding(nameof(BladesOfSteelViewModel.CommandsVisible));
            otherStack.SetBinding(IsVisibleProperty, thisBind);
            var endButton = GetGamingButton("End Turn", nameof(BladesOfSteelViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.VerticalOptions = LayoutOptions.Center;
            otherStack.Children.Add(endButton);
            var otherBut = GetGamingButton("Pass", nameof(BladesOfSteelViewModel.PassCommand));
            otherStack.Children.Add(otherBut);
            otherBut.HorizontalOptions = LayoutOptions.Start;
            otherBut.VerticalOptions = LayoutOptions.Center;
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", true, nameof(BladesOfSteelPlayerItem.ObjectCount), rightMargin: 5);
            _thisScore.AddColumn("Score", true, nameof(BladesOfSteelPlayerItem.Score), rightMargin: 5);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(BladesOfSteelViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(BladesOfSteelViewModel.Status));
            firstInfo.AddRow("Other Turn", nameof(BladesOfSteelViewModel.OtherPlayer));
            AddVerticalLabelGroup("Instructions", nameof(BladesOfSteelViewModel.Instructions), thisStack);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            ScoringGuideXF tempScore = new ScoringGuideXF();
            thisStack.Children.Add(tempScore);
            ScrollView thisScroll = new ScrollView();
            thisScroll.Content = thisStack;
            MainGrid!.Children.Add(thisScroll);
            _deckGPile.Margin = new Thickness(5, 5, 5, 5);
            _discardGPile.Margin = new Thickness(5, 5, 5, 5);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        private void SetHorizontal(BaseHandXF<RegularSimpleCard, ts, DeckOfCardsXF<RegularSimpleCard>> hand)
        {
            hand.HandType = BasicGameFramework.DrawableListsViewModels.HandViewModel<RegularSimpleCard>.EnumHandList.Horizontal;
            hand.HorizontalOptions = LayoutOptions.Start;
            hand.VerticalOptions = LayoutOptions.Start;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<BladesOfSteelPlayerItem, BladesOfSteelSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterCommonRegularCards<BladesOfSteelViewModel, RegularSimpleCard>(aceLow: false, registerCommonProportions: false);
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 1.5f;
                return 1.1f;
            }
        }
    }
}