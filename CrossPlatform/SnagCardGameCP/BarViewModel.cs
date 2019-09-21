using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.Extensions;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
namespace SnagCardGameCP
{
    public class BarViewModel : HandViewModel<SnagCardGameCardInformation>
    {
        private bool _wasSaved;
        protected override bool CanSelectSingleObject(SnagCardGameCardInformation thisObject)
        {
            if (thisObject.Equals(HandList.Last()))
                return true;
            if (HandList.Count() == 1)
                return false; //i think.
            int index = HandList.Count - 2;
            return thisObject.Equals(HandList[index]);
        }

        private readonly SnagCardGameMainGameClass _mainGame;
        public BarViewModel(IBasicGameVM thisMod) : base(thisMod)
        {
            _mainGame = thisMod.MainContainer!.Resolve<SnagCardGameMainGameClass>();
            Text = "Bar";
        }
        private bool NeedsReverse()
        {
            if (_mainGame.ThisData!.MultiPlayer == false && _wasSaved == false)
                return true;
            if (_mainGame.ThisData.MultiPlayer == false)
                return false;
            if (_mainGame.ThisData.Client == false && _wasSaved == false)
                return true;
            if (_mainGame.ThisData.Client == true && _wasSaved == true)
                return true;
            return false;
        }
        public void LoadBarCards(IDeckDict<SnagCardGameCardInformation> thisList)
        {
            if (_wasSaved == false && thisList.Count != 5)
                throw new BasicBlankException("Must have 5 cards for the bar.");
            if (_wasSaved == true && thisList.Count > 5)
                throw new BasicBlankException("The bar can never have more than 5 cards");
            bool rets = NeedsReverse();
            if (rets == true)
                thisList.Reverse();
            DeckRegularDict<SnagCardGameCardInformation> tempList = new DeckRegularDict<SnagCardGameCardInformation>();
            thisList.ForEach(thisCard =>
            {
                var newCard = new SnagCardGameCardInformation();
                newCard.Populate(thisCard.Deck);
                newCard.Angle = EnumRotateCategory.RotateOnly90;
                tempList.Add(newCard);
            });
            HandList.ReplaceRange(tempList);
        }
        public void LoadSavedGame()
        {
            _wasSaved = true;
            LoadBarCards(_mainGame.SaveRoot!.BarList);
        }
        public void Clear()
        {
            _wasSaved = false; //try this way.  so if autoresume but next round, hopefully will be okay (?)
        }
        public DeckRegularDict<SnagCardGameCardInformation> PossibleList
        {
            get
            {
                if (HandList.Count <= 2)
                    return HandList.ToRegularDeckDict();
                return new DeckRegularDict<SnagCardGameCardInformation>() { HandList.First(), HandList[1] };
            }
        }
    }
}