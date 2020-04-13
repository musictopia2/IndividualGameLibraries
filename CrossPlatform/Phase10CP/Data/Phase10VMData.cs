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
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using CommonBasicStandardLibraries.Messenging;
using BasicGameFrameworkLibrary.CommandClasses;
using Phase10CP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using BasicGameFrameworkLibrary.ColorCards;
using BasicGameFrameworkLibrary.DIContainers;
using Phase10CP.SetClasses;
//i think this is the most common things i like to do
namespace Phase10CP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class Phase10VMData : ObservableObject, IBasicCardGamesData<Phase10CardInformation>
    {

		public Phase10VMData(IEventAggregator aggregator, CommandContainer command, IGamePackageResolver resolver)
		{
			Deck1 = new DeckObservablePile<Phase10CardInformation>(aggregator, command);
			Pile1 = new PileObservable<Phase10CardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<Phase10CardInformation>(command);
			TempSets = new TempSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation>(command, resolver);
			MainSets = new MainSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation, PhaseSet, SavedSet>(command);
			TempSets.HowManySets = 5;

		}
		public TempSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation> TempSets;
		public MainSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation, PhaseSet, SavedSet> MainSets;
		public DeckObservablePile<Phase10CardInformation> Deck1 { get; set; }
		public PileObservable<Phase10CardInformation> Pile1 { get; set; }
		public HandObservable<Phase10CardInformation> PlayerHand1 { get; set; }
		public PileObservable<Phase10CardInformation>? OtherPile { get; set; }
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

		private string _currentPhase = "";
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


		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
