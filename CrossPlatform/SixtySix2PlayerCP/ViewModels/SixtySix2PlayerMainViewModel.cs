using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.DrawableListsObservable;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using SixtySix2PlayerCP.Cards;
using SixtySix2PlayerCP.Data;
using SixtySix2PlayerCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SixtySix2PlayerCP.ViewModels
{
    [InstanceGame]
    public class SixtySix2PlayerMainViewModel : TrickCardGamesVM<SixtySix2PlayerCardInformation, EnumSuitList>
    {
        private readonly SixtySix2PlayerMainGameClass _mainGame; //if we don't need, delete.
        private readonly SixtySix2PlayerVMData _model;

        public SixtySix2PlayerMainViewModel(CommandContainer commandContainer,
            SixtySix2PlayerMainGameClass mainGame,
            SixtySix2PlayerVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            commandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        }
        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
            return base.TryCloseAsync();
        }
        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return; //for now, has to be this way.
            if (CanAnnounceMarriage)
                _model.PlayerHand1!.AutoSelect = HandObservable<SixtySix2PlayerCardInformation>.EnumAutoType.SelectAsMany;
            else
                _model.PlayerHand1!.AutoSelect = HandObservable<SixtySix2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
        }
        public bool CanAnnounceMarriage => _model!.TrickArea1!.IsLead && _mainGame.SaveRoot!.CardsForMarriage.Count == 0;
        [Command(EnumCommandCategory.Game)]
        public async Task AnnouceMarriageAsync()
        {
            int howMany = _model.PlayerHand1.HowManySelectedObjects;
            if (howMany != 2)
            {
                await UIPlatform.ShowMessageAsync("Must choose 2 cards");
                return;
            }
            var thisList = _model.PlayerHand1.ListSelectedObjects();
            var thisMarriage = _mainGame!.WhichMarriage(thisList);
            if (thisMarriage == EnumMarriage.None)
            {
                await UIPlatform.ShowMessageAsync("This is not a valid marrige");
                return;
            }
            if (_mainGame.CanShowMarriage(thisMarriage) == false)
            {
                await UIPlatform.ShowMessageAsync("Cannot show marriage because the points will put you over 66 points.");
                return;
            }
            var tempList = thisList.GetDeckListFromObjectList();
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("announcemarriage", tempList);
            await _mainGame.AnnounceMarriageAsync(tempList);
        }
        public bool CanGoOut()
        {
            if (_mainGame!.CanAnnounceMarriageAtBeginning == true || _model.TrickArea1!.IsLead && _mainGame.SaveRoot!.CardsForMarriage.Count == 0)
                return _model.TrickArea1!.IsLead;
            return false;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task GoOutAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("goout");
            await _mainGame!.GoOutAsync();
        }

        private int _bonusPoints;
        [VM]
        public int BonusPoints
        {
            get { return _bonusPoints; }
            set
            {
                if (SetProperty(ref _bonusPoints, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return _mainGame!.CanExchangeForDiscard();
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            if (_mainGame!.CanExchangeForDiscard() == false)
                throw new BasicBlankException("Should have been disabled because cannot exchange for discard");
            int howMany = _model.PlayerHand1!.HowManySelectedObjects;
            if (howMany == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose a card to exchange");
                return;
            }
            if (howMany > 1)
            {
                await UIPlatform.ShowMessageAsync("Cannot choose more than one card to exchange");
                return;
            }
            int decks = _model.PlayerHand1!.ObjectSelected();
            var thisCard = _mainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
            if (thisCard.Value > EnumCardValueList.Nine)
            {
                await UIPlatform.ShowMessageAsync("Must choose a nine to exchange");
                return;
            }
            if (thisCard.Suit != _mainGame.SaveRoot!.TrumpSuit)
            {
                await UIPlatform.ShowMessageAsync("Must choose the nine of the trump suit in order to exchange");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("exchangediscard", thisCard.Deck);
            await _mainGame!.ExchangeDiscardAsync(thisCard.Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
    }
}