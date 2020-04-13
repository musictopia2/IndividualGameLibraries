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
using BasicGamingUIWPFLibrary.BasicControls.GameFrames;
using LifeBoardGameCP.Cards;
using LifeBoardGameCP.Graphics;

namespace LifeBoardGameWPF
{
    public class LifeHandWPF : BaseHandWPF<LifeBaseCard, CardCP, CardWPF>
    {
        protected override void AfterCollectionChange()
        {
            if (ObjectList!.Count == 0)
                return;
            if (ObjectList.First().IsUnknown)
            {
                Divider = 1.4;
                MaximumCards = 0;
                Height = 900;
            }
            else
            {
                Divider = 1;
                MaximumCards = ObjectList.Count;
            }
            RecalulateFrames();
        }
    }
}