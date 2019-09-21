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
using BaseSolitaireClassesCP.PileInterfaces;
using BaseSolitaireClassesCP.Cards;
using BasicGameFramework.BasicDrawables.Dictionary;
//i think this is the most common things i like to do
namespace AlternationSolitaireCP
{
    public class CustomMain : IMain
    {
        public int CardsNeededToBegin { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsRound { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

#pragma warning disable 0067

        public event MainPileClickedEventHandler? PileSelectedAsync;
#pragma warning restore 0067

        public void SetSavedScore(int previousScore)
        {
            
        }

        public void ClearBoard()
        {
        }

        public void AddCard(int pile, SolitaireCard thisCard)
        {
        }

        public void FirstLoad(bool needToMatch, bool showNextNeeded)
        {
        }
        public int HowManyPiles()
        {
            return 0; // has to change to however many piles there are
        }

        public bool CanAddCard(int pile, SolitaireCard thiscard)
        {
            return false;
        }

        public bool HasCard(int Pile)
        {
            return false;
        }

        public int StartNumber()
        {
            throw new NotImplementedException(); // until i decide what to do
        }

        public void ClearBoard(IDeckDict<SolitaireCard> thisList)
        {
            
        }

        public async Task LoadGameAsync(string data)
        {

            await Task.CompletedTask;
        }

        public async Task<string> GetSavedPilesAsync()
        {
            return await Task.FromResult(""); //until i decide what the data will be (?)
        }

        public void AddCards(int Pile, IDeckDict<SolitaireCard> list)
        {
            
        }

        public void AddCards(IDeckDict<SolitaireCard> list)
        {
            
        }
    }
}
