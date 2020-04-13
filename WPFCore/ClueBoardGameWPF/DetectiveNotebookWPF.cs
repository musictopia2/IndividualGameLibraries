using BasicGamingUIWPFLibrary.Helpers;
using ClueBoardGameCP.Data;
using ClueBoardGameCP.ViewModels;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ClueBoardGameWPF
{
    public class DetectiveNotebookWPF : UserControl
    {
        public void LoadControls(ClueBoardGameGameContainer gameContainer)
        {
            StackPanel firstStack = new StackPanel();
            firstStack.Orientation = Orientation.Horizontal;
            StackPanel firstRow = new StackPanel();
            StackPanel secondRow = new StackPanel();
            StackPanel thirdRow = new StackPanel();
            firstStack.Children.Add(firstRow);
            firstStack.Children.Add(secondRow);
            firstStack.Children.Add(thirdRow);
            var thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsRoom).ToCustomBasicList();
            Button thisBut;
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                firstRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsCharacter).ToCustomBasicList();
            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                secondRow.Children.Add(thisBut);
            });
            thisList = gameContainer.DetectiveList.Values.Where(items => items.Category == EnumCardType.IsWeapon).ToCustomBasicList();
            if (thisList.Count != 6)
                throw new BasicBlankException("There has to be 6 weapons when loading controls");

            thisList.ForEach(thisItem =>
            {
                thisBut = GetButton(thisItem);
                thirdRow.Children.Add(thisBut);
            });
            Content = firstStack;
        }
        private Button GetButton(DetectiveInfo thisDet)
        {
            Button output = new Button();
            output.Content = thisDet.Name;
            output.FontSize = 22;
            output.CommandParameter = thisDet;
            Binding thisBind = new Binding(nameof(DetectiveInfo.IsChecked));
            DetectiveConverter thisC = new DetectiveConverter();
            thisBind.Converter = thisC;
            output.DataContext = thisDet;
            output.SetBinding(BackgroundProperty, thisBind);
            output.Name = nameof(ClueBoardGameMainViewModel.FillInClue);
            GamePackageViewModelBinder.ManuelElements.Add(output); //try this way.
            output.CommandParameter = thisDet;
            return output;
        }
    }
}