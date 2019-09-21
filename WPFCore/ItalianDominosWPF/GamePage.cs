using AndyCristinaGamePackageCP.CommonProportionClasses;
using AndyCristinaGamePackageCP.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Dominos;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dominos;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using ItalianDominosCP;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace ItalianDominosWPF
{
    public class GamePage : MultiPlayerWindow<ItalianDominosViewModel, ItalianDominosPlayerItem, ItalianDominosSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            ItalianDominosSaveInfo saveRoot = OurContainer!.Resolve<ItalianDominosSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            ThisMod.BoneYard!.MaxSize = new SKSize(750, 350);
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
        private BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>>? _thisBone;
        private BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>? _playerHandWPF;
        private ScoreBoardWPF? _thisScore;
        protected async override void AfterGameButton()
        {
            _thisBone = new BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandWPF = new BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            _thisScore = new ScoreBoardWPF();
            StackPanel thisStack = new StackPanel();
            BasicSetUp(); //forgot this part.
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisBone.Height = 400;
            _thisBone.Width = 800;
            thisStack.Children.Add(_thisBone);
            thisStack.Children.Add(_playerHandWPF);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(ItalianDominosViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(ItalianDominosViewModel.Status));
            firstInfo.AddRow("Up To", nameof(ItalianDominosViewModel.UpTo));
            firstInfo.AddRow("Next #", nameof(ItalianDominosViewModel.NextNumber));
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            var thisBut = GetGamingButton("Play", nameof(ItalianDominosViewModel.PlayCommand));
            otherStack.Children.Add(thisBut);
            _thisScore.AddColumn("Total Score", true, nameof(ItalianDominosPlayerItem.TotalScore), rightMargin: 10);
            _thisScore.AddColumn("Dominos Left", true, nameof(ItalianDominosPlayerItem.ObjectCount), rightMargin: 10); // if not important, can just comment
            _thisScore.AddColumn("Drew Yet", true, nameof(ItalianDominosPlayerItem.DrewYet), useTrueFalse: true);
            otherStack.Children.Add(firstInfo.GetContent);
            otherStack.Children.Add(_thisScore);
            thisStack.Children.Add(otherStack); // this may be missing as well.
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