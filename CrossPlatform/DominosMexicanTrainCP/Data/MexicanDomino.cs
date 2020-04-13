using BasicGameFrameworkLibrary.Dominos;
using DominosMexicanTrainCP.Logic;
using System.Linq;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace DominosMexicanTrainCP.Data
{
    public class MexicanDomino : SimpleDominoInfo
    {
        public override int Points
        {
            get
            {
                if (FirstNum == 0 && SecondNum == 0)
                    return 50;
                return base.Points;
            }
        }
        private DominosMexicanTrainMainGameClass? _mainGame;
        public override int HighestDomino
        {
            get
            {
                if (_mainGame == null)
                    _mainGame = Resolve<DominosMexicanTrainMainGameClass>(); //needs this
                if (_mainGame.PlayerList.Count() < 5)
                    return 12;
                return 15;
            }
        }
        public int Train { get; set; }
        public string Status { get; set; } = "";
        public int Keeps { get; set; }
    }
}
