using BasicGamingUIXFLibrary.BasicControls.SingleCardFrames;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using Xamarin.Forms;

namespace PaydayXF.Views
{
    public class MailPileView : CustomControlBase
    {
        public MailPileView(PaydayVMData data)
        {
            BasePileXF<MailCard, CardGraphicsCP, MailCardXF> pile = new BasePileXF<MailCard, CardGraphicsCP, MailCardXF>();
            pile.Init(data.MailPile, "");
            Content = pile;
        }
    }
}