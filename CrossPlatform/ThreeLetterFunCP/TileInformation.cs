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
using BasicGameFramework.BasicDrawables.BasicClasses;
using BasicGameFramework.BasicDrawables.Interfaces;
using SkiaSharp;
namespace ThreeLetterFunCP
{
    public class TileInformation : SimpleDeckObject, IDeckObject
    {
        private char _Letter;
        public char Letter
        {
            get
            {
                return _Letter;
            }

            set
            {
                if (SetProperty(ref _Letter, value) == true)
                {
                }
            }
        }

        private bool _IsMoved; // something else will change the color used (?)
        public bool IsMoved
        {
            get
            {
                return _IsMoved;
            }

            set
            {
                if (SetProperty(ref _IsMoved, value) == true)
                {
                }
            }
        }

        public int Index { get; set; } // hopefully no need for bindings.  this is needed for hints for positioning for the use for the card (if needed)
        public TileInformation()
        {
            DefaultSize = new SKSize(19, 30);
        }

        public void Reset() { }

        public void Populate(int chosen)
        {
            //decided to do nothing.
            //only needed to implement it because otherwise, can't use the hand view model for the tiles.
        }
    }
}