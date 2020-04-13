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
using SkuckCardGameCP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
//i think this is the most common things i like to do
namespace SkuckCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class SkuckCardGameVMData : ObservableObject, ITrickCardGamesData<SkuckCardGameCardInformation, EnumSuitList>
		, ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>
	{
		private readonly SkuckCardGameGameContainer _gameContainer;
		public SkuckCardGameVMData(IEventAggregator aggregator,
			CommandContainer command,
			BasicTrickAreaObservable<EnumSuitList, SkuckCardGameCardInformation> trickArea1,
			IGamePackageResolver resolver,
			SkuckCardGameGameContainer gameContainer
			)
		{
			Deck1 = new DeckObservablePile<SkuckCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<SkuckCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<SkuckCardGameCardInformation>(command);
			TrickArea1 = trickArea1;
			_gameContainer = gameContainer;
			Bid1 = new NumberPicker(command, resolver);
			Suit1 = new SimpleEnumPickerVM<EnumSuitList, DeckPieceCP>(command, new SuitListChooser());
			//iffy part is the trickarea visible.
			Suit1.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect;
			Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
			Suit1.ItemSelectionChanged += Suit1_ItemSelectionChanged;
			Bid1.LoadNormalNumberRangeValues(1, 26);
		}
		public NumberPicker Bid1;
		public SimpleEnumPickerVM<EnumSuitList, DeckPieceCP> Suit1;
		public BasicTrickAreaObservable<EnumSuitList, SkuckCardGameCardInformation> TrickArea1 { get; set; }
		public DeckObservablePile<SkuckCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<SkuckCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<SkuckCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<SkuckCardGameCardInformation>? OtherPile { get; set; }
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
		private EnumStatusList _gameStatus;

		[VM]
		public EnumStatusList GameStatus
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
		public int BidAmount { get; set; } = -1;



		DeckObservableDict<SkuckCardGameCardInformation> ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.GetCurrentHandList()
		{
			DeckObservableDict<SkuckCardGameCardInformation> output = _gameContainer!.SingleInfo!.MainHandList.ToObservableDeckDict();
			output.AddRange(_gameContainer.SingleInfo.TempHand!.ValidCardList);
			return output; //hopefully this simple.
		}
		int ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.CardSelected()
		{
			if (_gameContainer!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
				throw new BasicBlankException("Only self can get card selected.  If I am wrong, rethink");
			int selects = PlayerHand1!.ObjectSelected();
			int others = _gameContainer.SingleInfo.TempHand!.CardSelected;
			if (selects != 0 && others != 0)
				throw new BasicBlankException("You cannot choose from both hand and temps.  Rethink");
			if (selects != 0)
				return selects;
			return others;
		}
		void ITrickDummyHand<EnumSuitList, SkuckCardGameCardInformation>.RemoveCard(int deck)
		{
			bool rets = _gameContainer!.SingleInfo!.MainHandList.ObjectExist(deck);
			if (rets == true)
			{
				_gameContainer.SingleInfo.MainHandList.RemoveObjectByDeck(deck);
				return;
			}
			var thisCard = _gameContainer.SingleInfo.TempHand!.CardList.GetSpecificItem(deck);
			if (thisCard.IsEnabled == false)
				throw new BasicBlankException("Card was supposed to be disabled");
			_gameContainer.SingleInfo.TempHand.HideCard(thisCard);
		}
		private Task Bid1_ChangedNumberValueAsync(int chosen)
		{
			BidAmount = chosen;
			return Task.CompletedTask;
		}
		private void Suit1_ItemSelectionChanged(EnumSuitList? piece)
		{
			if (piece.HasValue == false)
				_gameContainer!.SaveRoot!.TrumpSuit = EnumSuitList.None;
			else
				_gameContainer!.SaveRoot!.TrumpSuit = piece!.Value;
		}

	}
}
