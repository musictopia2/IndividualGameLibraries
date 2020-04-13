using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using FluxxCP.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluxxCP.ViewModels
{
    public abstract class BasicKeeperScreen : Screen, IBlankGameVM
    {
        public BasicKeeperScreen(FluxxGameContainer gameContainer, KeeperContainer keeperContainer)
        {
            GameContainer = gameContainer;
            KeeperContainer = keeperContainer;
            CommandContainer = gameContainer.Command;
        }
        public CommandContainer CommandContainer { get; set; }
        protected FluxxGameContainer GameContainer { get; }
        protected KeeperContainer KeeperContainer { get; }
    }
}
