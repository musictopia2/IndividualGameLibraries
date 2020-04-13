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
using HuseHeartsCP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using HuseHeartsCP.Logic;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
//i think this is the most common things i like to do
namespace HuseHeartsCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class HuseHeartsVMData : ObservableObject, ITrickCardGamesData<HuseHeartsCardInformation, EnumSuitList>,
		ITrickDummyHand<EnumSuitList, HuseHeartsCardInformation>
	{
		private readonly HuseHeartsGameContainer _gameContainer;
		public HuseHeartsVMData(IEventAggregator aggregator,
			CommandContainer command,
			HuseHeartsTrickAreaCP trickArea1,
			HuseHeartsGameContainer gameContainer
			)
		{
			Deck1 = new DeckObservablePile<HuseHeartsCardInformation>(aggregator, command);
			Pile1 = new PileObservable<HuseHeartsCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<HuseHeartsCardInformation>(command);
			TrickArea1 = trickArea1;
			_gameContainer = gameContainer;
			Dummy1 = new HandObservable<HuseHeartsCardInformation>(command);
			Blind1 = new HandObservable<HuseHeartsCardInformation>(command);
			Blind1.Maximum = 4;
			Blind1.Text = "Blind";
			Dummy1.Text = "Dummy Hand";
			Dummy1.AutoSelect = HandObservable<HuseHeartsCardInformation>.EnumAutoType.SelectOneOnly;

		}
		public HandObservable<HuseHeartsCardInformation> Dummy1;
		public HandObservable<HuseHeartsCardInformation> Blind1;
		public HuseHeartsTrickAreaCP TrickArea1 { get; set; }
		BasicTrickAreaObservable<EnumSuitList, HuseHeartsCardInformation> ITrickCardGamesData<HuseHeartsCardInformation, EnumSuitList>.TrickArea1
		{
			get => TrickArea1;
			set => TrickArea1 = (HuseHeartsTrickAreaCP)value;
		}

		public DeckObservablePile<HuseHeartsCardInformation> Deck1 { get; set; }
		public PileObservable<HuseHeartsCardInformation> Pile1 { get; set; }
		public HandObservable<HuseHeartsCardInformation> PlayerHand1 { get; set; }
		public PileObservable<HuseHeartsCardInformation>? OtherPile { get; set; }
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
		private int _roundNumber;

		[VM]
		public int RoundNumber
		{
			get { return _roundNumber; }
			set
			{
				if (SetProperty(ref _roundNumber, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		private EnumStatus _gameStatus;
		[VM]
		public EnumStatus GameStatus
		{
			get { return _gameStatus; }
			set
			{
				if (SetProperty(ref _gameStatus, value))
				{
					//can decide what to do when property changes
				}

			}
		}



		public DeckObservableDict<HuseHeartsCardInformation> GetCurrentHandList()
		{
			if (TrickArea1!.FromDummy == true)
				return Dummy1!.HandList;
			else
				return _gameContainer!.SingleInfo!.MainHandList;
		}

		public int CardSelected()
		{
			if (TrickArea1!.FromDummy == true)
				return Dummy1!.ObjectSelected();
			else if (_gameContainer!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
				throw new BasicBlankException("Only self can show card selected.  If I am wrong, rethink");
			return PlayerHand1!.ObjectSelected();
		}
		public void RemoveCard(int deck)
		{
			if (TrickArea1!.FromDummy == true)
				Dummy1!.HandList.RemoveObjectByDeck(deck);
			else
				_gameContainer.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //because computer player does this too.
		}



		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
