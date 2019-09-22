using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
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
using DominosRegularCP;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DominosRegularWPF
{
    public class GamePage : MultiPlayerWindow<DominosRegularViewModel, DominosRegularPlayerItem, DominosRegularSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            DominosRegularSaveInfo saveRoot = OurContainer!.Resolve<DominosRegularSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            ThisMod.BoneYard!.MaxSize = new SKSize(750, 350);
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
            _playerHandWPF!.UpdateList(ThisMod.PlayerHand1!);
            return Task.CompletedTask;
        }
        private BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>,
            DominosBasicShuffler<SimpleDominoInfo>>? _thisBone;
        private BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>? _playerHandWPF;
        private ScoreBoardWPF? _thisScore;
        private GameBoardUI? _gameBoard1;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            _thisBone = new BoneYardWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>, DominosBasicShuffler<SimpleDominoInfo>>();
            _playerHandWPF = new BaseHandWPF<SimpleDominoInfo, ts, DominosWPF<SimpleDominoInfo>>();
            _thisScore = new ScoreBoardWPF();
            _gameBoard1 = new GameBoardUI();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            _thisBone.Height = 300;
            _thisBone.Width = 800;
            thisStack.Children.Add(_thisBone);
            thisStack.Children.Add(_gameBoard1);
            thisStack.Children.Add(_playerHandWPF);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(DominosRegularViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(DominosRegularViewModel.Status));
            thisStack.Children.Add(firstInfo.GetContent);
            _thisScore.AddColumn("Total Score", true, nameof(DominosRegularPlayerItem.TotalScore));
            _thisScore.AddColumn("Dominos Left", true, nameof(DominosRegularPlayerItem.ObjectCount)); // if not important, can just comment
            thisStack.Children.Add(_thisScore);
            Button endTurn = GetGamingButton("End Turn", nameof(DominosRegularViewModel.EndTurnCommand));
            endTurn.HorizontalAlignment = HorizontalAlignment.Left;
            thisStack.Children.Add(endTurn);
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