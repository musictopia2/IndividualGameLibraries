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
using LifeBoardGameCP.Cards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.Messenging;
using static BasicGameFrameworkLibrary.ChooserClasses.ListViewPicker;
//i think this is the most common things i like to do
namespace LifeBoardGameCP.Data
{
    [SingletonGame]
    public class LifeBoardGameVMData : ObservableObject, ISimpleBoardGamesData, IEnableAlways
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


		//any other ui related properties will be here.
		//can copy/paste for the actual view model.
		private int _currentSelected; //this is the space you selected so far when you have a choice between 2 spaces.
		[VM]
		public int CurrentSelected
		{
			get { return _currentSelected; }
			set
			{
				if (SetProperty(ref _currentSelected, value))
				{
					//can decide what to do when property changes
				}
			}
		}

		private EnumWhatStatus _gameStatus = EnumWhatStatus.None;
		[VM]
		public EnumWhatStatus GameStatus
		{
			get { return _gameStatus; }
			set
			{
				if (SetProperty(ref _gameStatus, value))
				{
					
				}
			}
		}
		private string _gameDetails = "";
		[VM]
		public string GameDetails
		{
			get { return _gameDetails; }
			set
			{
				if (SetProperty(ref _gameDetails, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		//i don't think we need gender part at all (somebody else is handling it anyways).
		public HandObservable<LifeBaseCard> HandList;

		public SimpleEnumPickerVM<EnumGender, CirclePieceCP<EnumGender>> GenderChooser { get; set; }
		public string PlayerChosen { get; set; } = "";
		public ListViewPicker PlayerPicker { get; set; }
		public PileObservable<LifeBaseCard> SinglePile { get; set; }
		public LifeBoardGameVMData(CommandContainer command, IGamePackageResolver resolver, IEventAggregator aggregator)
		{
			HandList = new HandObservable<LifeBaseCard>(command);
			GenderChooser = new SimpleEnumPickerVM<EnumGender, CirclePieceCP<EnumGender>>(command, new ColorListChooser<EnumGender>()); //hopefully still works.
			GenderChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
			PlayerPicker = new ListViewPicker(command, resolver);
			PlayerPicker.IndexMethod = EnumIndexMethod.OneBased;
			PlayerPicker.ItemSelectedAsync += PlayerPicker_ItemSelectedAsync;
			SinglePile = new PileObservable<LifeBaseCard>(aggregator, command);
			SinglePile.SendAlwaysEnable(this); //hopefuly this simple.
			SinglePile.Text = "Card";
			SinglePile.CurrentOnly = true;
			HandList.Text = "None";
			HandList.IgnoreMaxRules = true;
		}

		private Task PlayerPicker_ItemSelectedAsync(int SelectedIndex, string SelectedText)
		{
			PlayerChosen = SelectedText;
			return Task.CompletedTask;
		}

		public int GetRandomCard => HandList.HandList.GetRandomItem().Deck;

		bool IEnableAlways.CanEnableAlways()
		{
			return false;
		}
	}
}
