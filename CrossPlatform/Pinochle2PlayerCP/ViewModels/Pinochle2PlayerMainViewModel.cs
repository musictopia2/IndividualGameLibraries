using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
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
using Pinochle2PlayerCP.Cards;
using Pinochle2PlayerCP.Data;
using Pinochle2PlayerCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace Pinochle2PlayerCP.ViewModels
{
    [InstanceGame]
    public class Pinochle2PlayerMainViewModel : TrickCardGamesVM<Pinochle2PlayerCardInformation, EnumSuitList>
    {
        private readonly Pinochle2PlayerMainGameClass _mainGame; //if we don't need, delete.
        private readonly Pinochle2PlayerVMData _model;

        public Pinochle2PlayerMainViewModel(CommandContainer commandContainer,
            Pinochle2PlayerMainGameClass mainGame,
            Pinochle2PlayerVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            var player = _mainGame.PlayerList.GetSelf();
            _mainGame.Aggregator.Subscribe(player);
            _model.TempSets.Init(this);
            _model.TempSets.ClearBoard(); //try this too.
            _model.TempSets.SetClickedAsync += TempSets_SetClickedAsync;
            _model.OpponentMelds.SendEnableProcesses(this, () => false);
            commandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
            _model.YourMelds.SendEnableProcesses(this, () =>
            {
                if (_mainGame!.SaveRoot!.ChooseToMeld)
                {
                    _model.YourMelds.AutoSelect = HandObservable<Pinochle2PlayerCardInformation>.EnumAutoType.SelectAsMany;
                    return true;
                }
                _model.YourMelds.AutoSelect = HandObservable<Pinochle2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
                return _model.Pile1!.PileEmpty() == false && _mainGame.SaveRoot.MeldList.Any(items => items.Player == _mainGame.WhoTurn && items.CardList.Count > 0);
            });
        }

        private void CommandContainer_ExecutingChanged()
        {
            if (CommandContainer!.IsExecuting)
                return; //for now, has to be this way.
            if (_mainGame!.SaveRoot!.ChooseToMeld)
                _model.PlayerHand1!.AutoSelect = HandObservable<Pinochle2PlayerCardInformation>.EnumAutoType.SelectAsMany;
            else
                _model.PlayerHand1!.AutoSelect = HandObservable<Pinochle2PlayerCardInformation>.EnumAutoType.SelectOneOnly;
        }

        //anything else needed is here.


        protected override Task TryCloseAsync()
        {
            CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
            _model.TempSets.SetClickedAsync -= TempSets_SetClickedAsync;
            return base.TryCloseAsync();
        }
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            if (_mainGame!.SaveRoot!.ChooseToMeld == false)
                return false;
            return _model.Pile1!.GetCardInfo().Value != EnumCardValueList.Nine && _mainGame.SingleInfo!.MainHandList.Any(items => items.Value == EnumCardValueList.Nine && items.Suit == _mainGame.SaveRoot.TrumpSuit);

        }

        public override bool CanEndTurn()
        {
            if (_model.PlayerHand1!.HasSelectedObject() || _model.YourMelds!.HasSelectedObject() || _model.TempSets!.HasSelectedObject)
                return false;
            return _mainGame!.SaveRoot!.ChooseToMeld;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int fromTemps = _model.TempSets!.HowManySelectedObjects;
            int hands = _model.PlayerHand1!.HowManySelectedObjects;
            if (hands == 0 && fromTemps == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose a card from hand in order to exchange for the top card");
                return;
            }
            if (hands + fromTemps > 1)
            {
                await UIPlatform.ShowMessageAsync("Must choose only one card from hand to exchange");
                return;
            }
            Pinochle2PlayerCardInformation thisCard;
            if (fromTemps > 0)
            {
                thisCard = _model.TempSets.GetSelectedObject;
            }
            else
            {
                thisCard = _mainGame!.SingleInfo!.MainHandList.GetSpecificItem(_model.PlayerHand1.ObjectSelected());
            }

            if (thisCard.Value > EnumCardValueList.Nine)
            {
                await UIPlatform.ShowMessageAsync("Must choose a nine to exchange");
                return;
            }
            if (thisCard.Suit != _mainGame!.SaveRoot!.TrumpSuit)
            {
                await UIPlatform.ShowMessageAsync("Must choose the nine of the trump suit in order to exchange");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("exchangediscard", thisCard.Deck);
            await _mainGame.ExchangeDiscardAsync(thisCard.Deck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private bool _isProcessing;
        private Task TempSets_SetClickedAsync(int index)
        {
            if (_isProcessing == true)
                return Task.CompletedTask;
            _isProcessing = true;
            var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
            _model.TempSets!.AddCards(index, tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
        public bool CanMeld => _mainGame!.SaveRoot!.ChooseToMeld;
        [Command(EnumCommandCategory.Game)]
        public async Task MeldAsync()
        {
            if (_model.PlayerHand1!.HasSelectedObject() == false && _model.TempSets.HowManySelectedObjects == 0)
            {
                await UIPlatform.ShowMessageAsync("Must choose at least one card from hand in order to meld");
                return;
            }
            var completeList = _model.PlayerHand1.ListSelectedObjects();
            var temps = _model.TempSets.ListSelectedObjects();
            completeList.AddRange(temps);
            DeckRegularDict<Pinochle2PlayerCardInformation> otherList = new DeckRegularDict<Pinochle2PlayerCardInformation>();
            if (_model.YourMelds!.HasSelectedObject())
            {
                otherList = _model.YourMelds.ListSelectedObjects();
                completeList.AddRange(otherList);
            }
            var thisMeld = _mainGame!.GetMeldFromList(completeList);
            if (thisMeld.ClassAValue == EnumClassA.None && thisMeld.ClassBValue == EnumClassB.None && thisMeld.ClassCValue == EnumClassC.None)
            {
                await UIPlatform.ShowMessageAsync("There is no meld combinations here");
                return;
            }
            if (_model.YourMelds.HasSelectedObject())
            {
                foreach (var thisCard in otherList)
                {
                    var tempMeld = _mainGame.GetMeldFromCard(thisCard);
                    if (tempMeld.ClassAValue == EnumClassA.Dix)
                        throw new BasicBlankException("Should have caught the problem with using dix earlier");
                    if (tempMeld.ClassBValue == thisMeld.ClassBValue && thisMeld.ClassBValue > EnumClassB.None)
                    {
                        await UIPlatform.ShowMessageAsync("Cannot reuse class b for class b");
                        return;
                    }
                    if (tempMeld.ClassCValue == thisMeld.ClassCValue && thisMeld.ClassCValue > EnumClassC.None)
                    {
                        await UIPlatform.ShowMessageAsync("Cannot reuse a pinochle for a pinochle");
                        return;
                    }
                    if (tempMeld.ClassAValue <= thisMeld.ClassAValue && thisMeld.ClassAValue > EnumClassA.None)
                    {
                        await UIPlatform.ShowMessageAsync("Cannot download class A to get more points or to create another of same value");
                        return;
                    }
                }
            }
            var deckList = completeList.GetDeckListFromObjectList();
            if (_mainGame.BasicData!.MultiPlayer)
                await _mainGame.Network!.SendAllAsync("meld", deckList);
            await _mainGame.MeldAsync(deckList);
        }

    }
}