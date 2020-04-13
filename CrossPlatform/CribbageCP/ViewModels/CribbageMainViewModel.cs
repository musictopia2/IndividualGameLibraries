using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CribbageCP.Data;
using CribbageCP.Logic;
using System.Linq;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
namespace CribbageCP.ViewModels
{
    [InstanceGame]
    public class CribbageMainViewModel : BasicCardGamesVM<CribbageCard>
    {
        private readonly CribbageMainGameClass _mainGame; //if we don't need, delete.
        private readonly CribbageVMData _model;
        public CribbageMainViewModel(CommandContainer commandContainer,
            CribbageMainGameClass mainGame,
            CribbageVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
            _model = viewModel;
            _model.Deck1.NeverAutoDisable = false;
            _model.Pile1.Text = "Start Card";
            if (_mainGame.PlayerList.Count == 2)
            {
                _model.CribFrame.Maximum = 4;
            }
            else
            {
                _model.CribFrame.Maximum = 3;
            }
            _model.MainFrame.Maximum = 10;
            _model.PlayerHand1.Maximum = 6;

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
            await Task.CompletedTask;
        }
        public override bool CanEnableAlways()
        {
            return true;
        }

        private int _totalScore;
        [VM]
        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                if (SetProperty(ref _totalScore, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private int _totalCount;
        [VM]
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                if (SetProperty(ref _totalCount, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private string _dealer = "";
        [VM]
        public string Dealer
        {
            get { return _dealer; }
            set
            {
                if (SetProperty(ref _dealer, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        protected override bool AlwaysEnableHand()
        {
            return false;
        }
        protected override bool CanEnableHand()
        {
            return _mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.PlayCard || _mainGame.SaveRoot.WhatStatus == EnumGameStatus.CardsForCrib;
        }
        public bool CanContinue => _mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.GetResultsCrib
                || _mainGame.SaveRoot.WhatStatus == EnumGameStatus.GetResultsHand;
        [Command(EnumCommandCategory.Game)]
        public async Task ContinueAsync()
        {
            _mainGame!.SingleInfo = _mainGame.PlayerList!.GetSelf();
            _mainGame.SingleInfo.FinishedLooking = true;
            if (_mainGame.BasicData!.MultiPlayer == true)
                await _mainGame.Network!.SendAllAsync("endview", _mainGame.SingleInfo.Id);
            await _mainGame.EndViewAsync();
        }

        public bool CanPlay => _mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.PlayCard;
        [Command(EnumCommandCategory.Game)]
        public async Task PlayAsync()
        {
            var thisList = _model.PlayerHand1!.ListSelectedObjects();
            if (thisList.Count > 1)
                throw new BasicBlankException("It should have only allowed selecting one at a time now");
            if (thisList.Count == 0)
            {
                await UIPlatform.ShowMessageAsync("You must choose a card to play");
                return;
            }
            if (_mainGame!.IsValidMove(thisList.Single()) == false)
            {
                await UIPlatform.ShowMessageAsync("You cannot play a card that makes it more than 31 points");
                return;
            }
            await _mainGame!.PlayCardAsync(thisList.Single());
        }

        public bool CanCrib => _mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.CardsForCrib;
        [Command(EnumCommandCategory.Game)]
        public async Task CribAsync()
        {
            var thisList = _model.PlayerHand1!.ListSelectedObjects();
            if (_mainGame!.PlayerList.Count() == 3 && thisList.Count > 1)
                throw new BasicBlankException("It should have only allowed selecting one card at a time because 3 players");
            int maxs;
            if (_mainGame.PlayerList.Count() == 3)
                maxs = 1;
            else
                maxs = 2;
            if (thisList.Count != maxs)
            {
                await UIPlatform.ShowMessageAsync($"Must select {maxs} cards for crib");
                return;
            }
            _mainGame.SingleInfo = _mainGame.PlayerList!.GetSelf();
            await _mainGame.ProcessCribAsync(thisList);
        }

    }
}