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
using BladesOfSteelCP.Data;
using BladesOfSteelCP.Logic;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace BladesOfSteelCP.ViewModels
{
    [InstanceGame]
    public class BladesOfSteelMainViewModel : BasicCardGamesVM<RegularSimpleCard>
    {
        private readonly BladesOfSteelMainGameClass _mainGame; //if we don't need, delete.
        private readonly BladesOfSteelVMData _model;
        private readonly BladesOfSteelGameContainer _gameContainer;
        public BladesOfSteelMainViewModel(CommandContainer commandContainer,
            BladesOfSteelMainGameClass mainGame,
            BladesOfSteelVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            BladesOfSteelGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _gameContainer = gameContainer;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = false;
            _model.PlayerHand1.Maximum = 6;
            _model.PlayerHand1.AutoSelect = HandObservable<RegularSimpleCard>.EnumAutoType.SelectAsMany;
            _model.MainDefense1.SendEnableProcesses(this, () => _mainGame.SaveRoot.PlayOrder.OtherTurn > 0);
            _model.YourAttackPile.SendEnableProcesses(this, () => CanEnablePiles(false));
            _model.YourDefensePile.SendEnableProcesses(this, () => CanEnablePiles(true));
            LoadAttackCommands();
        }

        protected override Task ActivateAsync()
        {

            return base.ActivateAsync();
        }
        //something could be iffy here (?)

        private bool CanEnablePiles(bool isDefense)
        {
            if (_mainGame!.SaveRoot!.PlayOrder.OtherTurn > 0)
            {
                return isDefense;
            }
            return !_model.MainDefense1!.HasCards;
        }

        protected override Task TryCloseAsync()
        {
            CloseAttackCommands();

            return base.TryCloseAsync();
        }
        private void LoadAttackCommands()
        {
            _model.MainDefense1.BoardClickedAsync += MainDefense1_BoardClickedAsync;
            _model.YourAttackPile.BoardClickedAsync += YourAttackPile_BoardClickedAsync;
            _model.YourDefensePile.BoardClickedAsync += YourDefensePile_BoardClickedAsync;
            _model.YourDefensePile.ObjectClickedAsync += YourDefensePile_ObjectClickedAsync;
        }
        private void CloseAttackCommands()
        {
            _model.MainDefense1.BoardClickedAsync -= MainDefense1_BoardClickedAsync;
            _model.YourAttackPile.BoardClickedAsync -= YourAttackPile_BoardClickedAsync;
            _model.YourDefensePile.BoardClickedAsync -= YourDefensePile_BoardClickedAsync;
            _model.YourDefensePile.ObjectClickedAsync -= YourDefensePile_ObjectClickedAsync;
        }

        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return !_model.MainDefense1!.HasCards;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            if (_mainGame!.SingleInfo!.MainHandList.Count == 0 && _mainGame.SingleInfo.DefenseList.Count == 0)
                throw new BasicBlankException("There are no cards from hand or defense list.  Therfore; should have disabled the pile");
            if (_mainGame.SingleInfo.DefensePile!.HandList.HasSelectedObject() == true)
            {
                if (_mainGame.SingleInfo.DefenseList.All(items => items.IsSelected == false))
                {
                    await UIPlatform.ShowMessageAsync("If you choose one card from defense, you must choose all cards");
                    return;
                }
                if (_mainGame.BasicData!.MultiPlayer == true)
                    await _mainGame.Network!.SendAllAsync("throwawaydefense");
                await _mainGame!.ThrowAwayDefenseCardsAsync();
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("throwawayallcardsfromhand");
            await _mainGame!.ThrowAwayAllCardsFromHandAsync();
        }
        public override bool CanEnableAlways()
        {
            return true;
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
        public override bool CanEndTurn()
        {
            return _model.MainDefense1!.HasCards;
        }

        private async Task MainDefense1_BoardClickedAsync()
        {
            if (_gameContainer!.OtherTurn == 0)
                throw new BasicBlankException("Should not have allowed the click for main defense because not the other turn to defend themselves");
            if (_mainGame!.SingleInfo!.MainHandList.HasSelectedObject() == false && _mainGame.SingleInfo.DefenseList.HasSelectedObject() == false)
            {
                await UIPlatform.ShowMessageAsync("Must choose at least a card from defense or at least a card from hand");
                return;
            }
            if (_mainGame.SingleInfo.MainHandList.HasSelectedObject() && _mainGame.SingleInfo.DefenseList.HasSelectedObject())
            {
                await UIPlatform.ShowMessageAsync("Cannot choose cards from both from hand and from defense piles");
                return;
            }
            bool fromHand;
            DeckRegularDict<RegularSimpleCard> tempDefenseList;
            if (_mainGame.SingleInfo.MainHandList.HasSelectedObject())
            {
                fromHand = true;
                tempDefenseList = _mainGame.SingleInfo.MainHandList.GetSelectedItems();
            }
            else
            {
                fromHand = false;
                tempDefenseList = _mainGame.SingleInfo.DefenseList.GetSelectedItems();
                if (tempDefenseList.Any(items => items.Color == EnumColorList.Red))
                    throw new BasicBlankException("No red attack cards should have even been put to defense pile");
            }
            if (await _mainGame.CanPlayDefenseCardsAsync(tempDefenseList) == false)
                return;
            if (_model.MainDefense1!.CanAddDefenseCards(tempDefenseList) == false)
            {
                await UIPlatform.ShowMessageAsync("This defense is not enough to prevent a goal from the attacker");
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                var deckList = tempDefenseList.GetDeckListFromObjectList();
                if (fromHand == true)
                    await _mainGame.Network!.SendAllAsync("defensehand", deckList);
                else
                    await _mainGame.Network!.SendAllAsync("defenseboard", deckList);
            }
            tempDefenseList.UnselectAllObjects();
            await _mainGame.PlayDefenseCardsAsync(tempDefenseList, fromHand);
        }
        private Task YourDefensePile_ObjectClickedAsync(RegularSimpleCard thisObject, int index)
        {
            if (_mainGame!.SingleInfo!.MainHandList.HasSelectedObject() == true)
                return Task.CompletedTask;
            thisObject.IsSelected = !thisObject.IsSelected;
            return Task.CompletedTask;
        }
        private async Task YourDefensePile_BoardClickedAsync()
        {
            if (_gameContainer!.OtherTurn > 0)
                return;
            if (_mainGame!.SingleInfo!.MainHandList.HasSelectedObject() == false)
                return;
            if (_mainGame.SingleInfo.DefenseList.HasSelectedObject() == true)
            {
                await UIPlatform.ShowMessageAsync("Cannot choose any defense cards on the board to add more defense cards");
                return;
            }
            var tempDefenseList = _mainGame.SingleInfo.MainHandList.GetSelectedItems();
            if (await _mainGame.CanPlayDefenseCardsAsync(tempDefenseList) == false)
            {
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("addtodefense", tempDefenseList.GetDeckListFromObjectList());
            tempDefenseList.UnselectAllObjects();
            await _mainGame.AddCardsToDefensePileAsync(tempDefenseList); //hopefully this is correct.
        }
        private async Task YourAttackPile_BoardClickedAsync()
        {
            var tempAttackList = _mainGame!.SingleInfo!.MainHandList.GetSelectedItems();
            if (await _mainGame.CanPlayAttackCardsAsync(tempAttackList) == false)
                return;
            tempAttackList.UnselectAllObjects();
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("attack", tempAttackList.GetDeckListFromObjectList());
            await _mainGame.PlayAttackCardsAsync(tempAttackList);
        }
        public bool CanPass => _mainGame!.SaveRoot!.PlayOrder.OtherTurn > 0;
        [Command(EnumCommandCategory.Game)]
        public async Task PassAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("passdefense");
            await _mainGame!.PassDefenseAsync();
        }

    }
}