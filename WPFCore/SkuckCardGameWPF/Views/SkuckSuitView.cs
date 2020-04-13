using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SkuckCardGameCP.Data;
using SkuckCardGameCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace SkuckCardGameWPF.Views
{
    public class SkuckSuitView : BaseFrameWPF, IUIView
    {
        public SkuckSuitView(SkuckCardGameVMData model)
        {
            Text = "Trump Info";
            Grid grid = new Grid();
            var rect = ThisFrame.GetControlArea();
            StackPanel thisStack = new StackPanel();
            SetUpMarginsOnParentControl(thisStack, rect);
            EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList> thisSuit = new EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList>();
            thisStack.Children.Add(thisSuit);
            thisSuit.LoadLists(model.Suit1!);
            Button thisBut = GetGamingButton("Choose Trump Suit", nameof(SkuckSuitViewModel.TrumpAsync));
            thisStack.Children.Add(thisBut);
            grid.Children.Add(ThisDraw);
            grid.Children.Add(thisStack);
            Content = grid;
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