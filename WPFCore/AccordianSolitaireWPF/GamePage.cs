using AccordianSolitaireCP;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BaseGPXWindowsAndControlsCore.BasicControls.GameFrames;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.CommonInterfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
namespace AccordianSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<AccordianSolitaireViewModel>, IRedo
    {
        public GamePage(IStartUp starts, EnumGamePackageMode mode) //this means something needs to put into here.
        {
            BuildXAML(starts, mode);
        }
        public override Task HandleAsync(LoadEventModel message)
        {
            return Task.CompletedTask;
        }
        public override Task HandleAsync(UpdateEventModel message)
        {
            return Task.CompletedTask;
        }
        private BaseHandWPF<AccordianSolitaireCardInfo, ts, DeckOfCardsWPF<AccordianSolitaireCardInfo>>? _thisBoard;
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            SimpleLabelGrid thisLabel = new SimpleLabelGrid();
            thisLabel.AddRow("Score", nameof(AccordianSolitaireViewModel.Score));
            _thisBoard = new BaseHandWPF<AccordianSolitaireCardInfo, ts, DeckOfCardsWPF<AccordianSolitaireCardInfo>>();
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
            OurContainer!.RegisterNonSavedClasses<AccordianSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
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
        float IProportionImage.Proportion => 1.7f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}