using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.GameGraphics.GamePieces;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CrazyEightsCP.Data;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CrazyEightsWPF.Views
{
    public class ChooseSuitView : UserControl, IUIView
    {

        public ChooseSuitView(CrazyEightsVMData model)
        {
            EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList> picker = new EnumPickerWPF<DeckPieceCP, DeckPieceWPF, EnumSuitList>();
            picker.GraphicsHeight = 200;
            picker.GraphicsWidth = 200;
            picker.LoadLists(model.SuitChooser);
            Content = picker;
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
