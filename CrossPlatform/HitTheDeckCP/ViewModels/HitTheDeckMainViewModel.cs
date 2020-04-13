using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using HitTheDeckCP.Cards;
using HitTheDeckCP.Data;
using HitTheDeckCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace HitTheDeckCP.ViewModels
{
    [InstanceGame]
    public class HitTheDeckMainViewModel : BasicCardGamesVM<HitTheDeckCardInformation>
    {
        private readonly HitTheDeckMainGameClass _mainGame; //if we don't need, delete.
        private readonly HitTheDeckVMData _model;
        private readonly HitTheDeckGameContainer _gameContainer; //if not needed, delete.

        public HitTheDeckMainViewModel(CommandContainer commandContainer,
            HitTheDeckMainGameClass mainGame,
            HitTheDeckVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            HitTheDeckGameContainer gameContainer
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _gameContainer = gameContainer;
            _model.Deck1.NeverAutoDisable = true;
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            if (NeedsSpecial == true)
                return false; //otherwise, can't compile.
            if (_mainGame!.SingleInfo!.MainHandList.Any(items => _mainGame.CanPlay(items.Deck)) && _mainGame.Test!.AllowAnyMove == false)
                return false;
            return !_gameContainer.AlreadyDrew;
        }

        protected override bool CanEnablePile1()
        {
            return !NeedsSpecial;
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            int newDeck = _model.PlayerHand1!.ObjectSelected();
            if (newDeck == 0)
            {
                await UIPlatform.ShowMessageAsync("Sorry, must select a card first");
                return;
            }
            if (_mainGame!.CanPlay(newDeck) == false)
            {
                await UIPlatform.ShowMessageAsync("Illegal move");
                return;
            }
            await _mainGame.ProcessPlayAsync(newDeck);
        }
        public override bool CanEnableAlways()
        {
            return true;
        }
        public override bool CanEndTurn()
        {
            var thisCard = _model.Pile1!.GetCardInfo();
            if (thisCard.Instructions == EnumInstructionList.Flip || thisCard.Instructions == EnumInstructionList.Cut)
                return false;
            if (_mainGame!.SingleInfo!.MainHandList.Any(items => _mainGame.CanPlay(items.Deck)))
                return false;
            return _gameContainer.AlreadyDrew;
        }

        private string _nextPlayer = "";
        [VM]
        public string NextPlayer
        {
            get { return _nextPlayer; }
            set
            {
                if (SetProperty(ref _nextPlayer, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private bool NeedsSpecial
        {
            get
            {
                var thisCard = _model.Pile1!.GetCardInfo();
                return thisCard.Instructions == EnumInstructionList.Cut || thisCard.Instructions == EnumInstructionList.Flip;
            }
        }
        public bool CanCut()
        {
            var thisCard = _model.Pile1!.GetCardInfo();
            return thisCard.Instructions == EnumInstructionList.Cut;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task CutAsync()
        {
            await _mainGame!.CutDeckAsync();
        }
        public bool CanFlip()
        {
            var thisCard = _model.Pile1!.GetCardInfo();
            return thisCard.Instructions == EnumInstructionList.Flip;
        }
        [Command(EnumCommandCategory.Game)]
        public async Task FlipAsync()
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("flipdeck");
            await _mainGame!.FlipDeckAsync();
        }
    }
}