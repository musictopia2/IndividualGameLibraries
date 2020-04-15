using BasicGamingUIXFLibrary.BasicControls.ChoicePickers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using FroggiesCP.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static BasicGamingUIXFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.

namespace FroggiesXF.Views
{
    public class FroggiesOpeningView : ContentView, IUIView
    {

        private readonly NumberChooserXF _picker;



        public FroggiesOpeningView()
        {

            //TestControl test = new TestControl();
            //Content = test;
            //FroggiesShellViewModel.EmulateCloseOpen =  () => IsVisible = false;
            _picker = new NumberChooserXF();
            _picker.Columns = 13; //we can increase (?)
            StackLayout stack = new StackLayout();
            Label label = GetDefaultLabel();
            //label.FontSize = 50;
            label.Text = "Choose How Many Frogs:";
            label.Margin = new Thickness(0, 10, 0, 10);
            stack.Children.Add(label);
            _picker.Margin = new Thickness(5);
            stack.Children.Add(_picker);
            Content = stack;
        }


        object IUIView.DataContext
        {
            get => BindingContext;
            set => BindingContext = value;
        }

        Task IUIView.TryActivateAsync()
        {
            FroggiesOpeningViewModel model = (FroggiesOpeningViewModel)BindingContext;
            _picker.LoadLists(model.LevelPicker);
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            InputTransparent = true;
            IsVisible = false;
            IsEnabled = false;
            
            return Task.CompletedTask;
        }
    }
}
