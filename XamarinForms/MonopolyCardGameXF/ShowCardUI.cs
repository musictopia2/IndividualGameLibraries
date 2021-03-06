using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using MonopolyCardGameCP.Data;
using MonopolyCardGameCP.Logic;
using MonopolyCardGameCP.ViewModels;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace MonopolyCardGameXF
{
    public class ShowCardUI : BaseFrameXF, IChangeCard
    {
        private CardGraphicsXF? _thisG;
        private DetailCardViewModel? _thisDetail;
        private Label? _thisLabel;
        public void LoadControls(MonopolyCardGameVMData model)
        {
            _thisDetail = model.AdditionalInfo1;
            Text = "Card Information";
            _thisDetail!.Change = this;
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