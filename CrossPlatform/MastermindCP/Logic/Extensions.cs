using CommonBasicStandardLibraries.Messenging;
using MastermindCP.Data;
using MastermindCP.EventModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MastermindCP.Logic
{
    public static class Extensions
    {
        //maybe those 2 are not needed anymore since i have things better splitted apart.



        //public static void ShowSolution(this IEventAggregator aggregator)
        //{
        //    //take some risks.
        //    aggregator.Publish(new ShowSolutionEventModel());
        //}
        //public static void HideSolution(this IEventAggregator aggregator)
        //{
        //    aggregator.Publish(new HideSolutionEventModel());
        //}
        public static async Task ScrollToGuessAsync(this IEventAggregator aggregator, Guess guess)
        {
            await aggregator.PublishAsync(new ScrollToGuessEventModel(guess));
        }
    }
}
