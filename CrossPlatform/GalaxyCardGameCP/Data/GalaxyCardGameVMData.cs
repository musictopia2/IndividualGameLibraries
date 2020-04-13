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
using GalaxyCardGameCP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using GalaxyCardGameCP.Logic;
//i think this is the most common things i like to do
namespace GalaxyCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class GalaxyCardGameVMData : ObservableObject, ITrickCardGamesData<GalaxyCardGameCardInformation, EnumSuitList>
    {

		public GalaxyCardGameVMData(IEventAggregator aggregator,
			CommandContainer command,
			BasicTrickAreaObservable<EnumSuitList, GalaxyCardGameCardInformation> trickArea1
			)
		{
			Deck1 = new DeckObservablePile<GalaxyCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<GalaxyCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<GalaxyCardGameCardInformation>(command);
			PlayerHand1.Maximum = 9;
			TrickArea1 = trickArea1;
		}
		public INewWinCard? WinUI; //do the views can implement and call it.
		public BasicTrickAreaObservable<EnumSuitList, GalaxyCardGameCardInformation> TrickArea1 { get; set; }
		public DeckObservablePile<GalaxyCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<GalaxyCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<GalaxyCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<GalaxyCardGameCardInformation>? OtherPile { get; set; }
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
		private EnumSuitList _trumpSuit;
		[VM]
		public EnumSuitList TrumpSuit
		{
			get { return _trumpSuit; }
			set
			{
				if (SetProperty(ref _trumpSuit, value)) { }
			}
		}



		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
