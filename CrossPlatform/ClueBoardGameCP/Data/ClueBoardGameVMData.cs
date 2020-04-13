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
using BasicGameFrameworkLibrary.DrawableListsObservable;
using ClueBoardGameCP.Cards;
using CommonBasicStandardLibraries.Messenging;
//i think this is the most common things i like to do
namespace ClueBoardGameCP.Data
{
    [SingletonGame]
	[AutoReset]
    public class ClueBoardGameVMData : ObservableObject, IDiceBoardGamesData
	{
		private readonly CommandContainer _command;
		private readonly IGamePackageResolver _resolver;
		public HandObservable<CardInfo> HandList; //this time, needs the hand
		public PileObservable<CardInfo> Pile;

		public ClueBoardGameVMData(CommandContainer command, IGamePackageResolver resolver, IEventAggregator aggregator)
		{
			_command = command;
			_resolver = resolver;
			HandList = new HandObservable<CardInfo>(command);
			HandList.AutoSelect = HandObservable<CardInfo>.EnumAutoType.None;
			HandList.Maximum = 3;
			HandList.Text = "Your Cards";
			Pile = new PileObservable<CardInfo>(aggregator, command);
			Pile.CurrentOnly = true;
			Pile.Text = "Clue";
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

		private int _leftToMove;
		[VM]
		public int LeftToMove
		{
			get { return _leftToMove; }
			set
			{
				if (SetProperty(ref _leftToMove, value))
				{
					//can decide what to do when property changes
				}

			}
		}


		private string _currentRoomName = "";
		[VM]
		public string CurrentRoomName
		{
			get { return _currentRoomName; }
			set
			{
				if (SetProperty(ref _currentRoomName, value))
				{
					//can decide what to do when property changes
				}
			}
		}
		private string _currentCharacterName = "";
		[VM]
		public string CurrentCharacterName
		{
			get { return _currentCharacterName; }
			set
			{
				if (SetProperty(ref _currentCharacterName, value))
				{
					//can decide what to do when property changes
				}
			}
		}
		private string _currentWeaponName = "";
		[VM]
		public string CurrentWeaponName
		{
			get { return _currentWeaponName; }
			set
			{
				if (SetProperty(ref _currentWeaponName, value))
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
			Cup.HowManyDice = 1; //you specify how many dice here.
			Cup.Visible = true; //i think.

		}



	}
}
