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
using BasicGameFramework.MultiplePilesViewModels;
using BasicGameFramework.ViewModelInterfaces;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
using BasicGameFramework.RegularDeckOfCards;
//i think this is the most common things i like to do
namespace CaptiveQueensSolitaireCP
{
    public class CustomMain : BasicMultiplePilesCP<SolitaireCard>, IMain
    {
        public int CardsNeededToBegin { get; set; }
        public bool IsRound { get; set; }

#pragma warning disable 0067

        public event MainPileClickedEventHandler? PileSelectedAsync;
#pragma warning restore 0067
        private readonly CaptiveQueensSolitaireViewModel _thisMod;

        public CustomMain(IBasicGameVM ThisMod) : base(ThisMod)
        {
            _thisMod = (CaptiveQueensSolitaireViewModel)ThisMod;
        }

        public void SetSavedScore(int previousScore)
        {
            _thisMod.Score = previousScore;
        }

        public override void ClearBoard()
        {
            base.ClearBoard();
            _thisMod.Score = 4;
        }

        public void AddCard(int pile, SolitaireCard thisCard)
        {
            AddCardToPile(pile, thisCard);
            _thisMod.Score++;
        }

        public void FirstLoad(bool needToMatch, bool showNextNeeded)
        {
            Columns = 8;
            Rows = 1;
            HasFrame = true;
            HasText = false;
            Style = EnumStyleList.HasList;
            LoadBoard();

            static EnumRotateCategory GetAngle(int index)
            {
                if (index == 2 || index == 3 || index == 6 || index == 7)
                    return EnumRotateCategory.RotateOnly90;
                return EnumRotateCategory.None;
            }
            PileList!.ForEach(thisPile =>
            {
                int index = PileList.IndexOf(thisPile);
                thisPile.Angle = GetAngle(index);
            });

        }
        public int HowManyPiles()
        {
            if (PileList!.Count != 8)
                throw new BasicBlankException("There should have been 8 piles");
            return PileList.Count;
        }
        protected override async Task OnPileClickedAsync(int Index, BasicPileInfo<SolitaireCard> ThisPile)
        {
            await PileSelectedAsync!(Index);
        }

        public bool CanAddCard(int pile, SolitaireCard thiscard)
        {
            if (HasCard(pile) == false)
            {
                return pile switch
                {
                    0 => thiscard.Value == EnumCardValueList.Six && thiscard.Suit == EnumSuitList.Hearts,
                    1 => thiscard.Value == EnumCardValueList.Five && thiscard.Suit == EnumSuitList.Hearts,
                    2 => thiscard.Value == EnumCardValueList.Six && thiscard.Suit == EnumSuitList.Clubs,
                    3 => thiscard.Value == EnumCardValueList.Five && thiscard.Suit == EnumSuitList.Clubs,
                    4 => thiscard.Value == EnumCardValueList.Six && thiscard.Suit == EnumSuitList.Diamonds,
                    5 => thiscard.Value == EnumCardValueList.Five && thiscard.Suit == EnumSuitList.Diamonds,
                    6 => thiscard.Value == EnumCardValueList.Six && thiscard.Suit == EnumSuitList.Spades,
                    7 => thiscard.Value == EnumCardValueList.Five && thiscard.Suit == EnumSuitList.Spades,
                    _ => throw new BasicBlankException("Only has 8 piles"),
                };
            }
            var firstCard = PileList![pile].ObjectList.First();
            var lastCard = GetLastCard(pile);
            if (lastCard.Suit != firstCard.Suit)
                throw new BasicBlankException("Must match suit");
            if (lastCard.Suit != thiscard.Suit)
                return false;
            if (lastCard.Value + 1 == thiscard.Value && firstCard.Value == EnumCardValueList.Six)
                return true;
            if (lastCard.Value - 1 == thiscard.Value && firstCard.Value == EnumCardValueList.Five)
                return true;
            return firstCard.Value == EnumCardValueList.Five && lastCard.Value == EnumCardValueList.LowAce && thiscard.Value == EnumCardValueList.King;
        }

        public int StartNumber()
        {
            throw new NotImplementedException(); // until i decide what to do
        }

        public void ClearBoard(IDeckDict<SolitaireCard> thisList) //somehow was never done.
        {

        }

        public async Task LoadGameAsync(string data)
        {
            PileList = await js.DeserializeObjectAsync<CustomBasicList<BasicPileInfo<SolitaireCard>>>(data);
        }

        public async Task<string> GetSavedPilesAsync()
        {
            return await js.SerializeObjectAsync(PileList);
        }

        public void AddCards(int Pile, IDeckDict<SolitaireCard> list)
        {

        }

        public void AddCards(IDeckDict<SolitaireCard> list)
        {

        }

        
    }
}