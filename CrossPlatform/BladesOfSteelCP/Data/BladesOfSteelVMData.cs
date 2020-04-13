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
using BladesOfSteelCP.CustomPiles;
//i think this is the most common things i like to do
namespace BladesOfSteelCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class BladesOfSteelVMData : ObservableObject, IBasicCardGamesData<RegularSimpleCard>
    {

		public BladesOfSteelVMData(IEventAggregator aggregator, CommandContainer command, BladesOfSteelGameContainer gameContainer)
		{
			Deck1 = new DeckObservablePile<RegularSimpleCard>(aggregator, command);
			Pile1 = new PileObservable<RegularSimpleCard>(aggregator, command);
			PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
			YourFaceOffCard = new PileObservable<RegularSimpleCard>(aggregator, command);
			YourFaceOffCard.IsEnabled = false;
			YourFaceOffCard.Text = "Your";
			OpponentFaceOffCard = new PileObservable<RegularSimpleCard>(aggregator, command);
			OpponentFaceOffCard.IsEnabled = false;
			OpponentFaceOffCard.Text = "Opponent";
			MainDefense1 = new MainDefenseCP(gameContainer);
			YourAttackPile = new PlayerAttackCP(command);
			YourDefensePile = new PlayerDefenseCP(command);
			OpponentAttackPile = new PlayerAttackCP(command);
			OpponentDefensePile = new PlayerDefenseCP(command);
		}
		public PileObservable<RegularSimpleCard> YourFaceOffCard;
		public PileObservable<RegularSimpleCard> OpponentFaceOffCard;
		public MainDefenseCP MainDefense1;
		public PlayerAttackCP YourAttackPile;
		public PlayerDefenseCP YourDefensePile;
		public PlayerAttackCP OpponentAttackPile;
		public PlayerDefenseCP OpponentDefensePile;
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

		private string _otherPlayer = "";
		[VM]
		public string OtherPlayer
		{
			get { return _otherPlayer; }
			set
			{
				if (SetProperty(ref _otherPlayer, value))
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
