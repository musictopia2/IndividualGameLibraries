using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using BasicGamingUIWPFLibrary.Helpers;
using SequenceDiceCP.Data;
using SequenceDiceCP.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;

namespace SequenceDiceWPF
{
    public class GameBoardWPF : BasicGameBoard<SpaceInfoCP>
    {
        protected override Control GetControl(SpaceInfoCP thisItem, int index)
        {
            var thisCon = new MiscGraphicsWPF();
            thisCon.DataContext = thisItem;
            thisCon.SetBinding(MiscGraphicsWPF.WasPreviousProperty, new Binding(nameof(SpaceInfoCP.WasRecent)));
            thisCon.SetBinding(MiscGraphicsWPF.NumberProperty, new Binding(nameof(SpaceInfoCP.Number)));
            thisCon.SetBinding(MiscGraphicsWPF.MainColorProperty, new Binding(nameof(SpaceInfoCP.Color)));
            thisCon.Name = nameof(SequenceDiceMainViewModel.MakeMoveAsync);
            thisCon.Width = 120;
            thisCon.Height = 120; //maybe this was needed too.
            thisCon.Init(); //maybe this too.
            GamePackageViewModelBinder.ManuelElements.Add(thisCon); //may have to be added manually.  i have had that experience before.
            thisCon.CommandParameter = thisItem;
            return thisCon; //hopefully this simple.
        }
    }
}