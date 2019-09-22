using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using DominosRegularCP;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.BasicGameDataClasses;
namespace DominosRegularXF
{
    public class GamePage : MultiPlayerPage<DominosRegularViewModel, DominosRegularPlayerItem, DominosRegularSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            DominosRegularSaveInfo saveRoot = OurContainer!.Resolve<DominosRegularSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            ThisMod.BoneYard!.MaxSize = new SKSize(600, 600);
            ThisMod.BoneYard.ScatterPieces();
            _thisBone!.LoadList(ThisMod.BoneYard, ts.TagUsed);
            _gameBoard1!.LoadList();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DominosRegularSaveInfo SaveRoot = OurContainer!.Resolve<DominosRegularSaveInfo>();
            _thisScore!.UpdateLists(SaveRoot.PlayerList);
            _thisBone!.UpdateList(ThisMod!.BoneYard!);
            _playerHand!.UpdateList(ThisMod.PlayerHand1!);
            return Task.CompletedTask;
        }
        private BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>>? _thisBone;
        private BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>? _playerHand;
        private ScoreBoardXF? _thisScore;
        private GameBoardUI? _gameBoard1;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisBone = new BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHand = new BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            _thisScore = new ScoreBoardXF();
            _gameBoard1 = new GameBoardUI();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            if (ScreenUsed != EnumScreen.SmallPhone)
                _thisBone.HeightRequest = 300;
            else
                _thisBone.HeightRequest = 90;
            _thisBone.WidthRequest = 300;
            thisStack.Children.Add(_thisBone);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DominosRegularViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DominosRegularViewModel.Status));
            _thisScore.AddColumn("Total Score", true, nameof(DominosRegularPlayerItem.TotalScore));
            _thisScore.AddColumn("Dominos Left", true, nameof(DominosRegularPlayerItem.ObjectCount)); // if not important, can just comment
            Button endTurn = GetGamingButton("End Turn", nameof(DominosRegularViewModel.EndTurnCommand));
            endTurn.HorizontalOptions = LayoutOptions.Start;
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            otherStack.Children.Add(_gameBoard1);
            thisStack.Children.Add(_playerHand);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_thisScore);
            otherStack.Children.Add(endTurn);
            thisStack.Children.Add(otherStack);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DominosRegularPlayerItem, DominosRegularSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<DominosRegularViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>();
        }
    }
}