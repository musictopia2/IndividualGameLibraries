using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.StandardImplementations.XamarinForms.Interfaces;;
using BaseGPXPagesAndControlsXF.BasePageProcesses.Pages;
using BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers;
using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using HeapSolitaireCP;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace HeapSolitaireXF
{
    public class GamePage : SinglePlayerGamePage<HeapSolitaireViewModel>
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
        protected override async Task AfterGameButtonAsync()
        {
            StackLayout thisStack = new StackLayout();
            GameButton!.HorizontalOptions = LayoutOptions.Start;
            GameButton.VerticalOptions = LayoutOptions.Center;
            var thisMain = new BasicMultiplePilesXF<HeapSolitaireCardInfo, ts, DeckOfCardsXF<HeapSolitaireCardInfo>>();
            thisMain.Margin = new Thickness(5, 5, 5, 5);
            thisStack.Children.Add(thisMain);
            StackLayout otherStack = new StackLayout();
            otherStack.Orientation = StackOrientation.Horizontal;
            CustomWasteUI thisWaste = new CustomWasteUI();
            otherStack.Children.Add(thisWaste);
            thisStack.Children.Add(otherStack);
            StackLayout finalStack = new StackLayout();
            SimpleLabelGridXF scoresAlone = new SimpleLabelGridXF();
            scoresAlone.AddRow("Score", nameof(HeapSolitaireViewModel.Score));
            finalStack.Children.Add(scoresAlone.GetContent);
            finalStack.Children.Add(GameButton);
            otherStack.Children.Add(finalStack);
            Content = thisStack; //if not doing this, rethink.
            await ThisMod!.StartNewGameAsync();
            thisMain.Init(ThisMod.Main1!, ts.TagUsed);
            thisWaste.Init(ThisMod);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<HeapSolitaireViewModel>(); //go ahead and use the custom processes for this.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<HeapSolitaireCardInfo>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
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
                    return 0.95f;
                return 1.4f;
            }
        }
    }
}