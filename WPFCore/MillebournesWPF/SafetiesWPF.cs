using BasicGameFrameworkLibrary.CommandClasses;
using BasicGamingUIWPFLibrary.Helpers;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MillebournesCP.Data;
using MillebournesCP.Logic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using BasicGameFrameworkLibrary.Extensions;
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
        private CommandContainer? _commandContainer;
        public void Init(TeamCP mod, MillebournesMainGameClass mainGame, CommandContainer commandContainer)
        {
            _safetyMod = mod;
            _commandContainer = commandContainer;
            _safetyList = _safetyMod.SafetyList;
            _safetyList.CollectionChanged += SafetyList_CollectionChanged;
            StackPanel thisStack = new StackPanel();
            thisStack.Margin = new Thickness(3, 3, 3, 3);
            _safetyStack = new StackPanel();
            StackPanel tempStack = new StackPanel();
            tempStack.Margin = new Thickness(0, 10, 0, 0);
            tempStack.Orientation = Orientation.Horizontal;
            var thisBut = GetButton("Safety", nameof(TeamCP.SafetyClickAsync), nameof(TeamCP.SafetyEnabled), mainGame);
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
        private Button GetButton(string text, string commandName, string visible, MillebournesMainGameClass mainGame)
        {
            var output = GetGamingButton(text, "");
            if (mainGame.SingleInfo!.Team == _safetyMod!.TeamNumber)
            {
                Binding binding = GetVisibleBinding(visible);
                output.SetBinding(VisibilityProperty, binding); //may have to be manually done if it does not work.
            }
            else
                output.Visibility = Visibility.Collapsed;
            output.FontSize = 30; //i think

            //PlainCommand command = _safetyMod.

            output.Command = _safetyMod.GetPlainCommand(_commandContainer!, commandName);

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
