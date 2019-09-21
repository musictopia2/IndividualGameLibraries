using BasicGameFramework.GameBoardCollections;
using BasicGameFramework.MiscProcesses;
using CommonBasicStandardLibraries.MVVMHelpers;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SolitaireBoardGameCP
{
    public class GameSpace : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }

        private bool _HasImage;

        public bool HasImage
        {
            get { return _HasImage; }
            set
            {
                if (SetProperty(ref _HasImage, value))
                {
                    //can decide what to do when property changes
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
            Color = cs.Blue; //was blue.  trying green for experimenting.
        }

        public bool IsFilled()
        {
            return false; //until we figure out what we do about this one.
        }
    }
}