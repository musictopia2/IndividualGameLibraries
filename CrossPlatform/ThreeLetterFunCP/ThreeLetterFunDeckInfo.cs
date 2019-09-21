using BasicGameFramework.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
namespace ThreeLetterFunCP
{
    public class ThreeLetterFunDeckInfo : IDeckCount
    {
        internal CustomBasicList<SavedCard> PrivateSavedList = new CustomBasicList<SavedCard>();
        internal void InitCards()
        {
            if (_mainGame.SaveRoot!.Level == EnumLevel.None)
                throw new BasicBlankException("Must choose the level before you can initialize the cards");
            PrivateSavedList = _mainGame.ThisGlobal!.SavedCardList.Where(items => items.Level == _mainGame.SaveRoot.Level).ToCustomBasicList();
        }
        private readonly ThreeLetterFunMainGameClass _mainGame;
        public ThreeLetterFunDeckInfo(ThreeLetterFunMainGameClass mainGame)
        {
            _mainGame = mainGame;
        }
        public int GetDeckCount()
        {
            InitCards(); //this is one place for sure to do it.
            return PrivateSavedList.Count;
        }
    }
}