using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using RummyDiceCP;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper; //just in case
namespace RummyDiceXF
{
    public class GamePage : MultiPlayerPage<RummyDiceViewModel, RummyDicePlayerItem, RummyDiceSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            if (_mainGame!.TempSets!.Count == 0)
                throw new BasicBlankException("You need to have 2 sets, not none for tempsets");
            _mainGame.TempSets.ForEach(tempCP =>
            {
                RummyDiceHandXF thisTemp = _tempList.Single(items => items.Index == tempCP.Index);
                thisTemp.LoadList(tempCP);
            });
            _diceControl!.LoadDiceViewModel(_mainGame);
            _thisScore!.LoadLists(_mainGame.PlayerList!); // i think
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            _mainGame!.TempSets!.ForEach(tempCP =>
            {
                RummyDiceHandXF thisTemp = _tempList.Single(items => items.Index == tempCP.Index);
                thisTemp.UpdateList(tempCP);
            });
            _diceControl!.UpdateDiceViewModel(_mainGame);
            _thisScore!.UpdateLists(_mainGame.PlayerList!);
            return Task.CompletedTask;
        }
        RummyDiceMainGameClass? _mainGame;
        readonly CustomBasicList<RummyDiceHandXF>? _tempList = new CustomBasicList<RummyDiceHandXF>();
        private ScoreBoardXF? _thisScore;
        private RummyDiceListXF? _diceControl;
        protected override async Task AfterGameButtonAsync() //could be forced to use grid (well see).
        {
            Grid tempGrid = new Grid();
            AddLeftOverRow(tempGrid, 50);
            AddLeftOverRow(tempGrid, 50);
            AddAutoRows(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            AddAutoColumns(tempGrid, 1);
            StackLayout thisStack = new StackLayout();
            RummyDiceHandXF thisTemp = new RummyDiceHandXF();
            thisTemp.Index = 1;
            AddControlToGrid(tempGrid, thisTemp, 0, 0);
            _tempList!.Add(thisTemp); //forgot this too.
            thisTemp = new RummyDiceHandXF();
            thisTemp.Index = 2;
            AddControlToGrid(tempGrid, thisTemp, 1, 0);
            _tempList.Add(thisTemp);
            _mainGame = OurContainer!.Resolve<RummyDiceMainGameClass>();
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(tempGrid);
            _thisScore = new ScoreBoardXF();
            AddControlToGrid(tempGrid, _thisScore, 0, 1);
            Grid.SetRowSpan(_thisScore, 3);
            SimpleLabelGridXF otherScore = new SimpleLabelGridXF();
            otherScore.AddRow("Roll", nameof(RummyDiceViewModel.RollNumber), new RollConverter());
            otherScore.AddRow("Score", nameof(RummyDiceViewModel.Score));
            tempGrid = otherScore.GetContent;
            tempGrid.VerticalOptions = LayoutOptions.End; // try this way
            var otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(tempGrid);
            Button thisBut;
            thisBut = GetGamingButton("Put Back", nameof(RummyDiceViewModel.BoardCommand));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            _thisScore.AddColumn("Score Round", false, nameof(RummyDicePlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", false, nameof(RummyDicePlayerItem.ScoreGame));
            _thisScore.AddColumn("Phase", false, nameof(RummyDicePlayerItem.Phase));
            _thisScore.VerticalOptions = LayoutOptions.Start;
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            thisStack.Children.Add(otherStack);
            _diceControl = new RummyDiceListXF();
            thisBut = GetGamingButton("Roll", nameof(RummyDiceViewModel.RollCommand));
            thisBut.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(thisBut);
            otherStack.Children.Add(_diceControl);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(RummyDiceViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(RummyDiceViewModel.Status));
            firstInfo.AddRow("Phase", nameof(RummyDiceViewModel.CurrentPhase));
            otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            StackLayout bottomStack = new StackLayout();
            otherStack.Children.Add(bottomStack);
            var endButton = GetGamingButton("End Turn", nameof(RummyDiceViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            endButton.Margin = new Thickness(5, 0, 0, 20);
            bottomStack.Children.Add(endButton);
            thisBut = GetGamingButton("Score Hand", nameof(RummyDiceViewModel.CheckCommand));
            thisBut.Margin = new Thickness(5, 0, 0, 0);
            bottomStack.Children.Add(thisBut);
            otherStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(otherStack);
            otherStack = new StackLayout();
            thisStack.Children.Add(otherStack);
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(GameButton);
            MainGrid!.Children.Add(thisStack);
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<RummyDicePlayerItem, RummyDiceSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<RummyDiceViewModel>();
            OurContainer.RegisterSingleton<IGenerateDice<int>, RummyDiceInfo>();
        }
    }
}