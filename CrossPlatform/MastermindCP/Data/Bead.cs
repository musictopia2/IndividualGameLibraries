using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq; //sometimes i do use linq.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace MastermindCP.Data
{
    public class Bead : ObservableObject
    {
        private EnumColorPossibilities _colorChosen = EnumColorPossibilities.None;
        public EnumColorPossibilities ColorChosen
        {
            get
            {
                return _colorChosen;
            }

            set
            {
                if (SetProperty(ref _colorChosen, value) == true)
                    // code to run
                    OnPropertyChanged(nameof(UIColor));
            }
        }

        public string UIColor
        {
            get
            {
                return ColorChosen.ToColor();
            }
        }// that way i don't need converter (since i have to do twice instead of just once).

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

        public bool DidCheck { get; set; }

        public Guess? CurrentGuess; // hopefully no binding.  however, this is needed so when i get the bead, i have the guess involved for this.

        public Bead()
        {
        }

        public Bead(EnumColorPossibilities color)
        {
            ColorChosen = color;
        }
    }
}
