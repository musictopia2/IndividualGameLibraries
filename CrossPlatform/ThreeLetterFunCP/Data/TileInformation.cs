using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using SkiaSharp;

namespace ThreeLetterFunCP.Data
{
    public class TileInformation : SimpleDeckObject, IDeckObject
    {
        private char _letter;
        public char Letter
        {
            get
            {
                return _letter;
            }

            set
            {
                if (SetProperty(ref _letter, value) == true)
                {
                }
            }
        }

        private bool _isMoved; // something else will change the color used (?)
        public bool IsMoved
        {
            get
            {
                return _isMoved;
            }

            set
            {
                if (SetProperty(ref _isMoved, value) == true)
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
