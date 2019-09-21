using BasicGameFramework.Attributes;
using BasicGameFramework.MultiplayerClasses.SavedGameClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace XactikaCP
{
    [SingletonGame]
    public class XactikaSaveInfo : BasicSavedTrickGamesClass<EnumShapes, XactikaCardInformation, XactikaPlayerItem>
    { //anything needed for autoresume is here.
        private EnumStatusList _GameStatus;

        public EnumStatusList GameStatus
        {
            get { return _GameStatus; }
            set
            {
                if (SetProperty(ref _GameStatus, value))
                {
                    StatusProcesses();
                }

            }
        }
        private void StatusProcesses()
        {
            if (_thisMod == null)
                return;
            _thisMod.VisibleChanges();
            //maybe i don't need the other part because of the visible changes.
        }
        private EnumGameMode _GameMode = EnumGameMode.None;

        public EnumGameMode GameMode
        {
            get { return _GameMode; }
            set
            {
                if (SetProperty(ref _GameMode, value))
                {
                    //can decide what to do when property changes
                    ProcessMode();
                }

            }
        }
        private void ProcessMode()
        {
            if (_thisMod == null)
                return;
            _thisMod.GameModeText = GameMode switch
            {
                EnumGameMode.None => "None",
                EnumGameMode.ToWin => "Win",
                EnumGameMode.ToLose => "Lose",
                EnumGameMode.ToBid => "Bid",
                _ => throw new BasicBlankException("Nothing found for mode"),
            };
            _thisMod.ModeChosen = GameMode; //hopefully this simple.
        }
        private int _RoundNumber;

        public int RoundNumber
        {
            get { return _RoundNumber; }
            set
            {
                if (SetProperty(ref _RoundNumber, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.RoundNumber = value;
                }

            }
        }
        public int Value { get; set; } //1, 2, or 3

        private EnumShapes _ShapeChosen;

        public EnumShapes ShapeChosen
        {
            get { return _ShapeChosen; }
            set
            {
                if (SetProperty(ref _ShapeChosen, value))
                {
                    //can decide what to do when property changes
                    if (_thisMod != null)
                        _thisMod.ShapeChosen = value;
                }

            }
        }

        private XactikaViewModel? _thisMod;

        public void LoadMod(XactikaViewModel thisMod)
        {
            _thisMod = thisMod;
            StatusProcesses();
            ProcessMode();
            thisMod.RoundNumber = RoundNumber;
            thisMod.ShapeChosen = ShapeChosen;
        }
        public bool ClearTricks { get; set; }
    }
}