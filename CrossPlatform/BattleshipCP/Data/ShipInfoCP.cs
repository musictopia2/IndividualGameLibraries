using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System.Collections.Generic;
namespace BattleshipCP.Data
{
    public class ShipInfoCP : ObservableObject
    {
        private string _shipName = "";
        public string ShipName
        {
            get
            {
                return _shipName;
            }

            set
            {
                if (SetProperty(ref _shipName, value) == true)
                {
                }
            }
        }
        private EnumShipList _shipCategory;
        public EnumShipList ShipCategory
        {
            get
            {
                return _shipCategory;
            }

            set
            {
                if (SetProperty(ref _shipCategory, value) == true)
                {
                }
            }
        }
        private bool _visible;
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (SetProperty(ref _visible, value) == true)
                {
                }
            }
        }
        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                if (SetProperty(ref _isEnabled, value) == true)
                {
                }
            }
        }
        private Dictionary<int, PieceInfoCP>? _pieceList;
        public Dictionary<int, PieceInfoCP>? PieceList
        {
            get
            {
                return _pieceList;
            }
            set
            {
                if (SetProperty(ref _pieceList, value) == true)
                {
                }
            }
        }
    }
}
