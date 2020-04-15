using BasicGamingUIXFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Containers;
using FluxxCP.UICP;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
namespace FluxxXF
{
    public class ShowCardUI : BaseFrameXF, IChangeCard
    {
        private readonly CardGraphicsXF? _graphics;
        private readonly DetailCardObservable _detail;
        private readonly Label? _label;

        public ShowCardUI(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer, EnumShowCategory category)
        {
            BindingContext = actionContainer;
            switch (category)
            {
                case EnumShowCategory.MainScreen:
                    Text = "Card Information";
                    _detail = model.CardDetail;
                    break;
                case EnumShowCategory.CurrentAction:
                    Text = "Current Card Information";
                    _detail = actionContainer.CurrentDetail;
                    break;
                case EnumShowCategory.MainAction:
                    SetBinding(TextProperty, new Binding(nameof(ActionContainer.ActionFrameText)));
                    _detail = actionContainer.ActionDetail;
                    break;
                case EnumShowCategory.KeeperScreen:
                    Text = "Current Card Information";
                    _detail = keeperContainer.CardDetail;
                    break;
                default:
                    throw new BasicBlankException("Category Not Found");
            }
            _detail!.Card = this;
            Grid tempGrid = new Grid();
            SetUpMarginsOnParentControl(tempGrid); //i think.
            AddAutoColumns(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            _graphics = new CardGraphicsXF();
            _label = GetDefaultLabel();
            _label.Text = _detail.CurrentCard.Description; //no wrap unfortunately.
            _graphics.SendSize("", _detail.CurrentCard);
            AddControlToGrid(tempGrid, _graphics, 0, 0);
            _label.Margin = new Thickness(10, 3, 5, 3);
            AddControlToGrid(tempGrid, _label, 0, 1);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(tempGrid);
            Content = grid;
        }

        public void ShowChangedCard()
        {
            _graphics!.BindingContext = _detail!.CurrentCard;
            _label!.Text = _detail.CurrentCard.Description;
        }
    }
}