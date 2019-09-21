using BasicGameFramework.BasicDrawables.Dictionary;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DominosMexicanTrainCP
{
    public class LongestTrainClass
    {
        private MexicanDomino CloneDominoPiece(MexicanDomino dominoPiece)
        {
            MexicanDomino output = new MexicanDomino();
            output.CurrentFirst = dominoPiece.CurrentFirst;
            output.CurrentSecond = dominoPiece.CurrentSecond;
            output.Deck = dominoPiece.Deck; //this time, we need deck.
            return output;
        }
        private CustomBasicList<MexicanDomino> CloneList(ICustomBasicList<MexicanDomino> originalList)
        {
            CustomBasicList<MexicanDomino> output = new CustomBasicList<MexicanDomino>();
            originalList.ForEach(current =>
            {
                MexicanDomino thisDomino = new MexicanDomino();
                thisDomino.CurrentFirst = current.CurrentFirst;
                thisDomino.CurrentSecond = current.CurrentSecond;
                thisDomino.FirstNum = current.FirstNum;
                thisDomino.SecondNum = current.SecondNum;
                thisDomino.Deck = current.Deck;
                thisDomino.Keeps = current.Keeps;
                thisDomino.Train = current.Train;
                thisDomino.Status = current.Status;
                output.Add(thisDomino);
            });
            return output;
        }
        private void ReplacePieceFirstSecond(MexicanDomino piece)
        {
            int temps = piece.CurrentFirst;
            piece.CurrentFirst = piece.CurrentSecond;
            piece.CurrentSecond = temps;
        }
        public DeckRegularDict<MexicanDomino> GetTrainList(DeckObservableDict<MexicanDomino> piecesCollection, int givenNumber)
        {
            DeckRegularDict<MexicanDomino> output;
            Dictionary<int, MexicanDomino> matchingPieces = new Dictionary<int, MexicanDomino>();
            List<int> Positions = new List<int>();
            DeckObservableDict<MexicanDomino> TempList = new DeckObservableDict<MexicanDomino>();
            foreach (var currentPiece in piecesCollection)
                TempList.Add(currentPiece);
            bool anyPieceMatch = false;
            int count = 1;
            DateTime newTime;
            newTime = DateTime.Now.AddSeconds(5);
            foreach (var currentPiece in piecesCollection)
            {
                if ((currentPiece.CurrentFirst == givenNumber) | (currentPiece.CurrentSecond == givenNumber))
                {
                    MexicanDomino newPiece = new MexicanDomino();
                    newPiece.CurrentFirst = currentPiece.CurrentFirst;
                    newPiece.CurrentSecond = currentPiece.CurrentSecond;
                    newPiece.Deck = currentPiece.Deck;
                    newPiece.CurrentFirst = currentPiece.CurrentFirst;
                    newPiece.Keeps = currentPiece.Keeps;
                    newPiece.CurrentSecond = currentPiece.CurrentSecond;
                    newPiece.Status = currentPiece.Status;
                    newPiece.Train = currentPiece.Train;
                    Positions.Add(count);
                    matchingPieces.Add(count, newPiece);
                    anyPieceMatch = true;
                }
                count += 1;
            }
            if (anyPieceMatch == false)
                return new DeckRegularDict<MexicanDomino>();
            CustomBasicList<CustomBasicList<MexicanDomino>> allTrains = new CustomBasicList<CustomBasicList<MexicanDomino>>();
            count = 1;
            // Searching for Trains
            foreach (var currentPiece in matchingPieces.Values)
            {
                // Searching for Train
                CustomBasicList<MexicanDomino> train = new CustomBasicList<MexicanDomino>();
                if (currentPiece.CurrentFirst != givenNumber)
                    ReplacePieceFirstSecond(currentPiece);
                train.Add(currentPiece);
                // All Pieces Clone with Removing the first matched
                var allPieces = CloneList(TempList);
                allPieces.RemoveAt(Positions[count - 1] - 1); // 0 based
                int i = 1;
                MexicanDomino trainEnd = currentPiece;
                CustomBasicList<CustomBasicList<MexicanDomino>> trainPieceList = new CustomBasicList<CustomBasicList<MexicanDomino>>();
                CustomBasicList<CustomBasicList<MexicanDomino>> trainSecondList = new CustomBasicList<CustomBasicList<MexicanDomino>>();
                CustomBasicList<MexicanDomino> trainDominoList = new CustomBasicList<MexicanDomino>();
                CustomBasicList<int> trainIndexList = new CustomBasicList<int>();
                while (i <= allPieces.Count)
                {
                    MexicanDomino piece;
                    piece = allPieces[i - 1]; // because 0 based
                    if (piece.CurrentFirst == trainEnd.CurrentSecond)
                    {
                        trainPieceList.Add(CloneList(allPieces));
                        trainSecondList.Add(CloneList(train));
                        trainDominoList.Add(CloneDominoPiece(trainEnd));
                        train.Add(piece);
                        allPieces.RemoveAt(i - 1); // 0 based
                        trainEnd = piece;
                        i = 1;
                    }
                    else if (piece.CurrentSecond == trainEnd.CurrentSecond)
                    {
                        trainPieceList.Add(CloneList(allPieces));
                        trainSecondList.Add(CloneList(train));
                        trainDominoList.Add(CloneDominoPiece(trainEnd));
                        trainIndexList.Add(i + 1);
                        ReplacePieceFirstSecond(piece);
                        train.Add(piece);
                        allPieces.RemoveAt(i - 1); // 0 based
                        trainEnd = piece;
                        i = 1;
                    }
                    else
                    {
                        if (i < allPieces.Count)
                        {
                            i += 1;
                        }
                        else if ((trainPieceList.Count > 0) & (i >= allPieces.Count) & trainIndexList.Count > 0)
                        {
                            allTrains.Add(CloneList(train));
                            if (allPieces.Count == 0)
                                throw new BasicBlankException("allpieces has nothing left");
                            allPieces = trainPieceList.First();
                            trainPieceList.RemoveAt(0);
                            if (trainSecondList.Count == 0)
                                throw new BasicBlankException("trainsecondlist has nothing left");
                            train = trainSecondList.First();
                            trainSecondList.RemoveAt(0);
                            if (trainDominoList.Count == 0)
                                throw new BasicBlankException("traindominolist has nothing left");
                            trainEnd = trainDominoList.First();
                            trainDominoList.RemoveAt(0);
                            if (trainIndexList.Count == 0)
                                i++;
                            else
                            {
                                i = trainIndexList.First();
                                trainIndexList.RemoveAt(0);
                            }

                        }
                        else
                        {

                            // Saves the current list
                            allTrains.Add(CloneList(train));

                            // No more List possible
                            break;
                        }
                        if (DateTime.Now > newTime)
                            // MsgBox("taking too long")
                            break;
                    }
                }
                if (train.Count > 0)

                    allTrains.Add(CloneList(train));
                if (DateTime.Now > newTime)
                    break;
                count += 1;
            }
            int j = 1;
            int piecesCount = 0;
            bool multiplePiecesCount = false;
            CustomBasicList<int> piecesPositions = new CustomBasicList<int>();
            while (j <= allTrains.Count)
            {
                CustomBasicList<MexicanDomino> currentTrain;
                currentTrain = allTrains[j - 1]; // 0 based
                if (currentTrain.Count > piecesCount)
                {
                    piecesCount = currentTrain.Count;
                    multiplePiecesCount = false;
                    piecesPositions.Clear();
                    piecesPositions.Add(j);
                }
                else if (currentTrain.Count == piecesCount)
                {
                    multiplePiecesCount = true;
                    piecesPositions.Add(j);
                }
                j += 1;
            }
            CustomBasicList<MexicanDomino> tempCol;
            if (!multiplePiecesCount)
            {
                tempCol = allTrains[piecesPositions[0] - 1]; // try 0 based here
                output = new DeckRegularDict<MexicanDomino>();
                MexicanDomino newDomino;
                foreach (var thisDomino in tempCol)
                {
                    newDomino = piecesCollection.GetSpecificItem(thisDomino.Deck);
                    newDomino.CurrentFirst = thisDomino.CurrentFirst;
                    newDomino.CurrentSecond = thisDomino.CurrentSecond;
                    piecesCollection.RemoveObjectByDeck(newDomino.Deck); // try this way
                    output.Add(newDomino);
                }
                return output;
            }
            else
            {
                int points;
                int bestPoints = 0;
                int bestPosition = 0;
                var loopTo = piecesPositions.Count - 1;
                for (points = 0; points <= loopTo; points++)
                {
                    int totalPoints = 0;
                    CustomBasicList<MexicanDomino> currentTrain = allTrains[piecesPositions[points] - 1]; // 0 based
                    foreach (var piece in currentTrain)

                        totalPoints += piece.Points;

                    if (totalPoints > bestPoints)
                        bestPosition = points;
                }
                tempCol = allTrains[piecesPositions[bestPosition] - 1]; // 0 based
                output = new DeckRegularDict<MexicanDomino>();
                MexicanDomino NewDomino;
                foreach (var ThisDomino in tempCol)
                {
                    NewDomino = piecesCollection.GetSpecificItem(ThisDomino.Deck);
                    NewDomino.CurrentFirst = ThisDomino.CurrentFirst;
                    NewDomino.CurrentSecond = ThisDomino.CurrentSecond;
                    piecesCollection.RemoveSpecificItem(NewDomino); // try this way
                    output.Add(NewDomino);
                }
                return output;
            }
        }
    }
}