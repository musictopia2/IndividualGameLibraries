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
using BasicGameFrameworkLibrary.MultiplayerClasses.InterfaceModels;
using BasicGameFrameworkLibrary.Dice;
using BasicGameFrameworkLibrary.MultiplayerClasses.SavedGameClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace BackgammonCP.Data
{
    [SingletonGame]
    public class BackgammonVMData : ObservableObject, IDiceBoardGamesData
	{
		private readonly CommandContainer _command;
		private readonly IGamePackageResolver _resolver;
		public BackgammonVMData(CommandContainer command, IGamePackageResolver resolver)
		{
			_command = command;
			_resolver = resolver;
		}
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

		private string _instructions = "";
		[VM]
		public string Instructions
		{
			get { return _instructions; }
			set
			{
				if (SetProperty(ref _instructions, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private int _movesMade;
		[VM]
		public int MovesMade
		{
			get { return _movesMade; }
			set
			{
				if (SetProperty(ref _movesMade, value))
				{
					//can decide what to do when property changes
				}
			}
		}
		private string _lastStatus = "";
		[VM]
		public string LastStatus
		{
			get { return _lastStatus; }
			set
			{
				if (SetProperty(ref _lastStatus, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		public DiceCup<SimpleDice>? Cup { get; set; }

		public void LoadCup(ISavedDiceList<SimpleDice> saveRoot, bool autoResume)
		{
			if (Cup != null && autoResume)
			{
				return;
			}
			Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
			if (autoResume == true)
			{
				Cup.CanShowDice = true;
			}
			Cup.HowManyDice = 2; //you specify how many dice here.
			Cup.Visible = true; //i think.

		}

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
