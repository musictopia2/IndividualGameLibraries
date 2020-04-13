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
using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
//i think this is the most common things i like to do
namespace RummyDiceCP.Data
{
    [SingletonGame]
    public class RummyDiceVMData : ObservableObject, IViewModelData
    {
		private string _normalTurn = "";
		[VM]
		public string NormalTurn
		{
			get { return _normalTurn; }
			set
			{
				if (SetProperty(ref _normalTurn, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private string _status = "";
		[VM] //use this tag to transfer to the actual view model.  this is being done to avoid overflow errors.
		public string Status
		{
			get { return _status; }
			set
			{
				if (SetProperty(ref _status, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private int _rollNumber;
		[VM]
		public int RollNumber
		{
			get { return _rollNumber; }
			set
			{
				if (SetProperty(ref _rollNumber, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private string _currentPhase = "None";
		[VM]
		public string CurrentPhase
		{
			get { return _currentPhase; }
			set
			{
				if (SetProperty(ref _currentPhase, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private int _score;
		[VM]
		public int Score
		{
			get { return _score; }
			set
			{
				if (SetProperty(ref _score, value))
				{
					//can decide what to do when property changes
				}

			}
		}


	}
}
