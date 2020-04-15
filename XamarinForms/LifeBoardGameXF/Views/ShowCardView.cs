using BasicXFControlsAndPages.MVVMFramework.Controls;
using LifeBoardGameCP.Data;
namespace LifeBoardGameXF.Views
{
    public class ShowCardView : CustomControlBase
    {
        public ShowCardView(LifeBoardGameVMData model)
        {
            LifePileXF card = new LifePileXF();
            card.Init(model.SinglePile, "");
            Content = card;
        }

    }
}