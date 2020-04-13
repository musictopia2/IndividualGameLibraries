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
using RageCardGameCP.Cards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using RageCardGameCP.Logic;
using BasicGameFrameworkLibrary.ChooserClasses;
using BasicGameFrameworkLibrary.GameGraphicsCP.GamePieces;
using System.Drawing;
using BasicGameFrameworkLibrary.DIContainers;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
//i think this is the most common things i like to do
namespace RageCardGameCP.Data
{
    [SingletonGame]
	[AutoReset] //usually needs autoreset
    public class RageCardGameVMData : ObservableObject, ITrickCardGamesData<RageCardGameCardInformation, EnumColor>
    {
		private readonly RageCardGameGameContainer _gameContainer;

		public RageCardGameVMData(IEventAggregator aggregator,
			CommandContainer command,
			SpecificTrickAreaObservable trickArea1,
			IGamePackageResolver resolver,
			RageCardGameGameContainer gameContainer
			)
		{
			Deck1 = new DeckObservablePile<RageCardGameCardInformation>(aggregator, command);
			Pile1 = new PileObservable<RageCardGameCardInformation>(aggregator, command);
			PlayerHand1 = new HandObservable<RageCardGameCardInformation>(command);
			PlayerHand1.Maximum = 11;
			TrickArea1 = trickArea1;
			_gameContainer = gameContainer;
			Color1 = new SimpleEnumPickerVM<EnumColor, CheckerChoiceCP<EnumColor>>(command, new ColorListChooser<EnumColor>());
			Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
			Color1.ItemClickedAsync += Color1_ItemClickedAsync;
			Bid1 = new NumberPicker(command, resolver);
			Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;

		}

		private async Task Color1_ItemClickedAsync(EnumColor piece)
		{
			if (piece == _gameContainer!.SaveRoot!.TrumpSuit && _gameContainer!.SaveRoot!.TrickList.Last().SpecialType == EnumSpecialType.Change)
			{
				await UIPlatform.ShowMessageAsync($"{piece} is already current trump.  Choose a different one");
				return;
			}
			ColorChosen = piece;
			if (_gameContainer.ColorChosenAsync == null)
			{
				throw new BasicBlankException("Nobody is handling the color chosen.  Rethink");
			}
			await _gameContainer!.ColorChosenAsync!.Invoke();
		}
		private Task Bid1_ChangedNumberValueAsync(int chosen)
		{
			BidAmount = chosen;
			return Task.CompletedTask;
		}
		BasicTrickAreaObservable<EnumColor, RageCardGameCardInformation> ITrickCardGamesData<RageCardGameCardInformation, EnumColor>.TrickArea1
		{
			get => TrickArea1;
			set => TrickArea1 = (SpecificTrickAreaObservable)value;
		}
		public SimpleEnumPickerVM<EnumColor, CheckerChoiceCP<EnumColor>> Color1;
		public NumberPicker Bid1;
		public int BidAmount { get; set; } = -1;
		public SpecificTrickAreaObservable TrickArea1 { get; set; }
		public DeckObservablePile<RageCardGameCardInformation> Deck1 { get; set; }
		public PileObservable<RageCardGameCardInformation> Pile1 { get; set; }
		public HandObservable<RageCardGameCardInformation> PlayerHand1 { get; set; }
		public PileObservable<RageCardGameCardInformation>? OtherPile { get; set; }
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
		private EnumColor _trumpSuit;
		[VM]
		public EnumColor TrumpSuit
		{
			get { return _trumpSuit; }
			set
			{
				if (SetProperty(ref _trumpSuit, value)) { }
			}
		}

		private string _lead = "";

		[VM]
		public string Lead
		{
			get { return _lead; }
			set
			{
				if (SetProperty(ref _lead, value))
				{
					//can decide what to do when property changes
				}

			}
		}

		public EnumColor ColorChosen { get; set; }

		//any other ui related properties will be here.
		//can copy/paste for the actual view model.

	}
}
