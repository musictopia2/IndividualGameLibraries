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
using BasicGameFramework.SpecializedGameTypes.TrickClasses;
using HuseHeartsCP;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HuseHeartsWPF
{
    public class GamePage : MultiPlayerWindow<HuseHeartsViewModel, HuseHeartsPlayerItem, HuseHeartsSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            HuseHeartsSaveInfo saveRoot = OurContainer!.Resolve<HuseHeartsSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame!.TrickArea1!, ts.TagUsed);
            _dummy1!.LoadList(ThisMod.Dummy1!, ts.TagUsed);
            _blind1!.LoadList(ThisMod.Blind1!, ts.TagUsed);
            _moon1!.LoadLists();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            HuseHeartsSaveInfo saveRoot = OurContainer!.Resolve<HuseHeartsSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHandWPF!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _dummy1!.UpdateList(ThisMod.Dummy1!);
            _blind1!.UpdateList(ThisMod.Blind1!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>? _playerHandWPF;
        private SeveralPlayersTrickWPF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>, HuseHeartsPlayerItem>? _trick1;
        private HuseHeartsMainGameClass? _mainGame;
        private MoonUI? _moon1;
        private BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>? _dummy1;
        private BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>? _blind1;
        protected async override void AfterGameButton()
        {
            _mainGame = OurContainer!.Resolve<HuseHeartsMainGameClass>();
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _thisScore = new ScoreBoardWPF();
            _playerHandWPF = new BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>();
            _playerHandWPF.Divider = 1.4;
            _trick1 = new SeveralPlayersTrickWPF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>, HuseHeartsPlayerItem>();
            _moon1 = new MoonUI();
            _dummy1 = new BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>();
            _blind1 = new BaseHandWPF<HuseHeartsCardInformation, ts, DeckOfCardsWPF<HuseHeartsCardInformation>>();
            _trick1.Width = 350;
            _dummy1.Divider = 1.4;
            var passBut = GetGamingButton("Pass Cards", nameof(HuseHeartsViewModel.PassCardsCommand));
            var thisBind = GetVisibleBinding(nameof(HuseHeartsViewModel.PassingVisible));
            passBut.SetBinding(VisibilityProperty, thisBind);
            thisBind = GetVisibleBinding(nameof(HuseHeartsViewModel.TrickVisible));
            _trick1.SetBinding(VisibilityProperty, thisBind);
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton); //most has rounds.
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardWPF();
            _thisScore.AddColumn("Cards Left", false, nameof(HuseHeartsPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tricks Won", false, nameof(HuseHeartsPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(HuseHeartsPlayerItem.CurrentScore));
            _thisScore.AddColumn("Previous Score", false, nameof(HuseHeartsPlayerItem.PreviousScore));
            _thisScore.AddColumn("Total Score", false, nameof(HuseHeartsPlayerItem.TotalScore));
            otherStack.Children.Add(_thisScore);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(HuseHeartsViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HuseHeartsViewModel.Status));
            firstInfo.AddRow("Round", nameof(HuseHeartsViewModel.RoundNumber)); //if trump is not needed, then comment.
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(passBut);
            otherStack.Children.Add(_moon1);
            otherStack.Children.Add(_blind1);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHandWPF);
            thisStack.Children.Add(_dummy1);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<HuseHeartsViewModel>();
            OurContainer!.RegisterType<DeckViewModel<HuseHeartsCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<HuseHeartsPlayerItem, HuseHeartsSaveInfo>>();
            OurContainer.RegisterType<RegularCardsBasicShuffler<HuseHeartsCardInformation>>();
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularAceHighCalculator>(); //most of the time, ace is high for trick taking games.
            bool rets = OurContainer.ObjectExist<ISortCategory>();
            if (rets == true)
            {
                ISortCategory ThisCat = OurContainer.Resolve<ISortCategory>();
                SortSimpleCards<HuseHeartsCardInformation> ThisSort = new SortSimpleCards<HuseHeartsCardInformation>();
                ThisSort.SuitForSorting = ThisCat.SortCategory;
                OurContainer.RegisterSingleton(ThisSort); //if we have a custom one, will already be picked up.
            }
            OurContainer.RegisterSingleton<IDeckCount, RegularAceHighSimpleDeck>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
        }
    }
}