using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MillebournesCP;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BaseGPXWindowsAndControlsCore.BaseWindows.SharedWindowFunctions;
namespace MillebournesWPF
{
    public class SafetiesWPF : UserControl
    {
        private enum EnumModel
        {
            Safety = 1,
            Main
        }
        private CustomBasicCollection<SafetyInfo>? _safetyList;
        private TeamCP? _safetyMod;
        private StackPanel? _safetyStack;
        public void Init(TeamCP mod, MillebournesMainGameClass mainGame)
        {
            _safetyMod = mod;
            _safetyList = _safetyMod.SafetyList;
            _safetyList.CollectionChanged += SafetyList_CollectionChanged;
            StackPanel thisStack = new StackPanel();
            thisStack.Margin = new Thickness(3, 3, 3, 3);
            _safetyStack = new StackPanel();
            StackPanel tempStack = new StackPanel();
            tempStack.Margin = new Thickness(0, 10, 0, 0);
            tempStack.Orientation = Orientation.Horizontal;
            var thisBut = GetButton("Safety", nameof(TeamCP.SafetyCommand), nameof(TeamCP.SafetyEnabled), EnumModel.Safety, mainGame);
            tempStack.Children.Add(thisBut);
            thisStack.Children.Add(_safetyStack);
            thisStack.Children.Add(tempStack);
            _safetyMod.SafetyList.ForEach(thisSafe =>
            {
                AddLabel(thisSafe);
            });
            Content = thisStack;
        }
        private void AddLabel(object thisSafe)
        {
            var thisLabel = GetDefaultLabel();
            thisLabel.SetBinding(TextBlock.TextProperty, new Binding(nameof(SafetyInfo.SafetyName)));
            thisLabel.DataContext = thisSafe;
            Binding binds = new Binding(nameof(SafetyInfo.WasCoupe));
            binds.Converter = new SafetyConverter();
            thisLabel.FontSize = 25;
            thisLabel.SetBinding(TextBlock.FontWeightProperty, binds);
            _safetyStack!.Children.Add(thisLabel);
        }
        private Button GetButton(string text, string command, string visible, EnumModel model, MillebournesMainGameClass mainGame)
        {
            var output = GetGamingButton(text, command);
            if (mainGame.SingleInfo!.Team == _safetyMod!.TeamNumber)
            {
                Binding binding = GetVisibleBinding(visible);
                output.SetBinding(VisibilityProperty, binding);
            }
            else
                output.Visibility = Visibility.Collapsed;
            output.FontSize = 30; //i think
            if (model == EnumModel.Main)
                output.DataContext = mainGame.ThisMod;
            else
                output.DataContext = _safetyMod;
            return output;
        }
        private void SafetyList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _safetyStack!.Children.Clear();
                return;
            }
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count != 1)
                    throw new BasicBlankException("Should only add one item at a time here");
                AddLabel(e.NewItems[0]!);
            }
        }
    }
}