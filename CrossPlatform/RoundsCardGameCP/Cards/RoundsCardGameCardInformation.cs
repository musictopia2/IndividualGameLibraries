using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using SkiaSharp;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
//i think this is the most common things i like to do
namespace RoundsCardGameCP.Cards
{
    public class RoundsCardGameCardInformation : RegularTrickCard, IDeckObject
    {
        public RoundsCardGameCardInformation()
        {
            DefaultSize = new SKSize(55, 72); //this is neeeded too.
        }
        //public void Populate(int chosen)
        //{
        //    //populating the card.

        //}

        //public void Reset()
        //{
        //    //anything that is needed to reset.
        //}
    }
}
