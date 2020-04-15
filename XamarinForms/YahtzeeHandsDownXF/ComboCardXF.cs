using BasicGamingUIXFLibrary.GameGraphics.Base;
using YahtzeeHandsDownCP.Cards;

namespace YahtzeeHandsDownXF
{
    public class ComboCardXF : BaseDeckGraphicsXF<ComboCardInfo, ComboCP>
    {
        public ComboCardInfo? CurrentCombo //taking a risk.
        {
            get
            {
                return MainObject!.ThisCombo;
            }
            set
            {
                MainObject!.ThisCombo = value;
            }
        }
        protected override void DoCardBindings()
        {
            //hopefully the risk pays off this time.  if not rethink this one.
        }

        protected override void PopulateInitObject()
        {
            MainObject!.Init();
        }
    }
    
}