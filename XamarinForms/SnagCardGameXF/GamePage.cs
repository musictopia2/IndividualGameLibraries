using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using SnagCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SnagCardGameXF
{
    public class GamePage : MultiPlayerPage<SnagCardGameViewModel, SnagCardGamePlayerItem, SnagCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            SnagCardGameSaveInfo saveRoot = OurContainer!.Resolve<SnagCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame!.TrickArea1!, ts.TagUsed);
            _bar1!.LoadList(ThisMod.Bar1!, ts.TagUsed);
            _human1!.LoadList(ThisMod.Human1!, ts.TagUsed);
            _opponent1!.LoadList(ThisMod.Opponent1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override async Task HandleAsync(UpdateEventModel message)
        {
            SnagCardGameSaveInfo saveRoot = OurContainer!.Resolve<SnagCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _bar1!.UpdateList(ThisMod.Bar1!);
            _human1!.UpdateList(ThisMod.Human1!);
            _opponent1!.UpdateList(ThisMod.Opponent1!);
            await Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>? _playerHand;
        private SeveralPlayersTrickXF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>, SnagCardGamePlayerItem>? _trick1;
        private SnagCardGameMainGameClass? _mainGame;
        private BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>? _bar1;
        private BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>? _human1;
        private BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>? _opponent1;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<SnagCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>, SnagCardGamePlayerItem>();
            _bar1 = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _human1 = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _opponent1 = new BaseHandXF<SnagCardGameCardInformation, ts, DeckOfCardsXF<SnagCardGameCardInformation>>();
            _bar1.HandType = HandViewModel<SnagCardGameCardInformation>.EnumHandList.Vertical;
            _bar1.HasAngles = true; //this is needed.
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout TempStack = new StackLayout();
            otherStack.Children.Add(_trick1);
            TempStack.Children.Add(_human1);
            TempStack.Children.Add(_opponent1);
            otherStack.Children.Add(TempStack);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Won", true, nameof(SnagCardGamePlayerItem.CardsWon));
            _thisScore.AddColumn("Current Points", true, nameof(SnagCardGamePlayerItem.CurrentPoints));
            _thisScore.AddColumn("Total Points", true, nameof(SnagCardGamePlayerItem.TotalPoints));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(SnagCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnagCardGameViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SnagCardGameViewModel.Instructions));
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 30);
            AddLeftOverColumn(otherGrid, 70); // can always be adjusted
            AddControlToGrid(otherGrid, _bar1, 0, 0);
            AddControlToGrid(otherGrid, thisStack, 0, 1);
            MainGrid!.Children.Add(otherGrid);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<SnagCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<SnagCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<SnagCardGamePlayerItem, SnagCardGameSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<SnagCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<SnagCardGameCardInformation> ThisSort = new SortSimpleCards<SnagCardGameCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
        }
    }
}