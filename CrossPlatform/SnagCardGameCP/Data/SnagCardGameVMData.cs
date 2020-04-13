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
using SnagCardGameCP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using SnagCardGameCP.Logic;
//i think this is the most common things i like to do
namespace SnagCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class SnagCardGameVMData : ObservableObject, ITrickCardGamesData<SnagCardGameCardInformation, EnumSuitList>
    {

		public SnagCardGameVMData(IEventAggregator aggregator,
			CommandContainer command,
			SnagTrickObservable trickArea1,
			SnagCardGameGameContainer gameContainer
			)
		{
			Deck1 = new DeckObservablePile<SnagCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<SnagCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<SnagCardGameCardInformation>(command);
			TrickArea1 = trickArea1;
			Bar1 = new BarObservable(gameContainer);
			Human1 = new HandObservable<SnagCardGameCardInformation>(command);
			Opponent1 = new HandObservable<SnagCardGameCardInformation>(command);
			Bar1.Visible = true;
			Bar1.AutoSelect = HandObservable<SnagCardGameCardInformation>.EnumAutoType.SelectOneOnly;
			Human1.Text = "Your Cards Won";
			Opponent1.Text = "Opponent Cards Won";
			Human1.Visible = false;
			Opponent1.Visible = false;
		}

		BasicTrickAreaObservable<EnumSuitList, SnagCardGameCardInformation> ITrickCardGamesData<SnagCardGameCardInformation, EnumSuitList>.TrickArea1
		{
			get => TrickArea1;
			set => TrickArea1 = (SnagTrickObservable) value;
		}
		public BarObservable Bar1;
		public HandObservable<SnagCardGameCardInformation> Human1;
		public HandObservable<SnagCardGameCardInformation> Opponent1;

		public SnagTrickObservable TrickArea1 { get; set; }
		//public BasicTrickAreaObservable<EnumSuitList, SnagCardGameCardInformation> TrickArea1 { get; set; }
		public DeckObservablePile<SnagCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<SnagCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<SnagCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<SnagCardGameCardInformation>? OtherPile { get; set; }
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
