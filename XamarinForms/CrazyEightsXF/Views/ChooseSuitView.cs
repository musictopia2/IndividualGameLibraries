using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIXFLibrary.GameGraphics.GamePieces;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using CrazyEightsCP.Data;

namespace CrazyEightsXF.Views
{
    public class ChooseSuitView : CustomControlBase
    {
        public ChooseSuitView(CrazyEightsVMData model)
        {
            EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList> picker = new EnumPickerXF<DeckPieceCP, DeckPieceXF, EnumSuitList>();
            picker.GraphicsHeight = 200;
            picker.GraphicsWidth = 200;
            picker.LoadLists(model.SuitChooser);
            Content = picker;
        }
    }
}
