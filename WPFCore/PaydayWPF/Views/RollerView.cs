using BasicGameFrameworkLibrary.Dice;
using BasicGamingUIWPFLibrary.BasicControls.Misc;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using PaydayCP.Data;
using PaydayCP.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;
using static BasicGamingUIWPFLibrary.Helpers.SharedUIFunctions;
namespace PaydayWPF.Views
{
    public class RollerView : UserControl, IUIView
    {
        public RollerView(PaydayVMData model)
        {
            StackPanel stack = new StackPanel();
            

            var thisRoll = GetGamingButton("Roll Dice", nameof(RollerViewModel.RollDiceAsync));
            stack.Children.Add(thisRoll);
            DiceListControlWPF<SimpleDice> dice = new DiceListControlWPF<SimpleDice>();
            dice.LoadDiceViewModel(model.Cup!);
            stack.Children.Add(dice);
            Content = stack;
        }
        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            return Task.CompletedTask;
        }
    }
}