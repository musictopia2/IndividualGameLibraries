using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
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
using HorseshoeCardGameCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HorseshoeCardGameWPF
{
    public class GamePage : MultiPlayerWindow<HorseshoeCardGameViewModel, HorseshoeCardGamePlayerItem, HorseshoeCardGameSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            HorseshoeCardGameSaveInfo saveRoot = OurContainer!.Resolve<HorseshoeCardGameSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame.TrickArea1!, ts.TagUsed);
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            HorseshoeCardGamePlayerItem thisPlayer;
            if (_mainGame.SingleInfo.Id == 1)
                thisPlayer = _mainGame.PlayerList[2];
            else
                thisPlayer = _mainGame.PlayerList[1];
            _temp1.LoadList(_mainGame.SingleInfo.TempHand!);
            _temp2.LoadList(thisPlayer.TempHand!);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            HorseshoeCardGameSaveInfo saveRoot = OurContainer!.Resolve<HorseshoeCardGameSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            HorseshoeCardGamePlayerItem thisPlayer;
            if (_mainGame.SingleInfo!.Id == 1)
                thisPlayer = _mainGame.PlayerList![2];
            else
                thisPlayer = _mainGame.PlayerList![1];
            _temp1.UpdateList(_mainGame.SingleInfo.TempHand!);
            _temp2.UpdateList(thisPlayer.TempHand!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>>? _playerHandWPF;
        private SeveralPlayersTrickWPF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem>? _trick1;
        private HorseshoeCardGameMainGameClass? _mainGame;
        private readonly PlayerBoardWPF<HorseshoeCardGameCardInformation> _temp1 = new PlayerBoardWPF<HorseshoeCardGameCardInformation>();
        private readonly PlayerBoardWPF<HorseshoeCardGameCardInformation> _temp2 = new PlayerBoardWPF<HorseshoeCardGameCardInformation>();
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<HorseshoeCardGameMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>>();
            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, HorseshoeCardGameCardInformation, ts, DeckOfCardsWPF<HorseshoeCardGameCardInformation>, HorseshoeCardGamePlayerItem>();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            thisStack.Children.Add(_trick1);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards", false, nameof(HorseshoeCardGamePlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tricks Won", false, nameof(HorseshoeCardGamePlayerItem.TricksWon));
            _thisScore.AddColumn("Previous Score", false, nameof(HorseshoeCardGamePlayerItem.PreviousScore));
            _thisScore.AddColumn("Total Score", false, nameof(HorseshoeCardGamePlayerItem.TotalScore));
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(HorseshoeCardGameViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HorseshoeCardGameViewModel.Status));
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(_thisScore);
            thisStack.Children.Add(firstInfo.GetContent);
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