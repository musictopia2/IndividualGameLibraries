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
using GolfCardGameCP.CustomPiles;
//i think this is the most common things i like to do
namespace GolfCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class GolfCardGameVMData : ObservableObject, IBasicCardGamesData<RegularSimpleCard>
    {

		public GolfCardGameVMData(IEventAggregator aggregator, CommandContainer command, GolfCardGameGameContainer gameContainer)
		{
			Deck1 = new DeckObservablePile<RegularSimpleCard>(aggregator, command);
			Pile1 = new PileObservable<RegularSimpleCard>(aggregator, command);
			PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
			OtherPile = new PileObservable<RegularSimpleCard>(aggregator, command);
			OtherPile.CurrentOnly = true;
			OtherPile.Text = "Current";
			HiddenCards1 = new HiddenCards(gameContainer);
			Beginnings1 = new Beginnings(command);
			GolfHand1 = new GolfHand(gameContainer);
		}

		public HiddenCards HiddenCards1;
		public Beginnings Beginnings1;
		public GolfHand GolfHand1;

		public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
		public PileObservable<RegularSimpleCard> Pile1 { get; set; }
		public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
		public PileObservable<RegularSimpleCard>? OtherPile { get; set; }
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

		private int _round;
		[VM]
		public int Round
		{
			get { return _round; }
			set
			{
				if (SetProperty(ref _round, value))
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

	}
}
