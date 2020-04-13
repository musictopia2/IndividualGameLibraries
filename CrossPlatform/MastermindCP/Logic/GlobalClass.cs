using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.CollectionClasses;
using MastermindCP.Data;
namespace MastermindCP.Logic
{
    [SingletonGame]
    public class GlobalClass
    {
        private readonly LevelClass _level;
        public CustomBasicList<Bead>? Solution;
        public GlobalClass(LevelClass level)
        {
            _level = level;
        }
        public int HowManyGuess
        {
            get
            {
                if (_level.LevelChosen == 1)
                    return 4;
                return _level.LevelChosen + 4;
            }
        }
        public void StartCheckingSolution()
        {
            Solution!.ForEach(thisBead => thisBead.DidCheck = false);
        }


        internal CustomBasicList<EnumColorPossibilities> ColorList { get; set; } = new CustomBasicList<EnumColorPossibilities>();
    }
}
