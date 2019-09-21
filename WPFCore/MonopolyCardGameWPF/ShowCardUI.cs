using BaseGPXWindowsAndControlsCore.BasicControls.SimpleControls;
using MonopolyCardGameCP;
using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
using static BasicControlsAndWindowsCore.Helpers.GridHelper;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace MonopolyCardGameWPF
{
    public class ShowCardUI : BaseFrameWPF, IChangeCard
    {
        private CardGraphicsWPF? _thisG;
        private DetailCardViewModel? _thisDetail;
        private TextBlock? _thisLabel;
        public void LoadControls()
        {
            MonopolyCardGameViewModel thisMod = Resolve<MonopolyCardGameViewModel>();
            _thisDetail = thisMod.AdditionalInfo1;
            Text = "Card Information";
            _thisDetail!.ThisChange = this;
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