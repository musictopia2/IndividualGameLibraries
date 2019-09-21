using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace XPuzzleCP
{
    public class XPuzzleSpaceInfo : ObservableObject, IBasicSpace
    {

        public Vector Vector { get; set; }

        private string _Text = "";
        public string Text
        {
            get
            {
                return _Text;
            }

            set
            {
                if (SetProperty(ref _Text, value) == true)
                {
                }
            }
        }

        private string _Color = cs.Transparent; //now we use string.
        public string Color
        {
            get
            {
                return _Color;
            }

            set
            {
                if (SetProperty(ref _Color, value) == true)
                {
                }
            }
        }

        public void ClearSpace()
        {
            Color = cs.Transparent;
            Text = "";
        }

        public bool IsFilled()
        {
            return !string.IsNullOrWhiteSpace(Text);
        }
    }
}
