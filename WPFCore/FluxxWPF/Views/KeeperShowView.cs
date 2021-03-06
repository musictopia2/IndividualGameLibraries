﻿using FluxxCP.Containers;

namespace FluxxWPF.Views
{
    public class KeeperShowView : KeeperBaseView
    {
        public KeeperShowView(FluxxGameContainer gameContainer,
            KeeperContainer keeperContainer,
            ActionContainer actionContainer,
            FluxxVMData model) : base(gameContainer, keeperContainer, actionContainer, model)
        {
        }

        protected override EnumKeeperCategory KeeperCategory => EnumKeeperCategory.Show;
    }
}
