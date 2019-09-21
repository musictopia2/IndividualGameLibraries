using BasicGameFramework.CommandClasses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MastermindCP
{
    public class HintChooserViewModel : ObservableObject
    {
        private GlobalClass? _thisGlobal;
        private readonly MastermindMainGameClass _mainGame;
        public PlainCommand<Bead> ChangeMindCommand { get; set; }

        public HintChooserViewModel(MastermindViewModel thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<MastermindMainGameClass>();
            ChangeMindCommand = new PlainCommand<Bead>(thisBead =>
            {
                thisBead.ColorChosen = EnumColorPossibilities.None; //hopefully still that simple.
            }, thisBead =>
            {
                if (thisMod.GameFinished == true)
                    return false;
                if (thisBead.CurrentGuess!.IsCompleted)
                    return false;
                if (thisBead.CurrentGuess.YourBeads.Count == 0)
                    throw new BasicBlankException("Not Sure");
                return true;
            }, thisMod, thisMod.CommandContainer!);
        }

        public CustomBasicCollection<Guess> GuessList = new CustomBasicCollection<Guess>();

        public void NewGame()
        {
            if (_thisGlobal == null)
                _thisGlobal = _mainGame.ThisMod.MainContainer!.Resolve<GlobalClass>(); //trying to prevent overflow errors.
            var tempList = new CustomBasicList<Guess>();
            int guesses = _thisGlobal.HowManyGuess;
            guesses.Times(x =>
            {
                Guess thisGuess = new Guess();
                if (x == 1)
                    thisGuess.GetNewBeads();
                tempList.Add(thisGuess);
            });
            GuessList.ReplaceRange(tempList);
            _thisGlobal.HintScroll.ScrollToGuess(GuessList.First());
        }
        private Guess GetCurrentGuess()
        {
            if (GuessList.Count == 0)
                throw new BasicBlankException("No guess even found.  Rethink");
            return GuessList.Single(items => items.YourBeads.Count == 4 && items.IsCompleted == false);
        }
        public void SelectedColorForCurrentGuess(EnumColorPossibilities thisColor)
        {
            var thisGuess = GetCurrentGuess();
            foreach (var thisBead in thisGuess.YourBeads)
            {
                if (thisBead.ColorChosen == EnumColorPossibilities.None)
                {
                    thisBead.ColorChosen = thisColor;
                    return;
                }
            }
        }
        public bool HasMoreGuessesLeft => !GuessList.Last().IsCompleted;
        public void StartNewGuess()
        {
            var nextGuess = GuessList.Where(items => items.IsCompleted == false).Take(1).Single();
            nextGuess.GetNewBeads();
            _thisGlobal!.HintScroll.ScrollToGuess(nextGuess);
        }
        public bool DidFillInGuess()
        {
            if (GuessList.Count == 0)
                return false;
            var thisGuess = GetCurrentGuess();
            return !thisGuess.YourBeads.Any(items => items.ColorChosen == EnumColorPossibilities.None);
        }
        public async Task SubmitGuessAsync()
        {
            var thisGuess = GetCurrentGuess();
            if (thisGuess.YourBeads.Any(items => items.ColorChosen == EnumColorPossibilities.None))
                return;
            if (thisGuess.YourBeads.Count != 4)
                throw new BasicBlankException("You must have all 4 beads in order to submit the guess.");
            thisGuess.IsCompleted = true;
            var (howManyCorrect, howManyOutOfOrder) = GetHintData(thisGuess);
            thisGuess.HowManyBlacks = howManyCorrect;
            thisGuess.HowManyAquas = howManyOutOfOrder;
            await _mainGame.FinishGuessAsync(thisGuess.HowManyBlacks);
        }
        private (int howManyCorrect, int howManyOutOfOrder) GetHintData(Guess thisGuess)
        {
            _thisGlobal!.StartCheckingSolution();
            if (_thisGlobal.Solution!.Count != 4 || thisGuess.YourBeads.Count != 4)
                throw new BasicBlankException("The solution must have 4 items and your guess must also have 4 items");
            int completeCorrect = 0;
            int semiCorrect = 0;
            for (int x = 0; x < 4; x++)
            {
                if (thisGuess.YourBeads[x].ColorChosen == _thisGlobal.Solution[x].ColorChosen)
                {
                    completeCorrect++;
                    BeadChecked(thisGuess.YourBeads[x], _thisGlobal.Solution[x]);
                }
            }
            var tempList = thisGuess.YourBeads.Where(items => items.DidCheck == false).ToCustomBasicList();
            tempList.ForEach(yourBead =>
            {
                var correctBead = _thisGlobal.Solution.Where(items => items.ColorChosen == yourBead.ColorChosen && items.DidCheck == false).Take(1).SingleOrDefault();
                if (correctBead != null)
                {
                    semiCorrect++;
                    correctBead.DidCheck = true;
                }
            });
            return (completeCorrect, semiCorrect);
        }
        private void BeadChecked(Bead YourBead, Bead CorrectBead)
        {
            YourBead.DidCheck = true;
            CorrectBead.DidCheck = true;
        }
    }
}