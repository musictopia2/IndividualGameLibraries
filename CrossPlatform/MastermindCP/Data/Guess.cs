using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq; //sometimes i do use linq.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;

namespace MastermindCP.Data
{
    public class Guess : ObservableObject
    {
        public CustomBasicList<Bead> YourBeads = new CustomBasicList<Bead>(); //hopefully this works.  if not rethink.
        //take a few risks.

        public void GetNewBeads()
        {
            // all 4 will be transparent.
            int x;
            CustomBasicList<Bead> tempList = new CustomBasicList<Bead>();
            for (x = 1; x <= 4; x++)
            {
                Bead thisBead = new Bead();
                thisBead.ColorChosen = EnumColorPossibilities.None;
                thisBead.CurrentGuess = this; // i think
                tempList.Add(thisBead);
            }
            YourBeads.ReplaceRange(tempList);
        }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }

            set
            {
                if (SetProperty(ref _isCompleted, value) == true)
                {
                }
            }
        }





        private int _howManyBlacks;
        public int HowManyBlacks
        {
            get
            {
                return _howManyBlacks;
            }

            set
            {
                if (SetProperty(ref _howManyBlacks, value) == true)
                {
                }
            }
        }

        private int _howManyAquas;
        public int HowManyAquas
        {
            get
            {
                return _howManyAquas;
            }

            set
            {
                if (SetProperty(ref _howManyAquas, value) == true)
                {
                }
            }
        }

    }
}
