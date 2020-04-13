using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using LifeBoardGameCP.Data;
using System.Collections.Generic;
namespace LifeBoardGameCP.Graphics
{
    public class TempInfo : ObservableObject
    {
        public EnumViewCategory CurrentView { get; set; }
        public int HeightWidth { get; set; }
        private int _currentNumber;
        public int CurrentNumber
        {
            get
            {
                return _currentNumber;
            }
            set
            {
                if (SetProperty(ref _currentNumber, value) == true)
                {
                }
            }
        }
        public List<PositionInfo> PositionList { get; set; } = new List<PositionInfo>(); //decided to not risk doing the custom one for here.  could put back if needed (?)
    }
}