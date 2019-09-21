using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace BattleshipCP
{
    public class FieldInfoCP : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; } //no need for column or row because of vector.  this takes care of that.
        private string _Letter = "";
        public string Letter
        {
            get
            {
                return _Letter;
            }

            set
            {
                if (SetProperty(ref _Letter, value) == true)
                {
                }
            }
        }
        private EnumWhatHit _Hit;
        public EnumWhatHit Hit
        {
            get
            {
                return _Hit;
            }
            set
            {
                if (SetProperty(ref _Hit, value) == true)
                {
                }
            }
        }
        private int _ShipNumber;
        public int ShipNumber
        {
            get
            {
                return _ShipNumber;
            }
            set
            {
                if (SetProperty(ref _ShipNumber, value) == true)
                {
                }
            }
        }
        private string _FillColor = cs.Blue;
        public string FillColor
        {
            get { return _FillColor; }
            set
            {
                if (SetProperty(ref _FillColor, value))
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