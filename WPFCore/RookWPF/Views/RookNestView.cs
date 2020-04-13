using BasicGamingUIWPFLibrary.Views;
using RookCP.ViewModels;

namespace RookWPF.Views
{
    public class RookNestView : BasicSubmitView
    {
        protected override string CommandText => nameof(RookNestViewModel.NestAsync);
    }
}