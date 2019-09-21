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
using HuseHeartsCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HuseHeartsXF
{
    public class GamePage : MultiPlayerPage<HuseHeartsViewModel, HuseHeartsPlayerItem, HuseHeartsSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            HuseHeartsSaveInfo saveRoot = OurContainer!.Resolve<HuseHeartsSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed); // i think
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
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            _dummy1!.UpdateList(ThisMod.Dummy1!);
            _blind1!.UpdateList(ThisMod.Blind1!);
            return Task.CompletedTask;
        }
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>? _playerHand;
        private SeveralPlayersTrickXF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>, HuseHeartsPlayerItem>? _trick1;
        private HuseHeartsMainGameClass? _mainGame;
        private MoonUI? _moon1;
        private BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>? _dummy1;
        private BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>? _blind1;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<HuseHeartsMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>();
            _trick1 = new SeveralPlayersTrickXF<EnumSuitList, HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>, HuseHeartsPlayerItem>();
            _dummy1 = new BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>();
            _blind1 = new BaseHandXF<HuseHeartsCardInformation, ts, DeckOfCardsXF<HuseHeartsCardInformation>>();
            _moon1 = new MoonUI();
            _dummy1.Divider = 1.4;
            _playerHand.Divider = 1.4;
            var passBut = GetGamingButton("Pass Cards", nameof(HuseHeartsViewModel.PassCardsCommand));
            var thisBind = new Binding(nameof(HuseHeartsViewModel.PassingVisible));
            passBut.SetBinding(IsVisibleProperty, thisBind);
            thisBind = new Binding(nameof(HuseHeartsViewModel.TrickVisible));
            _trick1.SetBinding(IsVisibleProperty, thisBind);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            StackLayout otherStack = new StackLayout();
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Cards Left", false, nameof(HuseHeartsPlayerItem.ObjectCount)); //very common.
            _thisScore.AddColumn("Tricks Won", false, nameof(HuseHeartsPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(HuseHeartsPlayerItem.CurrentScore));
            _thisScore.AddColumn("Previous Score", false, nameof(HuseHeartsPlayerItem.PreviousScore));
            _thisScore.AddColumn("Total Score", false, nameof(HuseHeartsPlayerItem.TotalScore));
            otherStack.Children.Add(_thisScore);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(HuseHeartsViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(HuseHeartsViewModel.Status));
            firstInfo.AddRow("Round", nameof(HuseHeartsViewModel.RoundNumber)); //if trump is not needed, then comment.
            otherStack.Children.Add(_trick1);
            otherStack.Children.Add(_moon1);
            StackLayout tempStack = new StackLayout();
            otherStack.Children.Add(tempStack);
            tempStack.Children.Add(_blind1);
            tempStack.Children.Add(passBut);
            tempStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack);
            thisStack.Children.Add(_playerHand);
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