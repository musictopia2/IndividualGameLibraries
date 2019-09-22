using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.Misc;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TileRummyCP;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
namespace TileRummyWPF
{
    public class GamePage : MultiPlayerWindow<TileRummyViewModel, TileRummyPlayerItem, TileRummySaveInfo>
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            TileRummySaveInfo saveRoot = OurContainer!.Resolve<TileRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList); // the data should come from the playeritem.
            _hand1!.LoadList(ThisMod!.PlayerHand1!, "");
            ThisMod.Pool1!.MaxSize = new SKSize(1000, 1000);
            ThisMod.Pool1.ScatterPieces();
            _pool1!.LoadList(ThisMod.Pool1, "");
            _tempG!.Init(ThisMod.TempSets!, "");
            _mainG!.Init(ThisMod.MainSets1!, "");
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            TileRummySaveInfo saveRoot = OurContainer!.Resolve<TileRummySaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _pool1!.UpdateList(ThisMod!.Pool1!);
            _hand1!.UpdateList(ThisMod.PlayerHand1!);
            _tempG!.Update(ThisMod.TempSets!);
            _mainG!.Update(ThisMod.MainSets1!);
            return Task.CompletedTask;
        }
        private ScoreBoardWPF? _thisScore;
        BaseHandWPF<TileInfo, TileCP, TileWPF>? _hand1;
        private BoneYardWPF<TileInfo, TileCP, TileWPF,
            TileShuffler>? _pool1;
        private TempRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF>? _tempG;
        private MainRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF, TileSet, SavedSet>? _mainG;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Center;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            _thisScore = new ScoreBoardWPF();
            _hand1 = new BaseHandWPF<TileInfo, TileCP, TileWPF>();
            _pool1 = new BoneYardWPF<TileInfo, TileCP, TileWPF, TileShuffler>();
            _tempG = new TempRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF>();
            _mainG = new MainRummySetsWPF<EnumColorType, EnumColorType, TileInfo, TileCP, TileWPF, TileSet, SavedSet>();
            var endButton = GetGamingButton("End Turn", nameof(TileRummyViewModel.EndTurnCommand));
            endButton.HorizontalAlignment = HorizontalAlignment.Left;
            var firstButton = GetGamingButton("Create First Sets", nameof(TileRummyViewModel.CreateFirstSetsCommand));
            var otherButton = GetGamingButton("Create New Set", nameof(TileRummyViewModel.CreateNewSetCommand));
            var undoButton = GetGamingButton("Reset Moves", nameof(TileRummyViewModel.UndoMoveCommand));
            _thisScore.AddColumn("Tiles Left", true, nameof(TileRummyPlayerItem.ObjectCount));
            _thisScore.AddColumn("Score", true, nameof(TileRummyPlayerItem.Score));

            _tempG.Height = 250;
            _pool1.Width = 300; // well see.
            SimpleLabelGrid firstInfo = new SimpleLabelGrid();
            firstInfo.AddRow("Turn", nameof(TileRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TileRummyViewModel.Status));
            var firstContent = firstInfo.GetContent;
            Grid completeGrid = new Grid();
            AddLeftOverRow(completeGrid, 50);
            AddAutoRows(completeGrid, 1);
            AddLeftOverRow(completeGrid, 50);
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            otherStack.Children.Add(_pool1);
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(firstButton);
            thisStack.Children.Add(otherButton);
            thisStack.Children.Add(undoButton);
            thisStack.Children.Add(endButton);
            otherStack.Children.Add(thisStack);
            thisStack = new StackPanel();
            thisStack.Children.Add(_tempG);
            thisStack.Children.Add(firstContent);
            thisStack.Children.Add(_thisScore);
            otherStack.Children.Add(thisStack);
            AddControlToGrid(completeGrid, otherStack, 0, 0);
            AddControlToGrid(completeGrid, _hand1, 1, 0);
            _pool1.Margin = new Thickness(2, 2, 2, 2);
            AddControlToGrid(completeGrid, _mainG, 2, 0); // used the wrong one.
            MainGrid!.Children.Add(completeGrid);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<TileRummyPlayerItem, TileRummySaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<TileRummyViewModel>();
            OurContainer.RegisterType<TileShuffler>();
            OurContainer.RegisterSingleton<IDeckCount, TileCountClass>();
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("");
            OurContainer.RegisterSingleton<IProportionImage, CustomProportion>("scatter");
        }
    }
}