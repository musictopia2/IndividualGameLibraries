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
using MonasteryCardGameCP.Logic;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace MonasteryCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class MonasteryCardGameVMData : ObservableObject, IBasicCardGamesData<MonasteryCardInfo>
    {

		public MonasteryCardGameVMData(IEventAggregator aggregator, CommandContainer command, IGamePackageResolver resolver)
		{
			Deck1 = new DeckObservablePile<MonasteryCardInfo>(aggregator, command);
			Pile1 = new PileObservable<MonasteryCardInfo>(aggregator, command);
			PlayerHand1 = new HandObservable<MonasteryCardInfo>(command);
			TempSets = new TempSetsObservable<EnumSuitList, EnumColorList, MonasteryCardInfo>(command, resolver);
			MainSets = new MainSetsObservable<EnumSuitList, EnumColorList, MonasteryCardInfo, RummySet, SavedSet>(command);
			TempSets.HowManySets = 4;

		}
		public TempSetsObservable<EnumSuitList, EnumColorList, MonasteryCardInfo> TempSets;
		public MainSetsObservable<EnumSuitList, EnumColorList, MonasteryCardInfo, RummySet, SavedSet> MainSets;
		public DeckObservablePile<MonasteryCardInfo> Deck1 { get; set; }
		public PileObservable<MonasteryCardInfo> Pile1 { get; set; }
		public HandObservable<MonasteryCardInfo> PlayerHand1 { get; set; }
		public PileObservable<MonasteryCardInfo>? OtherPile { get; set; }

		public CustomBasicCollection<MissionList> CompleteMissions = new CustomBasicCollection<MissionList>();

		internal void PopulateMissions(CustomBasicList<MissionList> thisList)
		{
			MissionChosen = "";
			CompleteMissions.ReplaceRange(thisList);
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



		//any other ui related properties will be here.
		//can copy/paste for the actual view model.
		private string _missionChosen = "";
		[VM]
		public string MissionChosen
		{
			get { return _missionChosen; }
			set
			{
				if (SetProperty(ref _missionChosen, value))
				{
					//can decide what to do when property changes
				}

			}
		}
	}
}
