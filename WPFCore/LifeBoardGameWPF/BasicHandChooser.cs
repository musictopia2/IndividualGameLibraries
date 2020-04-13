using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Data;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions; //this usually will be used too.
using System.Windows;
namespace LifeBoardGameWPF
{
    public abstract class BasicHandChooser : UserControl, IUIView
    {
        readonly Button _button;
        public BasicHandChooser(LifeBoardGameVMData model)
        {
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Button button = GetGamingButton("", nameof(BasicSubmitViewModel.SubmitAsync));
            _button = button;
            button.FontSize = 100; //make 100 instead of 200 now.
            button.VerticalAlignment = VerticalAlignment.Top;
            LifeHandWPF hand = new LifeHandWPF();
            hand.HandType = HandObservable<LifeBaseCard>.EnumHandList.Vertical;
            
            stack.Children.Add(hand);
            stack.Children.Add(button);
            hand.LoadList(model.HandList, "");
            Content = stack;
        }

        Task IUIView.TryActivateAsync()
        {
            BasicSubmitViewModel model = (BasicSubmitViewModel)DataContext;
            _button.Content = model.Text;
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}
