﻿using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;

namespace LifeBoardGameXF.Views
{
    public class StealTilesView : BasicPlayerPicker
    {
        public StealTilesView(LifeBoardGameVMData model, IEventAggregator aggregator, LifeBoardGameGameContainer gameContainer) : base(model, aggregator, gameContainer)
        {
        }
    }
}