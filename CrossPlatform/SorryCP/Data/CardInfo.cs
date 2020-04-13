using BasicGameFrameworkLibrary.BasicDrawables.BasicClasses;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.Exceptions;
using SkiaSharp;

namespace SorryCP.Data
{
    public class CardInfo : SimpleDeckObject, IDeckObject
    {
        public int Value { get; set; }
        public bool Sorry { get; set; }
        public bool CanTakeFromStart { get; set; }
        public bool AnotherTurn { get; set; }
        public bool SplitMove { get; set; }
        public bool Trade { get; set; }
        public int SpacesBackwards { get; set; }
        public int SpacesForward { get; set; }
        public string Details { get; set; } = "";
        public CardInfo()
        {
            DefaultSize = new SKSize(55, 72); //well see.
        }
        public void Populate(int chosen)
        {
            Deck = chosen;
            if (chosen <= 0)
                throw new BasicBlankException("Cannot be 0 or below");
            if (chosen < 6)
            {
                Value = 1;
                Details = "Move from start or move one forward one space";
                CanTakeFromStart = true;
            }
            else if (chosen < 10)
            {
                Details = "Move from start or move forward 2 spaces.  Draw Again.";
                Value = 2;
                CanTakeFromStart = true;
                AnotherTurn = true;
            }
            else if (chosen < 14)
            {
                Details = "Move forward three spaces.";
                Value = 3;
            }
            else if (chosen < 18)
            {
                Details = "Move backward four.";
                Value = 4;
                SpacesBackwards = 4;
            }
            else if (chosen < 22)
            {
                Details = "Move forward five spaces.";
                Value = 5;
            }
            else if (chosen < 26)
            {
                Details = "Move one forward seven spaces or split move between 2 pawns the seven spaces.";
                Value = 7;
                SplitMove = true;
            }
            else if (chosen < 30)
            {
                Details = "Move forward eight spaces.";
                Value = 8;
            }
            else if (chosen < 34)
            {
                Details = "Move forward ten spaces or move backward one space.";
                Value = 10;
                SpacesBackwards = 1;
            }
            else if (chosen < 38)
            {
                Details = "Move forward eleven spaces or change places with an opponent.";
                Value = 11;
                Trade = true;
            }
            else if (chosen < 42)
            {
                Details = "Move forward twelve spaces.";
                Value = 12;
            }
            else if (chosen < 46)
            {
                Details = "Move from start and swith places with an opponent who bump back to start.";
                Value = 13;
                Sorry = true;
            }
            else
            {
                throw new BasicBlankException("Cannot find sorry card.  Rethink");
            }

            if (Value < 13 && Value != 4)
                SpacesForward = Value;
        }
        public void Reset() { }
    }
}
