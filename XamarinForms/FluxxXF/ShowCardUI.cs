using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP;
using Xamarin.Forms;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
using static BasicXFControlsAndPages.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace FluxxXF
{
    public enum EnumShowCategory
    {
        MainScreen = 1, CurrentAction, MainAction, KeeperScreen
    }
    public class ShowCardUI : BaseFrameXF, IChangeCard
    {
        private CardGraphicsXF? _thisG;
        private DetailCardViewModel? _thisDetail;
        private Label? _thisLabel;
        public void LoadControls(EnumShowCategory category)
        {
            FluxxViewModel thisMod = Resolve<FluxxViewModel>();
            BindingContext = (ActionViewModel)thisMod.Action1!; //try this.
            switch (category)
            {
                case EnumShowCategory.MainScreen:
                    Text = "Card Information";
                    _thisDetail = thisMod.ThisDetail;
                    break;
                case EnumShowCategory.CurrentAction:
                    Text = "Current Card Information";
                    var thisAction1 = (ActionViewModel)thisMod.Action1!;
                    _thisDetail = thisAction1.CurrentDetail;
                    break;
                case EnumShowCategory.MainAction:
                    SetBinding(TextProperty, new Binding(nameof(ActionViewModel.ActionFrameText)));
                    var thisAction2 = (ActionViewModel)thisMod.Action1!;
                    _thisDetail = thisAction2.ActionDetail;
                    break;
                case EnumShowCategory.KeeperScreen:
                    Text = "Current Card Information";
                    var thisKeeper = (KeeperViewModel)thisMod.KeeperControl1!;
                    _thisDetail = thisKeeper.ThisDetail;
                    break;
                default:
                    throw new BasicBlankException("Category Not Found");
            }
            _thisDetail!.ThisChange = this;
            Grid tempGrid = new Grid();
            SetUpMarginsOnParentControl(tempGrid); //i think.
            AddAutoColumns(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            _thisG = new CardGraphicsXF();
            _thisLabel = GetDefaultLabel();
            _thisLabel.Text = _thisDetail.CurrentCard.Description;
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