using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using CommonBasicStandardLibraries.Exceptions;
using FluxxCP.Containers;
using FluxxCP.UICP;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FluxxWPF
{
    public class ShowCardUI : BaseFrameWPF, IChangeCard
    {

        private readonly CardGraphicsWPF _graphics;
        private readonly DetailCardObservable _detail;
        private readonly TextBlock _label;

        public ShowCardUI(FluxxVMData model, ActionContainer actionContainer, KeeperContainer keeperContainer, EnumShowCategory category)
        {
            DataContext = actionContainer;
            switch (category)
            {
                case EnumShowCategory.MainScreen:
                    Text = "Card Information";
                    _detail = model.CardDetail;
                    break;
                case EnumShowCategory.CurrentAction:
                    Text = "Current Card Information";
                    //var thisAction1 = (ActionViewModel)thisMod.Action1!;
                    _detail = actionContainer.CurrentDetail;
                    break;
                case EnumShowCategory.MainAction:
                    SetBinding(TextProperty, new Binding(nameof(ActionContainer.ActionFrameText)));
                    //var thisAction2 = (ActionViewModel)thisMod.Action1!;
                    _detail = actionContainer.ActionDetail;
                    break;
                case EnumShowCategory.KeeperScreen:
                    Text = "Current Card Information";
                    //var thisKeeper = (KeeperViewModel)thisMod.KeeperControl1!;
                    _detail = keeperContainer.CardDetail;
                    break;
                default:
                    throw new BasicBlankException("Category Not Found");
            }
            _detail!.Card = this;
            Grid tempGrid = new Grid();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(tempGrid, thisRect); //i think.
            AddAutoColumns(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            _graphics = new CardGraphicsWPF();
            _label = GetDefaultLabel();
            _label.Text = _detail.CurrentCard.Description;
            _label.TextWrapping = TextWrapping.Wrap;
            _graphics.SendSize("", _detail.CurrentCard);
            AddControlToGrid(tempGrid, _graphics, 0, 0);
            _label.Margin = new Thickness(10, 3, 5, 3);
            AddControlToGrid(tempGrid, _label, 0, 1);
            Grid grid = new Grid();
            grid.Children.Add(ThisDraw);
            grid.Children.Add(tempGrid);
            Content = grid;
        }


        void IChangeCard.ShowChangedCard()
        {
            _graphics!.DataContext = _detail!.CurrentCard;
            _label!.Text = _detail.CurrentCard.Description;
        }
    }
}
