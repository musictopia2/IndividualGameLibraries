using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace MastermindCP
{
    public enum EnumColorPossibilities
    {
        None = 0, // this means nothing was chosen
        Aqua = 1,
        Black = 2,
        Blue = 3,
        Green = 4,
        Purple = 5,
        Red = 6,
        White = 7,
        Yellow = 8
    }

    public interface IScroll
    {
        void ScrollToGuess(Guess thisGuess); // this will get a hint.  its up to the 2 implementions to figure out how to scroll it.
    }

    public interface IEndSolution
    {
        void ShowSolution();

        void HideSolution();
    }

    public class Bead : ObservableObject
    {
        private EnumColorPossibilities _ColorChosen = EnumColorPossibilities.None;
        public EnumColorPossibilities ColorChosen
        {
            get
            {
                return _ColorChosen;
            }

            set
            {
                if (SetProperty(ref _ColorChosen, value) == true)
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

        public bool DidCheck { get; set; }
        // no binding in this case.

        // from the video https://www.youtube.com/watch?v=qVpjlewCZNU 
        // if the item is correct, this will return true.
        // that will mean it can' check this one ever again.

        // his video showed first check for completely correct.

        // maybe the next step is whatever is left, check to see if it exist in the solution.  if it exists, then other variable adds one to it.
        // the link means if i need more hints again, i have it.

        // another hint:
        // whatever it finds that is correct and correct position, it has to mark DidCheck from both the guess and the solution.

        // also, when its getting ready for next one, has to reset that part.

        public Guess? CurrentGuess; // hopefully no binding.  however, this is needed so when i get the bead, i have the guess involved for this.

        public Bead()
        {
        }

        public Bead(EnumColorPossibilities color)
        {
            ColorChosen = color;
        }
    }



    public class Guess : ObservableObject
    {
        public CustomBasicCollection<Bead> YourBeads = new CustomBasicCollection<Bead>();

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

        private bool _IsCompleted;
        public bool IsCompleted
        {
            get
            {
                return _IsCompleted;
            }

            set
            {
                if (SetProperty(ref _IsCompleted, value) == true)
                {
                }
            }
        }





        private int _HowManyBlacks;
        public int HowManyBlacks
        {
            get
            {
                return _HowManyBlacks;
            }

            set
            {
                if (SetProperty(ref _HowManyBlacks, value) == true)
                {
                }
            }
        }

        private int _HowManyAquas;
        public int HowManyAquas
        {
            get
            {
                return _HowManyAquas;
            }

            set
            {
                if (SetProperty(ref _HowManyAquas, value) == true)
                {
                }
            }
        }
    }
}
