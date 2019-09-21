using CommonBasicStandardLibraries.MVVMHelpers;
using System.Collections.Generic;
namespace BattleshipCP
{
    public class ShipInfoCP : ObservableObject
    {
        private string _ShipName = "";
        public string ShipName
        {
            get
            {
                return _ShipName;
            }

            set
            {
                if (SetProperty(ref _ShipName, value) == true)
                {
                }
            }
        }
        private EnumShipList _ShipCategory;
        public EnumShipList ShipCategory
        {
            get
            {
                return _ShipCategory;
            }

            set
            {
                if (SetProperty(ref _ShipCategory, value) == true)
                {
                }
            }
        }
        private bool _Visible;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (SetProperty(ref _Visible, value) == true)
                {
                }
            }
        }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }

            set
            {
                if (SetProperty(ref _IsEnabled, value) == true)
                {
                }
            }
        }
        private Dictionary<int, PieceInfoCP>? _PieceList;
        public Dictionary<int, PieceInfoCP>? PieceList
        {
            get
            {
                return _PieceList;
            }
            set
            {
                if (SetProperty(ref _PieceList, value) == true)
                {
                }
            }
        }
    }
}