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
using SorryCardGameCP.Cards;
//i think this is the most common things i like to do
namespace SorryCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class SorryCardGameVMData : ObservableObject, IBasicCardGamesData<SorryCardGameCardInformation>
    {

		public SorryCardGameVMData(IEventAggregator aggregator, CommandContainer command)
		{
			Deck1 = new DeckObservablePile<SorryCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<SorryCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<SorryCardGameCardInformation>(command);
			OtherPile = new PileObservable<SorryCardGameCardInformation>(aggregator, command);
			OtherPile.Text = "Play Pile";
			OtherPile.FirstLoad(new SorryCardGameCardInformation());
		}
		public DeckObservablePile<SorryCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<SorryCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<SorryCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<SorryCardGameCardInformation>? OtherPile { get; set; }
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
