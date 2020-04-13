using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Dominos;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Messenging;
using LottoDominosCP.EventModels;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LottoDominosCP.Data
{
    [SingletonGame]
    public class LottoDominosSaveInfo : BasicSavedGameClass<LottoDominosPlayerItem>
    { //anything needed for autoresume is here.
        public DeckRegularDict<SimpleDominoInfo> ComputerList = new DeckRegularDict<SimpleDominoInfo>();
        public DeckRegularDict<SimpleDominoInfo>? BoardDice { get; set; }

        private EnumStatus _gameStatus;

        public EnumStatus GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (SetProperty(ref _gameStatus, value))
                {
                    
                }
            }
        }
        
    }
}