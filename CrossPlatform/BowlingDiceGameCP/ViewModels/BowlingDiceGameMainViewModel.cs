using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.BasicGameDataClasses;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.DIContainers;
using BasicGameFrameworkLibrary.MultiplayerClasses.MainViewModels;
using BasicGameFrameworkLibrary.TestUtilities;
using BowlingDiceGameCP.Logic;
using System.Threading.Tasks;

namespace BowlingDiceGameCP.ViewModels
{
    [InstanceGame]
    public class BowlingDiceGameMainViewModel : BasicMultiplayerMainVM
    {
        private readonly BowlingDiceGameMainGameClass _mainGame; //if we don't need, delete.

        public BowlingDiceGameMainViewModel(CommandContainer commandContainer,
            BowlingDiceGameMainGameClass mainGame,
            IViewModelData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver
            )
            : base(commandContainer, mainGame, viewModel, basicData, test, resolver)
        {
            _mainGame = mainGame;
        }

        private bool _isExtended;
        [VM]
        public bool IsExtended
        {
            get { return _isExtended; }
            set
            {
                if (SetProperty(ref _isExtended, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _whichPart;
        [VM]
        public int WhichPart
        {
            get { return _whichPart; }
            set
            {
                if (SetProperty(ref _whichPart, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _whatFrame;
        [VM]
        public int WhatFrame
        {
            get { return _whatFrame; }
            set
            {
                if (SetProperty(ref _whatFrame, value))
                {
                    //can decide what to do when property changes
                }
            }
        }

        private bool _lastTurn;
        public override bool CanEndTurn()
        {
            if (base.CanEndTurn() == false)
                return false;
            if (WhichPart < 3)
                return false;
            if (_lastTurn == true)
                return false;
            return !IsExtended;
        }
        public bool CanContinueTurn => IsExtended;
        [Command(EnumCommandCategory.Game)]
        public async Task ContinueTurnAsync()
        {
            _mainGame.SaveRoot.IsExtended = false;
            _lastTurn = true;
            await Task.Delay(10);
            CommandContainer.ManualReport(); //try this.
        }

        public bool CanRoll
        {
            get
            {
                if (WhichPart < 3)
                {
                    return true;
                }
                return _lastTurn;
            }
        }
        [Command(EnumCommandCategory.Game)]
        public async Task RollAsync()
        {
            _lastTurn = false;
            await _mainGame.RollDiceAsync();
        }

    }
}