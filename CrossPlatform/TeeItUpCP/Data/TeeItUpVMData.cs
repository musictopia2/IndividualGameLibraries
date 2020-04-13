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
using TeeItUpCP.Cards;
//i think this is the most common things i like to do
namespace TeeItUpCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class TeeItUpVMData : ObservableObject, IBasicCardGamesData<TeeItUpCardInformation>
    {

		public TeeItUpVMData(IEventAggregator aggregator, CommandContainer command)
		{
			Deck1 = new DeckObservablePile<TeeItUpCardInformation>(aggregator, command);
			Pile1 = new PileObservable<TeeItUpCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<TeeItUpCardInformation>(command);
			OtherPile = new PileObservable<TeeItUpCardInformation>(aggregator, command);
			OtherPile.CurrentOnly = true;
			OtherPile.Text = "Current Card";
			OtherPile.FirstLoad(new TeeItUpCardInformation());
			PlayerHand1.Visible = false; //try this too.
		}
		public DeckObservablePile<TeeItUpCardInformation> Deck1 { get; set; }
		public PileObservable<TeeItUpCardInformation> Pile1 { get; set; }
		public HandObservable<TeeItUpCardInformation> PlayerHand1 { get; set; }
		public PileObservable<TeeItUpCardInformation>? OtherPile { get; set; }
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


		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
