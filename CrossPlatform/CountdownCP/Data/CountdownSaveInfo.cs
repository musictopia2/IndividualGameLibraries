using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.CollectionClasses;
namespace CountdownCP.Data
{
    [SingletonGame]
    public class CountdownSaveInfo : BasicSavedDiceClass<CountdownDice, CountdownPlayerItem>
    { //anything needed for autoresume is here.

        private int _round;

        public int Round
        {
            get { return _round; }
            set
            {
                if (SetProperty(ref _round, value))
                {
                    if (_model == null)
                    {
                        return;
                    }
                    _model.Round = value;
                }

            }
        }
        public CustomBasicList<int> HintList { get; set; } = new CustomBasicList<int>();

        private CountdownVMData? _model;
        internal void LoadMod(CountdownVMData model)
        {
            _model = model;
            _model.Round = Round;
        }
    }
}