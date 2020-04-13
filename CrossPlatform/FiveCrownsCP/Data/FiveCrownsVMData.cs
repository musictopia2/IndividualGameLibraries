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
using FiveCrownsCP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using FiveCrownsCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace FiveCrownsCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class FiveCrownsVMData : ObservableObject, IBasicCardGamesData<FiveCrownsCardInformation>
    {

		public FiveCrownsVMData(IEventAggregator aggregator, CommandContainer command, IGamePackageResolver resolver)
		{
			Deck1 = new DeckObservablePile<FiveCrownsCardInformation>(aggregator, command);
			Pile1 = new PileObservable<FiveCrownsCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<FiveCrownsCardInformation>(command);
			TempSets = new TempSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation>(command, resolver);
			MainSets = new MainSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation, PhaseSet, SavedSet>(command);
			TempSets.HowManySets = 6;
		}
		public TempSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation> TempSets;
		public MainSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation, PhaseSet, SavedSet> MainSets;
		public DeckObservablePile<FiveCrownsCardInformation> Deck1 { get; set; }
		public PileObservable<FiveCrownsCardInformation> Pile1 { get; set; }
		public HandObservable<FiveCrownsCardInformation> PlayerHand1 { get; set; }
		public PileObservable<FiveCrownsCardInformation>? OtherPile { get; set; }
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

		private int _upTo;
		[VM]
		public int UpTo
		{
			get { return _upTo; }
			set
			{
				if (SetProperty(ref _upTo, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
