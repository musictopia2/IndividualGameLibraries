using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
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
using ItalianDominosCP;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace ItalianDominosXF
{
    public class GamePage : MultiPlayerPage<ItalianDominosViewModel, ItalianDominosPlayerItem, ItalianDominosSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            ItalianDominosSaveInfo saveRoot = OurContainer!.Resolve<ItalianDominosSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            ThisMod.BoneYard!.MaxSize = new SKSize(550, 250);
            ThisMod.BoneYard.ScatterPieces();
            _thisBone!.LoadList(ThisMod.BoneYard, ts.TagUsed);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ItalianDominosSaveInfo saveRoot = OurContainer!.Resolve<ItalianDominosSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _thisBone!.UpdateList(ThisMod!.BoneYard!);
            _playerHandWPF!.UpdateList(ThisMod.PlayerHand1!);
            return Task.CompletedTask;
        }
        private BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>>? _thisBone;
        private BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>? _playerHandWPF;
        private ScoreBoardXF? _thisScore;
        protected override async Task AfterGameButtonAsync()
        {
            _thisBone = new BoneYardXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandWPF = new BaseHandXF<SimpleDominoInfo, ts, DominosXF<SimpleDominoInfo>>();
            _thisScore = new ScoreBoardXF();
            StackLayout thisStack = new StackLayout();
            BasicSetUp(); //forgot this part.
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _thisBone.HeightRequest = 130;
            _thisBone.WidthRequest = 600;
            thisStack.Children.Add(_thisBone);
            thisStack.Children.Add(_playerHandWPF);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ItalianDominosViewModel.NormalTurn));
            firstInfo.AddRow("Up To", nameof(ItalianDominosViewModel.UpTo));
            firstInfo.AddRow("Next #", nameof(ItalianDominosViewModel.NextNumber));
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            var thisBut = GetGamingButton("Play", nameof(ItalianDominosViewModel.PlayCommand));
            otherStack.Children.Add(thisBut);
            _thisScore.AddColumn("Total Score", true, nameof(ItalianDominosPlayerItem.TotalScore), rightMargin: 10);
            _thisScore.AddColumn("Dominos Left", true, nameof(ItalianDominosPlayerItem.ObjectCount), rightMargin: 10); // if not important, can just comment
            _thisScore.AddColumn("Drew Yet", true, nameof(ItalianDominosPlayerItem.DrewYet), useTrueFalse: true);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack); // this may be missing as well.
            otherStack.Children.Add(_thisScore);
            MainGrid!.Children.Add(thisStack);
            AddRestoreCommand(thisStack); //i think.  if a stack can't be used, rethink.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ItalianDominosPlayerItem, ItalianDominosSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ItalianDominosViewModel>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(ts.TagUsed);
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>("scatter"); //this is needed so the boneyard part can work.
            OurContainer.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>(true);
            OurContainer.RegisterSingleton<IDeckCount, SimpleDominoInfo>(); //has to do this to stop overflow and duplicates bug.
        }
    }
}