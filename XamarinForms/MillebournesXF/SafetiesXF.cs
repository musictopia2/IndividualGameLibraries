using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MillebournesCP.Data;
using MillebournesCP.Logic;
using System.Collections.Specialized;
using Xamarin.Forms;
using static BasicGameFrameworkLibrary.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
namespace MillebournesXF
{
    public class SafetiesXF : ContentView
    {
        private enum EnumModel
        {
            Safety = 1,
            Main
        }
        private CustomBasicCollection<SafetyInfo>? _safetyList;
        private TeamCP? _safetyMod;
        private StackLayout? _safetyStack;
        private CommandContainer? _commandContainer;
        public void Init(TeamCP mod, MillebournesMainGameClass mainGame, CommandContainer commandContainer)
        {
            _safetyMod = mod;
            _commandContainer = commandContainer;
            _safetyList = _safetyMod.SafetyList;
            _safetyList.CollectionChanged += SafetyList_CollectionChanged;
            StackLayout thisStack = new StackLayout();
            thisStack.Margin = new Thickness(3, 0, 3, 0);
            _safetyStack = new StackLayout();
            _safetyStack.Spacing = 0;
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
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
            thisLabel.SetBinding(Label.TextProperty, new Binding(nameof(SafetyInfo.SafetyName)));
            thisLabel.BindingContext = thisSafe;
            Binding binds = new Binding(nameof(SafetyInfo.WasCoupe));
            binds.Converter = new SafetyConverter();
            if (ScreenUsed == EnumScreen.SmallPhone)
                thisLabel.FontSize = 10;
            else if (ScreenUsed == EnumScreen.SmallTablet)
                thisLabel.FontSize = 15;
            else
                thisLabel.FontSize = 20;
            thisLabel.SetBinding(Label.FontAttributesProperty, binds);
            _safetyStack!.Children.Add(thisLabel);
        }
        private Button GetButton(string text, string commandName, string visible, MillebournesMainGameClass mainGame)
        {
            var output = GetGamingButton(text, "");
            if (mainGame.SingleInfo!.Team == _safetyMod!.TeamNumber)
            {
                Binding binding = new Binding(visible);
                output.SetBinding(IsVisibleProperty, binding);
            }
            else
                output.IsVisible = false;
            if (mainGame.SingleInfo!.Team == _safetyMod!.TeamNumber)
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    output.FontSize = 20;
                else if (ScreenUsed == EnumScreen.SmallTablet)
                    output.FontSize = 30;
                else
                    output.FontSize = 40;
            }
            else
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                {
                    output.FontSize = 10;
                    output.HeightRequest = 38;
                }
                else if (ScreenUsed == EnumScreen.SmallTablet)
                    output.FontSize = 15;
                else
                    output.FontSize = 20;
            }
            output.BindingContext = _safetyMod; //hopefully its okay no matter what (?)
            output.Command = _safetyMod.GetPlainCommand(_commandContainer!, commandName);
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