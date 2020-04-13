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
using MonopolyCardGameCP.Cards;
using MonopolyCardGameCP.ViewModels;
namespace MonopolyCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class MonopolyCardGameVMData : ObservableObject, IBasicCardGamesData<MonopolyCardGameCardInformation>
    {

		public MonopolyCardGameVMData(IEventAggregator aggregator, CommandContainer command)
		{
			Deck1 = new DeckObservablePile<MonopolyCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<MonopolyCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<MonopolyCardGameCardInformation>(command);
			AdditionalInfo1 = new DetailCardViewModel();

		}
		public DetailCardViewModel AdditionalInfo1;

		public DeckObservablePile<MonopolyCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<MonopolyCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<MonopolyCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<MonopolyCardGameCardInformation>? OtherPile { get; set; }
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

	}
}
