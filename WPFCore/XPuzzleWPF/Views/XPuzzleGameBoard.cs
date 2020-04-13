using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGamingUIWPFLibrary.BasicControls.GameBoards;
using XPuzzleCP.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using XPuzzleCP.ViewModels;
//i think this is the most common things i like to do
namespace XPuzzleWPF.Views
{
    public class XPuzzleGameBoard : BasicGameBoard<XPuzzleSpaceInfo>
    {
        protected override void StartInit()
        {
            Padding = new Thickness(5, 5, 5, 5);
        }
        protected override Control GetControl(XPuzzleSpaceInfo thisItem, int index)
        {
            Button thisBut = new Button();
            thisBut.Height = 250;
            thisBut.Width = 250;
            thisBut.DataContext = thisItem;
            thisBut.SetBinding(ContentProperty, new Binding(nameof(XPuzzleSpaceInfo.Text)));
            thisBut.SetBinding(BackgroundProperty, GetColorBinding(nameof(XPuzzleSpaceInfo.Color)));
            thisBut.Name = nameof(XPuzzleMainViewModel.MakeMoveAsync);
            thisBut.BorderThickness = new Thickness(2, 2, 2, 2);
            thisBut.BorderBrush = Brushes.White;
            thisBut.FontSize = 200;
            thisBut.Foreground = Brushes.White;
            thisBut.CommandParameter = thisItem; //hopefully this simple (?)  especially since going to new game will reload everything anyways.
            //thisBut.SetBinding(ButtonBase.CommandParameterProperty, new Binding(".")); // hopefully this would be fine too (?)
            return thisBut;
        }
    }
}
