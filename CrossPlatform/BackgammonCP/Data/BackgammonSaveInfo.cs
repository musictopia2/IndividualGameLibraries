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
//i think this is the most common things i like to do
namespace BackgammonCP.Data
{
    [SingletonGame]
    public class BackgammonSaveInfo : BasicSavedBoardDiceGameClass<BackgammonPlayerItem>
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
		private int _movesMade;

		public int MovesMade
		{
			get { return _movesMade; }
			set
			{
				if (SetProperty(ref _movesMade, value))
				{
					//can decide what to do when property changes
					if (_model != null)
					{
						_model.MovesMade = value;
					}
				}

			}
		}

		private BackgammonVMData? _model;
		internal void LoadMod(BackgammonVMData model)
		{
			_model = model;
			_model.Instructions = Instructions;
			_model.MovesMade = MovesMade;
		}
		public int SpaceHighlighted { get; set; } = -1;
		public int NumberUsed { get; set; }
		//since its human only now, maybe no need for this part now.

		//public int ComputerSpaceTo { get; set; }
		public bool MadeAtLeastOneMove { get; set; }
		public EnumGameStatus GameStatus { get; set; }
	}
}