using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.BasicControls.TrickUIs;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using BasicGameFramework.RegularDeckOfCards;
using SnagCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace SnagCardGameWPF
{
    public class GamePage : MultiPlayerWindow<SnagCardGameViewModel, SnagCardGamePlayerItem, SnagCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            SnagCardGameSaveInfo saveRoot = OurContainer!.Resolve<SnagCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame.TrickArea1!, ts.TagUsed);
            _bar1!.LoadList(ThisMod.Bar1!, ts.TagUsed);
            _human1!.LoadList(ThisMod.Human1!, ts.TagUsed);
            _opponent1!.LoadList(ThisMod.Opponent1!, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            SnagCardGameSaveInfo saveRoot = OurContainer!.Resolve<SnagCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _bar1!.UpdateList(ThisMod.Bar1!);
            _human1!.UpdateList(ThisMod.Human1!);
            _opponent1!.UpdateList(ThisMod.Opponent1!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>? _playerHandWPF;
        private SeveralPlayersTrickWPF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>, SnagCardGamePlayerItem>? _trick1;
        private SnagCardGameMainGameClass? _mainGame;
        private BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>? _bar1;
        private BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>? _human1;
        private BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>? _opponent1;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<SnagCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>, SnagCardGamePlayerItem>();
            _bar1 = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _human1 = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _opponent1 = new BaseHandWPF<SnagCardGameCardInformation, ts, DeckOfCardsWPF<SnagCardGameCardInformation>>();
            _bar1.HandType = HandViewModel<SnagCardGameCardInformation>.EnumHandList.Vertical;
            _bar1.ExtraControlSpace = 20;
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            StackPanel TempStack = new StackPanel();
            otherStack.Children.Add(_trick1);
            TempStack.Children.Add(_human1);
            TempStack.Children.Add(_opponent1);
            otherStack.Children.Add(TempStack);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Won", true, nameof(SnagCardGamePlayerItem.CardsWon));
            _thisScore.AddColumn("Current Points", true, nameof(SnagCardGamePlayerItem.CurrentPoints));
            _thisScore.AddColumn("Total Points", true, nameof(SnagCardGamePlayerItem.TotalPoints));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(SnagCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(SnagCardGameViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(SnagCardGameViewModel.Instructions));
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 20);
            AddLeftOverColumn(otherGrid, 80); // can always be adjusted
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