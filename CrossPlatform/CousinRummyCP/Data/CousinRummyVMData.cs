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
using BasicGameFrameworkLibrary.SpecializedGameTypes.RummyClasses;
using CousinRummyCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace CousinRummyCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class CousinRummyVMData : ObservableObject, IBasicCardGamesData<RegularRummyCard>
    {

		public CousinRummyVMData(IEventAggregator aggregator, CommandContainer command, IGamePackageResolver resolver)
		{
			Deck1 = new DeckObservablePile<RegularRummyCard>(aggregator, command);
			Pile1 = new PileObservable<RegularRummyCard>(aggregator, command);
			PlayerHand1 = new HandObservable<RegularRummyCard>(command);
			TempSets = new TempSetsObservable<EnumSuitList, EnumColorList, RegularRummyCard>(command, resolver);
			MainSets = new MainSetsObservable<EnumSuitList, EnumColorList, RegularRummyCard, PhaseSet, SavedSet>(command);
			TempSets.HowManySets = 8;
			MainSets.Text = "Main Sets";
		}
		public TempSetsObservable<EnumSuitList, EnumColorList, RegularRummyCard> TempSets;
		public MainSetsObservable<EnumSuitList, EnumColorList, RegularRummyCard, PhaseSet, SavedSet> MainSets;
		public DeckObservablePile<RegularRummyCard> Deck1 { get; set; }
		public PileObservable<RegularRummyCard> Pile1 { get; set; }
		public HandObservable<RegularRummyCard> PlayerHand1 { get; set; }
		public PileObservable<RegularRummyCard>? OtherPile { get; set; }
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

		private string _phaseData = "";
		[VM]
		public string PhaseData
		{
			get { return _phaseData; }
			set
			{
				if (SetProperty(ref _phaseData, value))
				{
					//can decide what to do when property changes
				}

			}
		}
		private string _otherLabel = "";
		[VM]
		public string OtherLabel
		{
			get { return _otherLabel; }
			set
			{
				if (SetProperty(ref _otherLabel, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
