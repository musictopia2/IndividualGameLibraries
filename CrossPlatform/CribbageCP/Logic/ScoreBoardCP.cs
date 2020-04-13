using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CribbageCP.Data;
using System.Linq;

namespace CribbageCP.Logic
{
    public class ScoreBoardCP : ObservableObject
    {
        private readonly CustomBasicList<ScoreInfo> _tempList = new CustomBasicList<ScoreInfo>();
        public CustomBasicCollection<ScoreInfo> ScoreList = new CustomBasicCollection<ScoreInfo>();
        public int TotalScore => _tempList.Sum(items => items.Score);
        public void AddScore(string description, int score)
        {
            ScoreInfo thisScore = new ScoreInfo();
            thisScore.Description = description;
            thisScore.Score = score;
            _tempList.Add(thisScore);
        }
        public void ShowScores()
        {
            ScoreList.ReplaceRange(_tempList);
            OnPropertyChanged(nameof(TotalScore));
        }
        public void ResetScores()
        {
            if (_tempList.Count == 0)
                return; //already done.
            _tempList.Clear();
            ScoreList.Clear();
            OnPropertyChanged(nameof(TotalScore));
        }
    }
}
