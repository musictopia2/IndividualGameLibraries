using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
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
using HorseshoeCardGameCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HorseshoeCardGameXF
{
    public class GamePage : MultiPlayerPage<HorseshoeCardGameViewModel, HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            HorseshoeCardGameSaveInfo saveRoot = OurContainer!.Resolve<HorseshoeCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame!.TrickArea1!, ts.TagUsed);
            HorseshoeCardGamePlayerItem thisPlayer;
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            if (_mainGame.SingleInfo.Id == 1)
                thisPlayer = _mainGame.PlayerList![2];
            else
                thisPlayer = _mainGame.PlayerList![1];
            _temp1.LoadList(_mainGame.SingleInfo.TempHand!);
            _temp2.LoadList(thisPlayer.TempHand!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            HorseshoeCardGameSaveInfo saveRoot = OurContainer!.Resolve<HorseshoeCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            HorseshoeCardGamePlayerItem thisPlayer;
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            if (_mainGame.SingleInfo.Id == 1)
                thisPlayer = _mainGame.PlayerList![2];
            else
                thisPlayer = _mainGame.PlayerList![1];
            _temp1.UpdateList(_mainGame.SingleInfo.TempHand!);
            _temp2.UpdateList(thisPlayer.TempHand!);
            return Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>>? _playerHand;
        private SeveralPlayersTrickXF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem>? _trick1;
        private HorseshoeCardGameMainGameClass? _mainGame;
        private readonly PlayerBoardXF<HorseshoeCardGameCardInformation> _temp1 = new PlayerBoardXF<HorseshoeCardGameCardInformation>();
        private readonly PlayerBoardXF<HorseshoeCardGameCardInformation> _temp2 = new PlayerBoardXF<HorseshoeCardGameCardInformation>();
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<HorseshoeCardGameMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsXF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            thisStack.Children.Add(_trick1);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards", false, nameof(HorseshoeCardGamePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("T Won", false, nameof(HorseshoeCardGamePlayerItem.TricksWon));
            _thisScore.AddColumn("P Score", false, nameof(HorseshoeCardGamePlayerItem.PreviousScore));
            _thisScore.AddColumn("T Score", false, nameof(HorseshoeCardGamePlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(HorseshoeCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HorseshoeCardGameViewModel.Status));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            Grid finalGrid = new Grid();
            Grid tempGrid = new Grid();
            AddLeftOverRow(tempGrid, 1);
            AddLeftOverRow(tempGrid, 1);
            AddLeftOverColumn(finalGrid, 1);
            AddAutoColumns(finalGrid, 1);
            AddControlToGrid(finalGrid, thisStack, 0, 0);
            AddControlToGrid(finalGrid, tempGrid, 0, 1);
            AddControlToGrid(tempGrid, _temp1, 0, 0);
            AddControlToGrid(tempGrid, _temp2, 1, 0);
            MainGrid!.Children.Add(finalGrid);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<HorseshoeCardGameViewModel>();
            OurContainer!.RegisterType<DeckViewModel<HorseshoeCardGameCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<HorseshoeCardGameCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.

            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<HorseshoeCardGameCardInformation> ThisSort = new SortSimpleCards<HorseshoeCardGameCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
        }
    }
}