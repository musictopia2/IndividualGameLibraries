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
using BaseSolitaireClassesCP.MiscClasses;
using BaseSolitaireClassesCP.PileViewModels;
using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.RegularDeckOfCards;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
using BasicGameFramework.Extensions;
//i think this is the most common things i like to do
namespace CaptiveQueensSolitaireCP
{
    public class CustomWaste : IWaste
    {
        public bool IsEnabled { get; set; }
        public int CardsNeededToBegin { get; set; }
        public int HowManyPiles { get; set; }
#pragma warning disable 0067

        public event WastePileSelectedEventHandler? PileSelectedAsync;
        public event WasteDoubleClickEventHandler? DoubleClickAsync;
#pragma warning restore 0067

        public void AddSingleCard(int whichOne, SolitaireCard thisCard)
        {

        }

        public DeckObservableDict<SolitaireCard> CardList = new DeckObservableDict<SolitaireCard>();

        public void ClearBoard(IDeckDict<SolitaireCard> thisCol)
        {
            if (thisCol.Count != CardsNeededToBegin)
                throw new BasicBlankException($"Needs {CardsNeededToBegin}, not {thisCol.Count}");
            if (thisCol.Any(items => items.Value != EnumCardValueList.Queen))
                throw new BasicBlankException("Only queens can be used");
            thisCol.First().Angle = EnumRotateCategory.RotateOnly90;
            thisCol[2].Angle = EnumRotateCategory.RotateOnly90;
            CardList.ReplaceRange(thisCol);
        }

        public void DoubleClickColumn(int index)
        {

        }

        public void FirstLoad(bool isKlondike, IDeckDict<SolitaireCard> cardList)
        {

        }

        public void FirstLoad(int rows, int columns)
        {

        }



        public void GetUnknowns()
        {

        }



        public async Task LoadGameAsync(SavedWaste gameData)
        {

            DeckRegularDict<SolitaireCard> tempList = await js.DeserializeObjectAsync<DeckRegularDict<SolitaireCard>>(gameData.PileData);
            CardList.ReplaceRange(tempList);
        }

        public void MoveCards(int whichOne, int lasts)
        {

        }

        public void MoveSingleCard(int whichOne)
        {

        }



        public void RemoveSingleCard()
        {

        }

        public void SelectUnselectPile(int whichOne)
        {

        }

        public void UnselectAllColumns()
        {

        }
        public int OneSelected()
        {
            return -1;
        }
        public bool CanAddSingleCard(int WhichOne, SolitaireCard thisCard)
        {
            return false;
        }

        public bool CanMoveCards(int whichOne, out int lastOne)
        {
            lastOne = -1;
            return false;
        }

        public bool CanMoveToAnotherPile(int whichOne)
        {
            return false;
        }

        public bool CanSelectUnselectPile(int whichOne)
        {
            throw new NotImplementedException();
        }
        public IDeckDict<SolitaireCard> GetAllCards()
        {
            throw new NotImplementedException();
        }

        public SolitaireCard GetCard()
        {
            throw new NotImplementedException();
        }

        public SolitaireCard GetCard(int whichOne)
        {
            throw new NotImplementedException();
        }

        public async Task<SavedWaste> GetSavedGameAsync()
        {
            SavedWaste output = new SavedWaste();
            output.PileData = await js.SerializeObjectAsync(CardList.ToRegularDeckDict());
            return output;
        }
        public bool HasCard(int whichOne)
        {
            return false;
        }
    }
}
