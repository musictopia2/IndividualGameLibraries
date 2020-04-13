using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System.Linq;
using ThreeLetterFunCP.Logic;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace ThreeLetterFunCP.Data
{
    public class ThreeLetterFunDeckInfo : IDeckCount
    {
        //this will avoid overflow issues.
        internal CustomBasicList<SavedCard> PrivateSavedList { get; set; } = new CustomBasicList<SavedCard>();
        internal void InitCards()
        {
            ThreeLetterFunSaveInfo saveRoot = cons!.Resolve<ThreeLetterFunSaveInfo>();
            GlobalHelpers global = cons!.Resolve<GlobalHelpers>();
            if (saveRoot.Level == EnumLevel.None)
                throw new BasicBlankException("Must choose the level before you can initialize the cards");
            PrivateSavedList = global.SavedCardList.Where(items => items.Level == saveRoot.Level).ToCustomBasicList();
        }
        public int GetDeckCount()
        {
            InitCards(); //this is one place for sure to do it.
            return PrivateSavedList.Count;
        }
    }
}
