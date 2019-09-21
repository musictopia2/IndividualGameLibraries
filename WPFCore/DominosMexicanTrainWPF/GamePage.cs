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
using DominosMexicanTrainCP;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using ts = BasicGameFramework.GameGraphicsCP.Dominos.DominosCP;
namespace DominosMexicanTrainWPF
{
    public class GamePage : MultiPlayerWindow<DominosMexicanTrainViewModel, DominosMexicanTrainPlayerItem, DominosMexicanTrainSaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            DominosMexicanTrainSaveInfo saveRoot = OurContainer!.Resolve<DominosMexicanTrainSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _playerHandWPF!.LoadList(ThisMod!.PlayerHand1!, ts.TagUsed);
            ThisMod.BoneYard!.MaxSize = new SKSize(1000, 1000);
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
            _playerHandWPF!.UpdateList(ThisMod.PlayerHand1!);
            _playerTrain!.UpdateList(ThisMod.PrivateTrain1!);
            return Task.CompletedTask;
        }
        private BoneYardWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>,
            DominosBasicShuffler<MexicanDomino>>? _thisBone;
        private BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>>? _playerHandWPF;
        private ScoreBoardWPF? _thisScore;
        private BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>>? _playerTrain;
        private readonly TrainStationWPF _trainG = new TrainStationWPF(); //has to be new because of linking issues.
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            RoundButton!.HorizontalAlignment = HorizontalAlignment.Center;
            RoundButton.VerticalAlignment = VerticalAlignment.Center;
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(RoundButton);
            _thisBone = new BoneYardWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>, DominosBasicShuffler<MexicanDomino>>();
            _thisBone.Height = 500;
            _thisBone.Width = 1050;
            _playerHandWPF = new BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>>();
            _thisScore = new ScoreBoardWPF();
            _playerTrain = new BaseHandWPF<MexicanDomino, ts, DominosWPF<MexicanDomino>>();
            thisStack.Children.Add(_playerHandWPF);
            Grid otherGrid = new Grid();
            AddLeftOverColumn(otherGrid, 1);
            AddAutoColumns(otherGrid, 1);
            thisStack.Children.Add(otherGrid);
            StackPanel finalStack = new StackPanel();
            AddControlToGrid(otherGrid, finalStack, 0, 0);
            _trainG.Margin = new Thickness(5, 5, 5, 5);
            _trainG.HorizontalAlignment = HorizontalAlignment.Left;
            _trainG.VerticalAlignment = VerticalAlignment.Top;
            AddControlToGrid(otherGrid, _trainG, 0, 1);
            finalStack.Children.Add(_thisBone);
            _playerTrain.HorizontalAlignment = HorizontalAlignment.Left;
            finalStack.Children.Add(_playerTrain);
            StackPanel tempstack = new StackPanel();
            tempstack.Orientation = Orientation.Horizontal;
            finalStack.Children.Add(tempstack);
            Button endbutton = GetGamingButton("End Turn", nameof(DominosMexicanTrainViewModel.EndTurnCommand));
            endbutton.HorizontalAlignment = HorizontalAlignment.Left;
            tempstack.Children.Add(endbutton);
            Button thisButton = GetGamingButton("Longest Train", nameof(DominosMexicanTrainViewModel.LongestTrainCommand));
            tempstack.Children.Add(thisButton);
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
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
}