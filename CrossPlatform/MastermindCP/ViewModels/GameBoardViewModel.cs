using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using MastermindCP.Data;
using MastermindCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MastermindCP.ViewModels
{
    /// <summary>
    /// this is the hint view model.  however, had to make it a gameboard so it fits the new pattern.
    /// </summary>
    [InstanceGame]
    public class GameBoardViewModel : Screen, IBlankGameVM
    {
        private readonly GlobalClass _global;
        private readonly IEventAggregator _aggregator;
        private readonly IFinishGuess _mainGame;

        
        public CustomBasicCollection<Guess> GuessList = new CustomBasicCollection<Guess>();
        internal Task NewGameAsync()
        {
            //this gets called later.
            var tempList = new CustomBasicList<Guess>();
            int guesses = _global.HowManyGuess;
            guesses.Times(x =>
            {
                Guess thisGuess = new Guess();
                if (x == 1)
                    thisGuess.GetNewBeads();
                tempList.Add(thisGuess);
            });
            GuessList.ReplaceRange(tempList);
            return _aggregator.ScrollToGuessAsync(GuessList.First());
        }
        public bool CanChangeMind(Bead bead)
        {
            if (bead.CurrentGuess!.IsCompleted)
            {
                return false;
            }
            if (bead.CurrentGuess.YourBeads.Count == 0)
            {
                throw new BasicBlankException("Not Sure");
            }
            return true;
        }
        [Command(EnumCommandCategory.Plain)]
        public void ChangeMind(Bead bead)
        {
            bead.ColorChosen = EnumColorPossibilities.None;
        }
        //well see what we need for this one.
        public GameBoardViewModel(GlobalClass global, IEventAggregator aggregator, IFinishGuess mainGame, CommandContainer commandContainer)
        {
            _global = global;
            _aggregator = aggregator;
            _mainGame = mainGame;
            CommandContainer = commandContainer;
        }
        private Guess GetCurrentGuess()
        {
            if (GuessList.Count == 0)
                throw new BasicBlankException("No guess even found.  Rethink");
            return GuessList.Single(items => items.YourBeads.Count == 4 && items.IsCompleted == false);
        }
        internal void SelectedColorForCurrentGuess(EnumColorPossibilities thisColor)
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
        internal bool HasMoreGuessesLeft => !GuessList.Last().IsCompleted;

        public CommandContainer CommandContainer { get; set; }

        internal async Task StartNewGuessAsync()
        {
            var nextGuess = GuessList.Where(items => items.IsCompleted == false).Take(1).Single();
            nextGuess.GetNewBeads();
            await _aggregator.ScrollToGuessAsync(nextGuess);
        }
        internal bool DidFillInGuess()
        {
            if (GuessList.Count == 0)
                return false;
            var thisGuess = GetCurrentGuess();
            return !thisGuess.YourBeads.Any(items => items.ColorChosen == EnumColorPossibilities.None);
        }
        internal async Task SubmitGuessAsync()
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
            await _mainGame.FinishGuessAsync(thisGuess.HowManyBlacks, this);
        }
        private (int howManyCorrect, int howManyOutOfOrder) GetHintData(Guess thisGuess)
        {
            _global!.StartCheckingSolution();
            if (_global.Solution!.Count != 4 || thisGuess.YourBeads.Count != 4)
                throw new BasicBlankException("The solution must have 4 items and your guess must also have 4 items");
            int completeCorrect = 0;
            int semiCorrect = 0;
            for (int x = 0; x < 4; x++)
            {
                if (thisGuess.YourBeads[x].ColorChosen == _global.Solution[x].ColorChosen)
                {
                    completeCorrect++;
                    BeadChecked(thisGuess.YourBeads[x], _global.Solution[x]);
                }
            }
            var tempList = thisGuess.YourBeads.Where(items => items.DidCheck == false).ToCustomBasicList();
            tempList.ForEach(yourBead =>
            {
                var correctBead = _global.Solution.Where(items => items.ColorChosen == yourBead.ColorChosen && items.DidCheck == false).Take(1).SingleOrDefault();
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
