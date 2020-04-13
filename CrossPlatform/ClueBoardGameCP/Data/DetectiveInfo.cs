using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace ClueBoardGameCP.Data
{
    public class DetectiveInfo : ObservableObject
    {
        private string _name = "";
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (SetProperty(ref _name, value) == true)
                {
                }
            }
        }
        private EnumCardType _category;
        public EnumCardType Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (SetProperty(ref _category, value) == true)
                {
                }
            }
        }
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (SetProperty(ref _isChecked, value) == true)
                {
                }
            }
        }
    }
}
