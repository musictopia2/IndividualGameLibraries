using BasicGameFramework.Attributes;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.Dominos;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LottoDominosCP
{
    [SingletonGame]
    public class LottoDominosSaveInfo : BasicSavedGameClass<LottoDominosPlayerItem>
    {
        public DeckRegularDict<SimpleDominoInfo> ComputerList = new DeckRegularDict<SimpleDominoInfo>();
        public DeckRegularDict<SimpleDominoInfo>? BoardDice { get; set; }

        private EnumStatus _GameStatus;
        public EnumStatus GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    _thisMod.MakeControlsVisible();
                }
            }
        }
        private readonly IControlsVisible _thisMod;
        public LottoDominosSaveInfo()
        {
            _thisMod = Resolve<IControlsVisible>(); //don't expect to do unit testing on this game.
        }
    }
}