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
using MillebournesCP.Cards;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.Misc;
//i think this is the most common things i like to do
namespace MillebournesCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class MillebournesVMData : ObservableObject, IBasicCardGamesData<MillebournesCardInformation>
    {

		public MillebournesVMData(IEventAggregator aggregator, CommandContainer command)
		{
			Deck1 = new DeckObservablePile<MillebournesCardInformation>(aggregator, command);
			Pile1 = new PileObservable<MillebournesCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<MillebournesCardInformation>(command);
			OtherPile = Pile1;
			
			Pile2 = new PileObservable<MillebournesCardInformation>(aggregator, command);

			Stops = new CustomStopWatchCP();
			Stops.MaxTime = 3000;
		}
		public CustomStopWatchCP Stops;
		public DeckObservablePile<MillebournesCardInformation> Deck1 { get; set; }
		public PileObservable<MillebournesCardInformation> Pile1 { get; set; }
		public PileObservable<MillebournesCardInformation> Pile2 { get; set; }
		public HandObservable<MillebournesCardInformation> PlayerHand1 { get; set; }
		public PileObservable<MillebournesCardInformation>? OtherPile { get; set; }
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

		public bool CoupeVisible { get; set; }

	}
}
