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
using SkipboCP.Cards;
using SkipboCP.Piles;
using BasicGameFrameworkLibrary.SpecializedGameTypes.StockClasses;
using static SkipboCP.Data.GlobalConstants;
using BasicGameFrameworkLibrary.BasicEventModels;

//i think this is the most common things i like to do
namespace SkipboCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class SkipboVMData : ObservableObject, IBasicCardGamesData<SkipboCardInformation>
    {

		public SkipboVMData(IEventAggregator aggregator, CommandContainer command)
		{
			Deck1 = new DeckObservablePile<SkipboCardInformation>(aggregator, command);
			Pile1 = new PileObservable<SkipboCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<SkipboCardInformation>(command);
			StockPile = new StockViewModel(command, aggregator);
			//DiscardPiles = new DiscardPilesVM<SkipboCardInformation>(command, aggregator);
			//DiscardPiles.Init(HowManyDiscards); //hopefully this simple (?)
			PublicPiles = new PublicPilesViewModel(command, aggregator);
		}
		//i think this should hold the necessary stuff needed for piles.
		public StockViewModel StockPile;
		public DiscardPilesVM<SkipboCardInformation>? DiscardPiles;
		public PublicPilesViewModel PublicPiles; //has to create this too.
		//i think it should be in the vmdata since its more ui related stuff anyways.

		public DeckObservablePile<SkipboCardInformation> Deck1 { get; set; }
		public PileObservable<SkipboCardInformation> Pile1 { get; set; }
		public HandObservable<SkipboCardInformation> PlayerHand1 { get; set; }
		public PileObservable<SkipboCardInformation>? OtherPile { get; set; }
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

		private int _cardsToShuffle;

		[VM]
		public int CardsToShuffle
		{
			get { return _cardsToShuffle; }
			set
			{
				if (SetProperty(ref _cardsToShuffle, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.


		


	}
}
