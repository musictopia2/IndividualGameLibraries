using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Linq;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace CribbagePatienceCP
{
    public class ScoreSummaryCP : ObservableObject
    {

        public CustomBasicCollection<int> ScoreList
        {
            get => _mainGame.SaveRoot.ScoreList;
            set => _mainGame.SaveRoot.ScoreList = value;
        }


        private readonly CribbagePatienceGameClass _mainGame;
        public ScoreSummaryCP()
        {
            _mainGame = Resolve<CribbagePatienceGameClass>();

        }

        public int TotalScore
        {
            get
            {
                return ScoreList.Sum();
            }
        }// has to be property so it can be bound

        public void Reload()
        {
            OnPropertyChanged(nameof(TotalScore)); // i think this too.
        }

        public void AddScore(int Score)
        {
            ScoreList.Add(Score);
            OnPropertyChanged(nameof(TotalScore)); // so the ui is notified to update values
        }

        public void NewGame()
        {
            ScoreList.Clear();
            OnPropertyChanged(nameof(TotalScore));
        }

    }
}