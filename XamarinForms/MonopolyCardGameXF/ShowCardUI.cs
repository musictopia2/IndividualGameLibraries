using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using MonopolyCardGameCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MonopolyCardGameXF
{
    public class ShowCardUI : BaseFrameXF, IChangeCard
    {
        private CardGraphicsXF? _thisG;
        private DetailCardViewModel? _thisDetail;
        private Label? _thisLabel;
        public void LoadControls()
        {
            MonopolyCardGameViewModel thisMod = Resolve<MonopolyCardGameViewModel>();
            _thisDetail = thisMod.AdditionalInfo1;
            Text = "Card Information";
            _thisDetail!.ThisChange = this;
            Grid tempGrid = new Grid();
            HorizontalOptions = LayoutOptions.Start;
            SetUpMarginsOnParentControl(tempGrid); //i think.
            AddAutoColumns(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            _thisG = new CardGraphicsXF();
            _thisLabel = GetDefaultLabel();
            _thisLabel.Text = _thisDetail.CurrentCard.Description; //hopefully it wraps automatically since no option for it.
            _thisG.SendSize("", _thisDetail.CurrentCard);
            AddControlToGrid(tempGrid, _thisG, 0, 0);
            _thisLabel.Margin = new Thickness(10, 3, 5, 3);
            AddControlToGrid(tempGrid, _thisLabel, 0, 1);
            Grid thisGrid = new Grid();
            thisGrid.Children.Add(ThisDraw);
            thisGrid.Children.Add(tempGrid);
            Content = thisGrid;
        }
        public void ShowChangedCard()
        {
            _thisG!.BindingContext = _thisDetail!.CurrentCard;
            _thisLabel!.Text = _thisDetail.CurrentCard.Description;
        }
    }
}