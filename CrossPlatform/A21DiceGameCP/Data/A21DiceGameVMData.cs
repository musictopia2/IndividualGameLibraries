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
namespace A21DiceGameCP.Data
{
    [SingletonGame]
	[AutoReset]
    public class A21DiceGameVMData : ObservableObject, IBasicDiceGamesData<SimpleDice>
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
		private readonly IGamePackageResolver _resolver;
		private readonly CommandContainer _command;

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

		public DiceCup<SimpleDice>? Cup { get; set; }
		public A21DiceGameVMData(IGamePackageResolver resolver, CommandContainer command)
		{
			_resolver = resolver;
			_command = command;
		}
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
			Cup.HowManyDice = 1; //you specify how many dice here.
			Cup.Visible = true; //i think.
		}


		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
