using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CribbagePatienceCP.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace CribbagePatienceCP.Data
{
    public class ScoreSummaryCP : ObservableObject
    {

        public CustomBasicCollection<int> ScoreList
        {
            get => _mainGame._saveRoot.ScoreList;
            set => _mainGame._saveRoot.ScoreList = value;
        }


        private readonly CribbagePatienceMainGameClass _mainGame;
        public ScoreSummaryCP()
        {
            _mainGame = Resolve<CribbagePatienceMainGameClass>();

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
