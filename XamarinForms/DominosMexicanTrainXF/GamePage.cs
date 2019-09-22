using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using DominosMexicanTrainCP;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DominosMexicanTrainXF
{
    public class GamePage : MultiPlayerPage<DominosMexicanTrainViewModel, DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            DominosMexicanTrainSaveInfo saveRoot = OurContainer!.Resolve<DominosMexicanTrainSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHand!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            ThisMod.BoneYard!.MaxSize = new SKSize(600, 600);
            ThisMod.BoneYard.ScatterPieces();
            _thisBone!.LoadList(ThisMod.BoneYard, ts.TagUsed);
            _playerTrain!.LoadList(ThisMod.PrivateTrain1!, ts.TagUsed);
            _trainG.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            DominosMexicanTrainSaveInfo saveRoot = OurContainer!.Resolve<DominosMexicanTrainSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _thisBone!.UpdateList(ThisMod!.BoneYard!);
            _playerHand!.UpdateList(ThisMod.PlayerHand1!);
            _playerTrain!.UpdateList(ThisMod.PrivateTrain1!);
            return Task.CompletedTask;
        }
        private BoneYardXF<MexicanDomino, ts, DominosXF<MexicanDomino>,
            DominosBasicShuffler<MexicanDomino>>? _thisBone;
        private BaseHandXF<MexicanDomino, ts, DominosXF<MexicanDomino>>? _playerHand;
        private ScoreBoardXF? _thisScore;
        private BaseHandXF<MexicanDomino, ts, DominosXF<MexicanDomino>>? _playerTrain;
        private readonly TrainStationXF _trainG = new TrainStationXF(); //has to be new because of linking issues.
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            RoundButton!.HorizontalOptions = LayoutOptions.Center;
            RoundButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            _thisBone = new BoneYardXF<MexicanDomino, ts, DominosXF<MexicanDomino>, DominosBasicShuffler<MexicanDomino>>();
            if (ScreenUsed == EnumScreen.LargeTablet)
            {
                _thisBone.HeightRequest = 400;
                _thisBone.WidthRequest = 600;
            }
            else if (ScreenUsed == EnumScreen.SmallTablet)
            {
                _thisBone.HeightRequest = 250;
                _thisBone.WidthRequest = 400;
            }
            _playerHand = new BaseHandXF<MexicanDomino, ts, DominosXF<MexicanDomino>>();
            _thisScore = new ScoreBoardXF();
            _playerTrain = new BaseHandXF<MexicanDomino, ts, DominosXF<MexicanDomino>>();
            thisStack.Children.Add(_playerHand);
            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 1);
            AddAutoColumns(otherGrid, 1);
            thisStack.Children.Add(otherGrid);
            StackLayout finalStack = new StackLayout();
            AddControlToGrid(otherGrid, finalStack, 0, 0);
            _trainG.Margin = new Thickness(5, 5, 5, 5);
            _trainG.HorizontalOptions = LayoutOptions.Start;
            _trainG.VerticalOptions = LayoutOptions.Start;
            AddControlToGrid(otherGrid, _trainG, 0, 1);
            finalStack.Children.Add(_thisBone);
            _playerTrain.HorizontalOptions = LayoutOptions.Start;
            finalStack.Children.Add(_playerTrain);
            StackLayout tempstack = new StackLayout();
            tempstack.Orientation = StackOrientation.Horizontal;
            finalStack.Children.Add(tempstack);
            Button endbutton = GetGamingButton("End Turn", nameof(DominosMexicanTrainViewModel.EndTurnCommand));
            endbutton.HorizontalOptions = LayoutOptions.Start;
            tempstack.Children.Add(endbutton);
            Button thisButton = GetGamingButton("Longest Train", nameof(DominosMexicanTrainViewModel.LongestTrainCommand));
            tempstack.Children.Add(thisButton);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(DominosMexicanTrainViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DominosMexicanTrainViewModel.Status));
            _thisScore.AddColumn("Dominos Left", true, nameof(DominosMexicanTrainPlayerItem.ObjectCount));
            _thisScore.AddColumn("Total Score", true, nameof(DominosMexicanTrainPlayerItem.TotalScore));
            _thisScore.AddColumn("Previous Score", true, nameof(DominosMexicanTrainPlayerItem.PreviousScore));
            _thisScore.AddColumn("# Previous", true, nameof(DominosMexicanTrainPlayerItem.PreviousLeft));
            finalStack.Children.Add(_thisScore);
            finalStack.Children.Add(firstInfo.GetContent);
            AddRestoreCommand(finalStack); //usually to this.  can be to another control if needed.
            MainGrid!.Children.Add(thisStack); //this was forgotten from the templates.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<DominosMexicanTrainViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportions>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, CustomProportions>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<MexicanDomino>>(true);
            OurContainer.RegisterSingleton<IDeckCount, MexicanDomino>();
            OurContainer.RegisterSingleton<IProportionBoard, CustomProportions>();
            OurContainer.RegisterSingleton(_trainG.ThisElement, "");
        }
    }
    public class CustomProportions : IProportionImage, IProportionBoard
    {
        public float Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 1.3f;
                return .85f;
            }
        }
    }
}