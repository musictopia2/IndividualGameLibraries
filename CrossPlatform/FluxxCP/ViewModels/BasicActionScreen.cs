using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FluxxCP.Containers;
using FluxxCP.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace FluxxCP.ViewModels
{
    public abstract class BasicActionScreen : Screen, IBlankGameVM
    {
        private readonly KeeperContainer _keeperContainer;
        private readonly FluxxDelegates _delegates;

        public BasicActionScreen(FluxxGameContainer gameContainer,
            ActionContainer actionContainer,
            KeeperContainer keeperContainer,
            FluxxDelegates delegates,
            IFluxxEvent fluxxEvent,
            BasicActionLogic basicActionLogic
            )
        {
            GameContainer = gameContainer;
            ActionContainer = actionContainer;
            _keeperContainer = keeperContainer;
            _delegates = delegates;
            FluxxEvent = fluxxEvent;
            BasicActionLogic = basicActionLogic;
            CommandContainer = gameContainer.Command;
            ActionContainer.PropertyChanged += ActionContainer_PropertyChanged;
            ButtonChooseCardVisible = ActionContainer.ButtonChooseCardVisible;
        }

        protected override Task TryCloseAsync()
        {
            ActionContainer.PropertyChanged -= ActionContainer_PropertyChanged;
            return base.TryCloseAsync();
        }
        private void ActionContainer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ButtonChooseCardVisible))
            {
                ButtonChooseCardVisible = ActionContainer.ButtonChooseCardVisible;
            }
        }

        public CommandContainer CommandContainer { get; set; }
        protected FluxxGameContainer GameContainer { get; }
        protected ActionContainer ActionContainer { get; }
        protected IFluxxEvent FluxxEvent { get; }
        protected BasicActionLogic BasicActionLogic { get; }
        [Command(EnumCommandCategory.Plain)] //forgot this attribute.
        public async Task ShowKeepersAsync()
        {
            if (_delegates.LoadKeeperScreenAsync == null)
            {
                throw new BasicBlankException("Nobody is loading the keeper screen.  Rethink");
            }
            _keeperContainer.ShowKeepers();
            await _delegates.LoadKeeperScreenAsync.Invoke(_keeperContainer); //if we need more details, will provide.
        }

        private bool _buttonChooseCardVisible;

        public bool ButtonChooseCardVisible //it may or may not need this.
        {
            get { return _buttonChooseCardVisible; }
            set
            {
                if (SetProperty(ref _buttonChooseCardVisible, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

    }
}
