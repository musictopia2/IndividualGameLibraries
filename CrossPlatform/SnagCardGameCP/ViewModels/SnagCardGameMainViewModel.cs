using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using BasicGameFrameworkLibrary.SpecializedGameTypes.TrickClasses;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using SnagCardGameCP.Cards;
using SnagCardGameCP.Data;
using SnagCardGameCP.Logic;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace SnagCardGameCP.ViewModels
{
    [InstanceGame]
    public class SnagCardGameMainViewModel : TrickCardGamesVM<SnagCardGameCardInformation, EnumSuitList>,
        ITrickDummyHand<EnumSuitList, SnagCardGameCardInformation>
    {
        private readonly SnagCardGameMainGameClass _mainGame; //if we don't need, delete.
        private readonly SnagCardGameVMData _model;

        public SnagCardGameMainViewModel(CommandContainer commandContainer,
            SnagCardGameMainGameClass mainGame,
            SnagCardGameVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = true;
            _model.Bar1.SendEnableProcesses(this, () =>
            {
                if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
                    return false;
                return !_mainGame.SaveRoot.FirstCardPlayed;
            });
        }
        //anything else needed is here.
        protected override bool CanEnableDeck()
        {
            return false; //otherwise, can't compile.
        }

        protected override bool CanEnablePile1()
        {
            return false; //otherwise, can't compile.
        }

        protected override async Task ProcessDiscardClickedAsync()
        {
            //if we have anything, will be here.
            await Task.CompletedTask;
        }
        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
                return false;
            return _mainGame.SaveRoot.FirstCardPlayed;
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

        public DeckObservableDict<SnagCardGameCardInformation> GetCurrentHandList()
        {
            if (_mainGame!.SaveRoot!.FirstCardPlayed == false)
                return _model.Bar1!.PossibleList.ToObservableDeckDict();
            return _mainGame.SingleInfo!.MainHandList.ToObservableDeckDict();
        }
        public int CardSelected()
        {
            if (_mainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
                throw new BasicBlankException("Only self should know what is selected");
            if (_mainGame.SaveRoot!.FirstCardPlayed == false)
                return _model.Bar1!.ObjectSelected();
            return _model.PlayerHand1!.ObjectSelected();
        }
        public void RemoveCard(int deck)
        {
            if (_mainGame!.SaveRoot!.FirstCardPlayed == false)
                _model.Bar1!.HandList.RemoveObjectByDeck(deck);
            else
                _mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(deck); //try this way instead.
        }

    }
}