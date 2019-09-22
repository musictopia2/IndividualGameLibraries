using BasicGameFramework.StandardImplementations.CrossPlatform.CommonProportionClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.MultiplayerClasses.LoadingClasses;
using MancalaCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using CommonBasicStandardLibraries.Exceptions;
using BasicGameFramework.BasicGameDataClasses;
namespace MancalaXF
{
    public class GamePage : MultiPlayerPage<MancalaViewModel, MancalaPlayerItem, MancalaSaveInfo>
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }

        public override Task HandleAsync(LoadEventModel message)
        {
            _thisBoard.LoadBoard();
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        readonly GameBoardXF _thisBoard = new GameBoardXF();
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout(); //will usually start with a stack panel.  if i am wrong, rethink
            BasicSetUp();
            GameButton!.HorizontalOptions = LayoutOptions.Center;
            GameButton.VerticalOptions = LayoutOptions.Center;
            thisStack.Children.Add(GameButton);
            SimpleLabelGridXF firstInfo = new SimpleLabelGridXF();
            firstInfo.AddRow("Turn", nameof(MancalaViewModel.NormalTurn));
            firstInfo.AddRow("Status", nameof(MancalaViewModel.Status));
            firstInfo.AddRow("Instructions", nameof(MancalaViewModel.Instructions));
            thisStack.Children.Add(_thisBoard);
            thisStack.Children.Add(firstInfo.GetContent);
            MainGrid!.Children.Add(thisStack); //maybe i forgot that too.
            AddRestoreCommand(thisStack); //usually to this.  can be to another control if needed.
            await FinishUpAsync();
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterType<BasicGameLoader<MancalaPlayerItem, MancalaSaveInfo>>(); //i think basic game loader gets done here still.
            OurContainer!.RegisterNonSavedClasses<MancalaViewModel>();
            OurContainer.RegisterSingleton(_thisBoard.ThisElement, "");
            OurContainer.RegisterSingleton<IProportionBoard, CustomProportion>(""); //here too.
        }
    }
    public class CustomProportion : IProportionBoard
    {
        float IProportionBoard.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.Desktop)
                    return 2.3f; //uwp may require something different but don't know yet.
                if (ScreenUsed == EnumScreen.LargeTablet)
                    return 3.5f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return 2.6f;
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return 1.3f;
                throw new BasicBlankException("Screen not supported");
            }
        }
    }
}