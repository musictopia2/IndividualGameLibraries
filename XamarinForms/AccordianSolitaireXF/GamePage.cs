using AccordianSolitaireCP;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.GameFrames;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace AccordianSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<AccordianSolitaireViewModel>, IRedo
    {
        public GamePage(IGamePlatform customPlatform, IStartUp starts, EnumGamePackageMode mode) : base(customPlatform, starts, mode) { }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private BaseHandXF<AccordianSolitaireCardInfo, ts, DeckOfCardsXF<AccordianSolitaireCardInfo>>? _thisBoard;
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            SimpleLabelGridXF thisLabel = new SimpleLabelGridXF();
            thisLabel.AddRow("Score", nameof(AccordianSolitaireViewModel.Score));
            _thisBoard = new BaseHandXF<AccordianSolitaireCardInfo, ts, DeckOfCardsXF<AccordianSolitaireCardInfo>>();
            _thisBoard.Divider = 1.5f;
            _thisBoard.LoadList(ThisMod!.GameBoard1!, ts.TagUsed);
            thisStack.Children.Add(_thisBoard);
            thisStack.Children.Add(thisLabel.GetContent);
            thisStack.Children.Add(GameButton);
            Content = thisStack; //if not doing this, rethink.
            await ThisMod.StartNewGameAsync();
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<AccordianSolitaireViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<AccordianSolitaireCardInfo>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
        }
        void IRedo.RedoList()
        {
            _thisBoard!.RedoCards();
        }
    }
    public class CustomProportion : IProportionImage
    {
        float IProportionImage.Proportion
        {
            get
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    return .45f;
                if (ScreenUsed == EnumScreen.SmallTablet)
                    return .90f;
                return 1.2f;
            }
        }
    }
}