using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BasicControlsAndWindowsCore.BasicWindows.Windows;
using BasicControlsAndWindowsCore.BasicWindows.Misc;
using CommonBasicStandardLibraries.CollectionClasses; //just in case i want to use the new custom classes.
using BaseGPXWindowsAndControlsCore.BaseWindows;
using BakersDozenSolitaireCP;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.Extensions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //just in case
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using BasicGameFramework.StandardImplementations.CrossPlatform.ExtensionClasses;
using BasicGameFramework.CommonInterfaces;
using BaseGPXWindowsAndControlsCore.BasicControls.SingleCardFrames;
using ts = BasicGameFramework.GameGraphicsCP.Cards.DeckOfCardsCP;
using BaseGPXWindowsAndControlsCore.GameGraphics.Cards;
using BasicGameFramework.GameGraphicsCP.Interfaces;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.RegularDeckOfCards;
using BaseSolitaireClassesCP.Cards;
using BaseGPXWindowsAndControlsCore.BasicControls.MultipleFrameContainers;
using SolitaireGraphicsWPFCore;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicGameDataClasses;
namespace BakersDozenSolitaireWPF
{
    public class GamePage : SinglePlayerWindow<BakersDozenSolitaireViewModel>
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
        protected async override void AfterGameButton()
        {
            StackPanel thisStack = new StackPanel();
            GameButton!.HorizontalAlignment = HorizontalAlignment.Left;
            GameButton.VerticalAlignment = VerticalAlignment.Center;
            StackPanel otherStack = new StackPanel();
            otherStack.Orientation = Orientation.Horizontal;
            var thisMain = new BasicMultiplePilesWPF<SolitaireCard, ts, DeckOfCardsWPF<SolitaireCard>>();
            thisMain.Margin = new Thickness(10, 5, 5, 5);
            thisStack.Children.Add(otherStack);
            var autoBut = GetGamingButton("Auto Make Move", nameof(BakersDozenSolitaireViewModel.AutoMoveCommand));
            autoBut.HorizontalAlignment = HorizontalAlignment.Left;
            var scoresAlone = new SimpleLabelGrid();
            scoresAlone.AddRow("Score", nameof(BakersDozenSolitaireViewModel.Score));
            var tempGrid = scoresAlone.GetContent;
            StackPanel finalStack = new StackPanel();
            otherStack.Children.Add(finalStack);
            otherStack.Children.Add(thisMain);
            finalStack.Children.Add(tempGrid);
            finalStack.Children.Add(GameButton);
            finalStack.Children.Add(autoBut);
            var thisWaste = new SolitairePilesWPF();
            thisStack.Children.Add(thisWaste);
            Content = thisStack;
            await ThisMod!.StartNewGameAsync();
            var tempWaste = (WastePiles)ThisMod.WastePiles1!;
            thisWaste.Init(tempWaste.Piles);
            var tempMain = (MainPilesCP)ThisMod.MainPiles1!;
            thisMain.Init(tempMain.Piles, ts.TagUsed);
            ThisMod.CommandContainer!.IsExecuting = false;
        }
        protected override void RegisterInterfaces()
        {
            OurContainer!.RegisterNonSavedClasses<BakersDozenSolitaireViewModel>(); //go ahead and use the custom processes for this.  decided to mention non saved classes.
            OurContainer!.RegisterSingleton<IProportionImage, CustomProportion>(ts.TagUsed);
            OurContainer.RegisterType<DeckViewModel<SolitaireCard>>(true); //i think
            OurContainer.RegisterSingleton<IDeckCount, CustomDeck>(); //forgot to use a custom deck for this one.
            OurContainer.RegisterSingleton<IRegularAceCalculator, RegularLowAceCalculator>(); //most of the time, aces are low.
            OurContainer.RegisterType<WastePiles>(); //can't do automatically because we don't know if we will do it or not.
            OurContainer.RegisterType<MainPilesCP>();
        }
    }
    public class CustomProportion : IProportionImage //tablet has to be different too on that game.
    {
        float IProportionImage.Proportion => 2.2f; //2.3 was standard size.  you can either increase or decrease as needed.
    }
}