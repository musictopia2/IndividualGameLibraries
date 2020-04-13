using BasicGameFrameworkLibrary.GameBoardCollections;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using cs = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.SColorString;
namespace SolitaireBoardGameCP.Data
{
    public class GameSpace : ObservableObject, IBasicSpace
    {
        public Vector Vector { get; set; }

        private bool _hasImage;

        public bool HasImage
        {
            get { return _hasImage; }
            set
            {
                if (SetProperty(ref _hasImage, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private string _color = cs.Transparent; //now we use string.
        public string Color
        {
            get
            {
                return _color;
            }

            set
            {
                if (SetProperty(ref _color, value) == true)
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
