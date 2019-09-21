using BasicGameFramework.Attributes;
using BasicGameFramework.GameGraphicsCP.Animations;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections.Generic;
namespace BackgammonCP
{
    [SingletonGame]
    public class GlobalClass
    {
        internal Dictionary<int, TriangleClass> TriangleList { get; set; } = new Dictionary<int, TriangleClass>();
        internal AnimateSkiaSharpGameBoard? Animates;
        internal bool MoveInProgress;
        internal CustomBasicList<MoveInfo> MoveList = new CustomBasicList<MoveInfo>();
        internal int FirstDiceValue;
        internal int SecondDiceValue;
        internal bool HadDoubles()
        {
            if (FirstDiceValue == 0)
                throw new BasicBlankException("The dice can never roll a 0.  Must populate the dice value first");
            return FirstDiceValue == SecondDiceValue;
        }
    }
}