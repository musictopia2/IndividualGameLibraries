using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
namespace Pinochle2PlayerCP
{
    public class ScoreGuideViewModel : ObservableObject
    {
        public CustomBasicList<ScoreValuePair> PointValueList = new CustomBasicList<ScoreValuePair>();
        public CustomBasicList<ScoreValuePair> ClassAList = new CustomBasicList<ScoreValuePair>();
        public CustomBasicList<ScoreValuePair> ClassBList = new CustomBasicList<ScoreValuePair>();
        public CustomBasicList<ScoreValuePair> ClassCList = new CustomBasicList<ScoreValuePair>();
        private CustomBasicList<ScoreValuePair> PopulatePoints()
        {
            return new CustomBasicList<ScoreValuePair>()
            {
                new ScoreValuePair("Each Ace", 11),
                new ScoreValuePair("Each 10", 10),
                new ScoreValuePair("Each King", 4),
                new ScoreValuePair("Each Queen", 3),
                new ScoreValuePair("Each Jack", 2),
                new ScoreValuePair("Last Trick", 10)
            };
        }
        private CustomBasicList<ScoreValuePair> PopulateACustomBasicList()
        {
            return new CustomBasicList<ScoreValuePair>()
            {
                new ScoreValuePair("A, K, Q, J, 10 of trump suit (flush or sequence)", 150),
                new ScoreValuePair("K, Q of trump (royal marriage)", 40),
                new ScoreValuePair("K, Q same suit of any other suit (marriage)", 20),
                new ScoreValuePair("9 of trump-(lowest trump)", 10)
            };
        }
        private CustomBasicList<ScoreValuePair> PopulateBCustomBasicList()
        {
            return new CustomBasicList<ScoreValuePair>()
            {
                new ScoreValuePair("4 Aces (A) of different suits", 100),
                new ScoreValuePair("4 Kings (K) of different suits", 80),
                new ScoreValuePair("4 Queens (Q) of different suits", 60),
                new ScoreValuePair("4 Jacks (J) of different suits", 40)
            };
        }
        private CustomBasicList<ScoreValuePair> PopulateCCustomBasicList()
        {
            return new CustomBasicList<ScoreValuePair>()
            {
                new ScoreValuePair("Queen of Spades and Jack of Diamonds" + Constants.vbCrLf + "(Pinochle)", 40)
            };
        }
        public ScoreGuideViewModel()
        {
            PointValueList = PopulatePoints();
            ClassAList = PopulateACustomBasicList();
            ClassBList = PopulateBCustomBasicList();
            ClassCList = PopulateCCustomBasicList();
        }
    }
}