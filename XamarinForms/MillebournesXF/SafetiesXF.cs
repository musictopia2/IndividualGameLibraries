using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using MillebournesCP;
using System.Collections.Specialized;
using Xamarin.Forms;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using static BaseGPXPagesAndControlsXF.BasePageProcesses.Pages.SharedPageFunctions;
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
        public void Init(TeamCP mod, MillebournesMainGameClass mainGame)
        {
            _safetyMod = mod;
            _safetyList = _safetyMod.SafetyList;
            _safetyList.CollectionChanged += SafetyList_CollectionChanged;
            StackLayout thisStack = new StackLayout();
            thisStack.Margin = new Thickness(3, 0, 3, 0);
            _safetyStack = new StackLayout();
            _safetyStack.Spacing = 0;
            StackLayout tempStack = new StackLayout();
            tempStack.Orientation = StackOrientation.Horizontal;
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
        private Button GetButton(string text, string command, string visible, EnumModel model, MillebournesMainGameClass mainGame)
        {
            var output = GetGamingButton(text, command);
            if (mainGame.SingleInfo!.Team == _safetyMod!.TeamNumber)
            {
                Binding binding = new Binding(visible);
                output.SetBinding(IsVisibleProperty, binding);
            }
            else
                output.IsVisible = false;
            if (model == EnumModel.Main)
            {
                if (ScreenUsed == EnumScreen.SmallPhone)
                    output.FontSize = 20;
                else if (ScreenUsed == EnumScreen.SmallTablet)
                    output.FontSize = 30;
                else
                    output.FontSize = 40;
                output.BindingContext = mainGame.ThisMod;
            }
            else
            {
                output.BindingContext = _safetyMod;
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