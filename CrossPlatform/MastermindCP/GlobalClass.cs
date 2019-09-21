using BasicGameFramework.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
namespace MastermindCP
{
    [SingletonGame]
    public class GlobalClass
    {
        public CustomBasicList<Bead>? Solution; //decided to put here.

        private readonly MastermindViewModel _thisMod;

        internal readonly IScroll HintScroll;

        internal readonly IEndSolution EndUI;

        public GlobalClass(MastermindViewModel thisMod, IScroll hintScroll, IEndSolution endUI)
        {
            _thisMod = thisMod;
            HintScroll = hintScroll;
            EndUI = endUI;
        }

        public int HowManyGuess
        {
            get
            {
                if (_thisMod.LevelChosen == 1)
                    return 4;
                return _thisMod.LevelChosen + 4;
            }
        }
        public void StartCheckingSolution()
        {
            Solution!.ForEach(thisBead => thisBead.DidCheck = false);
        }


        internal CustomBasicList<EnumColorPossibilities> ColorList = new CustomBasicList<EnumColorPossibilities>();

    }
}
