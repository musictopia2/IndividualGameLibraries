using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.Misc;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Dice;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.Dice;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using ThinkTwiceCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.BasicGameDataClasses;
namespace ThinkTwiceXF
{
    public class GamePage : MultiPlayerPage<ThinkTwiceViewModel, ThinkTwicePlayerItem, ThinkTwiceSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            ThinkTwiceSaveInfo saveRoot = OurContainer!.Resolve<ThinkTwiceSaveInfo>();
            _thisScore!.LoadLists(saveRoot.PlayerList);
            _diceControl!.LoadDiceViewModel(ThisMod!.ThisCup!);
            _score1 = OurContainer.Resolve<ScoreViewModel>();
            CategoriesDice thisCat = OurContainer.Resolve<CategoriesDice>(); //i think
            Multiplier thisMul = OurContainer.Resolve<Multiplier>();
            _otherScore!.LoadList(_score1);
            CategoryXF firstCon = new CategoryXF();
            firstCon.SendDiceInfo(thisCat);
            
            firstCon.Margin = new Thickness(0, 0, 0, 0); //well see on tablets.
            _multStack!.Children.Add(firstCon);
            MultXF secondCon = new MultXF();
            secondCon.SendDiceInfo(thisMul);
            secondCon.Margin = new Thickness(0, 0, 0, 0);
            _multStack.Children.Add(secondCon);
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            ThinkTwiceSaveInfo saveRoot = OurContainer!.Resolve<ThinkTwiceSaveInfo>();
            _thisScore!.UpdateLists(saveRoot.PlayerList);
            _diceControl!.UpdateDice(ThisMod!.ThisCup!);
            return Task.CompletedTask;
        }
        ScoreBoardXF? _thisScore;
        DiceListControlXF<SimpleDice>? _diceControl; //i think.
        private ScoreViewModel? _score1;
        private StackLayout? _multStack;
        private ScoreXF? _otherScore;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            MainGrid!.Children.Add(thisStack);
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            _otherScore = new ScoreXF();
            if (ScreenUsed != EnumScreen.SmallPhone)
                thisStack.Children.Add(_otherScore);
            _multStack = new StackLayout();
            _multStack.Orientation = StackOrientation.Horizontal;
            if (ScreenUsed != EnumScreen.SmallPhone)
                thisStack.Children.Add(_multStack);
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                StackLayout tempStack = new StackLayout();
                _multStack.VerticalOptions = LayoutOptions.End;
                tempStack.Orientation = StackOrientation.Horizontal;
                tempStack.Children.Add(_otherScore);
                tempStack.Children.Add(_multStack);
                thisStack.Children.Add(tempStack);
                _multStack.Spacing = 2;
            }
            var thisRoll = GetSmallerButton("Roll Dice", nameof(ThinkTwiceViewModel.RollCommand));
            _diceControl = new DiceListControlXF<SimpleDice>();
            thisStack.Children.Add(_diceControl);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            otherStack.Children.Add(thisRoll);
            var endButton = GetSmallerButton("End Turn", nameof(ThinkTwiceViewModel.EndTurnCommand));
            endButton.HorizontalOptions = LayoutOptions.Start;
            otherStack.Children.Add(endButton);
            var thisBut = GetSmallerButton("Roll Multiplier Dice", nameof(ThinkTwiceViewModel.RollMultCommand));
            otherStack.Children.Add(thisBut);
            thisStack.Children.Add(otherStack);
            _thisScore = new ScoreBoardXF();
            _thisScore.AddColumn("Score Round", true, nameof(ThinkTwicePlayerItem.ScoreRound));
            _thisScore.AddColumn("Score Game", true, nameof(ThinkTwicePlayerItem.ScoreGame));
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(ThinkTwiceViewModel.NormalTurn)); // there is no roll number needed for this game.
            firstInfo.AddRow("Roll", nameof(ThinkTwiceViewModel.RollNumber)); //if you don't need, it comment it out.
            firstInfo.AddRow("Category", nameof(ThinkTwiceViewModel.CategoryChosen));
            firstInfo.AddRow("Score", nameof(ThinkTwiceViewModel.Score));
            thisStack.Children.Add(firstInfo.GetContent);
            thisStack.Children.Add(_thisScore);
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<ThinkTwicePlayerItem, ThinkTwiceSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<ThinkTwiceViewModel>();
            OurContainer.RegisterType<StandardRollProcesses<SimpleDice, ThinkTwicePlayerItem>>();
            OurContainer.RegisterSingleton<IProportionImage, StandardProportion>(StandardDiceXF.GetDiceTag);
            OurContainer.RegisterSingleton<IGenerateDice<int>, SimpleDice>();
        }
    }
}