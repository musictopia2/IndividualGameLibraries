using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using GoFishCP.Data;
using GoFishCP.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace GoFishWPF.Views
{
    public class AskView : BaseFrameWPF, IUIView
    {
        public AskView(GoFishVMData model)
        {
            Grid parentGrid = new Grid();
            StackPanel thisStack = new StackPanel();
            EnumPickerWPF<NumberPieceCP, NumberPieceWPF, EnumCardValueList>
                thisAsk = new EnumPickerWPF<NumberPieceCP, NumberPieceWPF, EnumCardValueList>();
            thisAsk.Rows = 3;
            thisAsk.Columns = 5;
            thisAsk.LoadLists(model.AskList!);
            Text = "Choose Number To Ask";
            thisStack.Children.Add(thisAsk);
            var thisBut = GetGamingButton("Number To Ask", nameof(AskViewModel.AskAsync));
            thisStack.Children.Add(thisBut);
            var thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(thisStack, thisRect);
            parentGrid.Children.Add(ThisDraw); // maybe this is causing problems for the other (?)
            parentGrid.Children.Add(thisStack);
            Content = parentGrid;
        }


        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
