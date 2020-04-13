using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Exceptions;
using XactikaCP.Cards;
namespace XactikaCP.Data
{
    [SingletonGame]
    public class XactikaSaveInfo : BasicSavedTrickGamesClass<EnumShapes, XactikaCardInformation, XactikaPlayerItem>
    { //anything needed for autoresume is here.

        public EnumStatusList GameStatus { get; set; }


        private EnumGameMode _gameMode = EnumGameMode.None;

        public EnumGameMode GameMode
        {
            get { return _gameMode; }
            set
            {
                if (SetProperty(ref _gameMode, value))
                {
                    //can decide what to do when property changes
                    ProcessMode();
                }

            }
        }
        private void ProcessMode()
        {
            if (_model == null)
                return;
            _model.GameModeText = GameMode switch
            {
                EnumGameMode.None => "None",
                EnumGameMode.ToWin => "Win",
                EnumGameMode.ToLose => "Lose",
                EnumGameMode.ToBid => "Bid",
                _ => throw new BasicBlankException("Nothing found for mode"),
            };
            _model.ModeChosen = GameMode; //hopefully this simple.
        }
        private int _roundNumber;

        public int RoundNumber
        {
            get { return _roundNumber; }
            set
            {
                if (SetProperty(ref _roundNumber, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.RoundNumber = value;
                }

            }
        }
        public int Value { get; set; } //1, 2, or 3

        private EnumShapes _shapeChosen;

        public EnumShapes ShapeChosen
        {
            get { return _shapeChosen; }
            set
            {
                if (SetProperty(ref _shapeChosen, value))
                {
                    //can decide what to do when property changes
                    if (_model != null)
                        _model.ShapeChosen = value;
                }

            }
        }

        private XactikaVMData? _model;

        public void LoadMod(XactikaVMData model)
        {
            _model = model;
            ProcessMode();
            _model.RoundNumber = RoundNumber;
            _model.ShapeChosen = ShapeChosen;
        }
        public bool ClearTricks { get; set; }
    }
}