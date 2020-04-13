using System;
using System.Collections.Generic;
using System.Text;

namespace MahJongSolitaireCP.EventModels
{
    public class TileChosenEventModel
    {
        public int Deck { get; set; } //if deck is 0, then its invisible.
        //otherwise, will be whatever it is.

    }
}