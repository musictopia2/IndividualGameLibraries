﻿using CommonBasicStandardLibraries.Messenging;
using LifeBoardGameCP.Data;

namespace LifeBoardGameXF.Views
{
    public class TradeSalaryView : BasicPlayerPicker
    {
        public TradeSalaryView(LifeBoardGameVMData model, IEventAggregator aggregator) : base(model, aggregator)
        {
        }
    }
}