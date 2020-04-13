using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using MonopolyCardGameCP.Data;
using MonopolyCardGameCP.Logic;
using MonopolyCardGameCP.ViewModels;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using static BasicControlsAndWindowsCore.Helpers.GridHelper; //usually needs this as well for grid helpers.
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace MonopolyCardGameWPF
{
    public class ShowCardUI : BaseFrameWPF, IChangeCard
    {
        private CardGraphicsWPF? _thisG;
        private DetailCardViewModel? _thisDetail;
        private TextBlock? _thisLabel;
        public void LoadControls(MonopolyCardGameVMData model)
        {
            _thisDetail = model.AdditionalInfo1;
            Text = "Card Information";
            _thisDetail!.Change = this;
            Grid tempGrid = new Grid();
            SKRect thisRect = ThisFrame.GetControlArea();
            SetUpMarginsOnParentControl(tempGrid, thisRect); //i think.
            AddAutoColumns(tempGrid, 1);
            AddLeftOverColumn(tempGrid, 1);
            _thisG = new CardGraphicsWPF();
            _thisLabel = GetDefaultLabel();
            _thisLabel.Text = _thisDetail.CurrentCard.Description;
            _thisLabel.TextWrapping = TextWrapping.Wrap;
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
            _thisG!.DataContext = _thisDetail!.CurrentCard;
            _thisLabel!.Text = _thisDetail.CurrentCard.Description;
        }
    }
}
