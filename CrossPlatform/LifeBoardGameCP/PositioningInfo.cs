using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using System.Collections.Generic;
namespace LifeBoardGameCP
{
    public class PositionInfo
    {
        public SKPoint PointView { get; set; }
        public int SpaceNumber { get; set; }
        public SKPoint SpacePoint { get; set; }
    }
    public class TempInfo : ObservableObject
    {
        public EnumViewCategory CurrentView { get; set; }
        public int HeightWidth { get; set; }
        private int _CurrentNumber;
        public int CurrentNumber
        {
            get
            {
                return _CurrentNumber;
            }
            set
            {
                if (SetProperty(ref _CurrentNumber, value) == true)
                {
                }
            }
        }
        public List<PositionInfo> PositionList { get; set; } = new List<PositionInfo>(); //decided to not risk doing the custom one for here.  could put back if needed (?)
    }
}