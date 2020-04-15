using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGamingUIXFLibrary.BasicControls.GameFrames;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using PaydayCP.Cards;
using PaydayCP.Data;
using PaydayCP.Graphics;
using Xamarin.Forms;

namespace PaydayXF.Views
{
    public class MailListView : CustomControlBase
    {
        public MailListView(PaydayVMData model)
        {
            BaseHandXF<MailCard, CardGraphicsCP, MailCardXF> hand = new BaseHandXF<MailCard, CardGraphicsCP, MailCardXF>();
            hand.HandType = HandObservable<MailCard>.EnumHandList.Vertical;
            hand.HeightRequest = 500;
            hand.HorizontalOptions = LayoutOptions.Start;
            hand.LoadList(model.CurrentMailList, "");
            Content = hand;
        }

    }
}