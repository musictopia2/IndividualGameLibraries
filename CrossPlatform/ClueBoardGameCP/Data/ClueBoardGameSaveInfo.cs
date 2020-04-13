using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using System.Collections.Generic;
//i think this is the most common things i like to do
namespace ClueBoardGameCP.Data
{
    [SingletonGame]
    public class ClueBoardGameSaveInfo : BasicSavedBoardDiceGameClass<ClueBoardGamePlayerItem>
    { //anything needed for autoresume is here.
		private string _instructions = "";

		public string Instructions
		{
			get { return _instructions; }
			set
			{
				if (SetProperty(ref _instructions, value))
				{
					//can decide what to do when property changes
					if(_model != null)
					{
						_model.Instructions = value;
					}
				}

			}
		}
		private ClueBoardGameVMData? _model;
		internal void LoadMod(ClueBoardGameVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
            _model.LeftToMove = MovesLeft;
		}


        public int DiceNumber { get; set; }
        public PredictionInfo? CurrentPrediction { get; set; }
        private int _movesLeft;
        public int MovesLeft
        {
            get { return _movesLeft; }
            set
            {
                if (SetProperty(ref _movesLeft, value))
                {
                    if (_model != null)
                    {
                        _model.LeftToMove = MovesLeft; //i think.
                    }
                }
            }
        }
        public bool AccusationMade { get; set; }
        public bool ShowedMessage { get; set; }
        public Dictionary<int, CharacterInfo> CharacterList { get; set; } = new Dictionary<int, CharacterInfo>();
        public PredictionInfo Solution { get; set; } = new PredictionInfo();
        public Dictionary<int, MoveInfo> PreviousMoves { get; set; } = new Dictionary<int, MoveInfo>();
        public Dictionary<int, WeaponInfo> WeaponList { get; set; } = new Dictionary<int, WeaponInfo>(); //needs this also.
        public EnumClueStatusList GameStatus { get; set; }


    }
}