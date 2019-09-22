using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.BasicClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.BasicControls.TrickUIs;
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.ColorCards;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using RookCP;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
namespace RookXF
{
    public class GamePage : MultiPlayerPage<RookViewModel, RookPlayerItem, RookSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            RookSaveInfo saveRoot = OurContainer!.Resolve<RookSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ""); // i think
            _trick1!.Init(_mainGame!.TrickArea1!, _mainGame!.TrickArea1!, "");
            _dummy1!.LoadList(ThisMod.Dummy1!, "");
            _bid1!.LoadLists(ThisMod);
            _trump1!.LoadLists(ThisMod);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            RookSaveInfo saveRoot = OurContainer!.Resolve<RookSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _playerHand!.UpdateList(ThisMod!.PlayerHand1!);
            _trick1!.Update(_mainGame!.TrickArea1!);
            return Task.CompletedTask;
        }

        private ScoreBoardXF? _thisScore;
        private BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>? _playerHand;
        private SeveralPlayersTrickXF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsXF, RookPlayerItem>? _trick1;
        private RookMainGameClass? _mainGame;
        private BidControl? _bid1;
        private TrumpControl? _trump1;
        private BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>? _dummy1;
        protected override async Task AfterGameButtonAsync()
        {
            _mainGame = OurContainer!.Resolve<RookMainGameClass>();
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            //anything needed for ui is here.
            _thisScore = new ScoreBoardXF();
            _playerHand = new BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>();
            _trick1 = new SeveralPlayersTrickXF<EnumColorTypes, RookCardInformation, RookGraphicsCP, CardGraphicsXF, RookPlayerItem>();
            _bid1 = new BidControl();
            _trump1 = new TrumpControl();
            _dummy1 = new BaseHandXF<RookCardInformation, RookGraphicsCP, CardGraphicsXF>();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);//most has rounds.
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            //_thisScore.HeightRequest = 200;
            _thisScore.AddColumn("Bid Amount", true, nameof(RookPlayerItem.BidAmount));
            _thisScore.AddColumn("Tricks Won", false, nameof(RookPlayerItem.TricksWon));
            _thisScore.AddColumn("Current Score", false, nameof(RookPlayerItem.CurrentScore));
            _thisScore.AddColumn("Total Score", false, nameof(RookPlayerItem.TotalScore));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RookViewModel.NormalTurn));
            firstInfo.AddRow("Trump", nameof(RookViewModel.TrumpSuit)); //if trump is not needed, then comment.
            firstInfo.AddRow("Status", nameof(RookViewModel.Status));
            var thisBut = GetGamingButton("Choose Nest Cards", nameof(RookViewModel.NestCommand));
            var thisBind = new Binding(nameof(RookViewModel.NestVisible));
            thisBut.SetBinding(IsVisibleProperty, thisBind);
            thisStack.Children.Add(thisBut);
            thisStack.Children.Add(_playerHand);
            thisStack.Children.Add(_dummy1);
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_bid1);
            otherStack.Children.Add(_trump1);
            otherStack.Children.Add(_trick1);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<RookViewModel>();
            OurContainer!.RegisterType<DeckViewModel<RookCardInformation>>(true);
            OurContainer.RegisterType<BasicGameLoader<RookPlayerItem, RookSaveInfo>>();
            OurContainer.RegisterType<GenericCardShuffler<RookCardInformation>>();
            OurContainer.RegisterSingleton<IDeckCount, RookDeckCount>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("");
            OurContainer.RegisterType<SmallWidthHeight>();
            OurContainer.RegisterType<StandardPickerSizeClass>();
        }
    }
}