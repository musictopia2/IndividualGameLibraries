using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;

namespace BattleshipCP.Data
{
    public class FieldInfoCP : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; } //no need for column or row because of vector.  this takes care of that.
        private string _letter = "";
        public string Letter
        {
            get
            {
                return _letter;
            }

            set
            {
                if (SetProperty(ref _letter, value) == true)
                {
                }
            }
        }
        private EnumWhatHit _hit;
        public EnumWhatHit Hit
        {
            get
            {
                return _hit;
            }
            set
            {
                if (SetProperty(ref _hit, value) == true)
                {
                }
            }
        }
        private int _shipNumber;
        public int ShipNumber
        {
            get
            {
                return _shipNumber;
            }
            set
            {
                if (SetProperty(ref _shipNumber, value) == true)
                {
                }
            }
        }
        private string _fillColor = cs.Blue;
        public string FillColor
        {
            get { return _fillColor; }
            set
            {
                if (SetProperty(ref _fillColor, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public void ClearSpace()
        {
            FillColor = cs.Blue;
            Hit = EnumWhatHit.None;
            ShipNumber = 0; //i think this too.
        }
        public bool IsFilled()
        {
            return Hit != EnumWhatHit.None; //if you missed, its still filled.
        }
    }
}
