using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using SkiaSharp;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using TileRummyCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.BasicGameDataClasses;
namespace TileRummyXF
{
    public class GamePage : MultiPlayerPage<TileRummyViewModel, TileRummyPlayerItem, TileRummySaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            TileRummySaveInfo saveRoot = OurContainer!.Resolve<TileRummySaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList); // the data should come from the playeritem.
            _hand1!.LoadList(ThisMod!.PlayerHand1!, "");
            ThisMod.Pool1!.MaxSize = new SKSize(1000, 1000); //hopefully this works as well.
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
        private ScoreBoardXF? _thisScore;
        BaseHandXF<TileInfo, TileCP, TileXF>? _hand1;
        private BoneYardXF<TileInfo, TileCP, TileXF,
            TileShuffler>? _pool1;
        private TempRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF>? _tempG;
        private MainRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF, TileSet, SavedSet>? _mainG;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            _thisScore = new ScoreBoardXF();
            _hand1 = new BaseHandXF<TileInfo, TileCP, TileXF>();
            _pool1 = new BoneYardXF<TileInfo, TileCP, TileXF, TileShuffler>();
            _tempG = new TempRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF>();
            _mainG = new MainRummySetsXF<EnumColorType, EnumColorType, TileInfo, TileCP, TileXF, TileSet, SavedSet>();
            var endButton = GetSmallerButton("End Turn", nameof(TileRummyViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            var firstButton = GetSmallerButton("Create First Sets", nameof(TileRummyViewModel.CreateFirstSetsCommand));
            var otherButton = GetSmallerButton("Create New Set", nameof(TileRummyViewModel.CreateNewSetCommand));
            var undoButton = GetSmallerButton("Reset Moves", nameof(TileRummyViewModel.UndoMoveCommand));
            _thisScore.AddColumn("Tiles Left", true, nameof(TileRummyPlayerItem.ObjectCount));
            _thisScore.AddColumn("Score", true, nameof(TileRummyPlayerItem.Score));
            if (ScreenUsed == EnumScreen.SmallTablet)
                _tempG.HeightRequest = 120;
            else
                _tempG.HeightRequest = 200;
            _pool1.WidthRequest = 250;
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(TileRummyViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(TileRummyViewModel.Status));
            var firstContent = firstInfo.GetContent;
            Grid completeGrid = new Grid();
            AddLeftOverRow(completeGrid, 55);
            AddAutoRows(completeGrid, 1);
            AddLeftOverRow(completeGrid, 50);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(_pool1);
            thisStack.Children.Add(GameButton);
            thisStack.Children.Add(firstButton);
            thisStack.Children.Add(otherButton);
            thisStack.Children.Add(undoButton);
            thisStack.Children.Add(endButton);
            otherStack.Children.Add(thisStack);
            thisStack = new StackLayout();
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