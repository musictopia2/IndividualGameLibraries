using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace MastermindCP
{
    [SingletonGame]
    public class MastermindMainGameClass
    {
        internal MastermindViewModel ThisMod;
        private GlobalClass? _thisGlobal;
        public MastermindMainGameClass(MastermindViewModel thisMod)
        {
            ThisMod = thisMod;
        }
        public void NewGame()
        {
            if (_thisGlobal == null)
                _thisGlobal = ThisMod.MainContainer!.Resolve<GlobalClass>(); //doing it this way to try to prevent overflow errors.
            _thisGlobal.EndUI.HideSolution();
            bool canRepeat = ThisMod.LevelChosen == 2 || ThisMod.LevelChosen == 4 || ThisMod.LevelChosen == 6;
            CustomBasicList<Bead> possibleList = new CustomBasicList<Bead>();
            int level = ThisMod.LevelChosen;
            if (level == 5 || level == 6)
            {
                possibleList.Add(new Bead(EnumColorPossibilities.Aqua));
                possibleList.Add(new Bead(EnumColorPossibilities.Black));
            }
            possibleList.Add(new Bead(EnumColorPossibilities.Blue));
            possibleList.Add(new Bead(EnumColorPossibilities.Green));
            if (level > 2)
                possibleList.Add(new Bead(EnumColorPossibilities.Purple));
            possibleList.Add(new Bead(EnumColorPossibilities.Red));
            possibleList.Add(new Bead(EnumColorPossibilities.White));
            if (level > 2)
                possibleList.Add(new Bead(EnumColorPossibilities.Yellow));
            ICustomBasicList<Bead> tempList;
            if (canRepeat == false)
                tempList = possibleList.GetRandomList(false, 4);
            else
            {
                int x;
                tempList = new CustomBasicList<Bead>();
                for (x = 1; x <= 4; x++)
                {
                    var ThisBead = possibleList.GetRandomItem();
                    tempList.Add(ThisBead); // can have repeat
                }
            }
            _thisGlobal.Solution = tempList.ToCustomBasicList();
            ThisMod.Guess1!.NewGame();
            ThisMod.Guess1!.StartNewGuess();
            _thisGlobal.ColorList = possibleList.Select(items => items.ColorChosen).ToCustomBasicList();
            ThisMod.Color1!.LoadEntireList(); //try this way.
            ThisMod.GameFinished = false; //hopefully this is fine.
        }
        public async Task GiveUpAsync()
        {
            await ThisMod.ShowGameMessageAsync("Sorry you are giving up");
            ThisMod.GameFinished = true;
            _thisGlobal!.EndUI.ShowSolution();
        }
        public async Task FinishGuessAsync(int howManyCorrect)
        {
            if (howManyCorrect == 4)
            {
                await ThisMod.ShowGameMessageAsync("Congratuations, you won");
                _thisGlobal!.EndUI.ShowSolution();
                ThisMod.GameFinished = true;
                return;
            }
            if (ThisMod.Guess1!.GuessList.Last().IsCompleted)
            {
                await ThisMod.ShowGameMessageAsync("You ran out of guesses.");
                _thisGlobal!.EndUI.ShowSolution();
                ThisMod.GameFinished = true;
                return;
            }
            ThisMod.Guess1.StartNewGuess();
        }
    }
}