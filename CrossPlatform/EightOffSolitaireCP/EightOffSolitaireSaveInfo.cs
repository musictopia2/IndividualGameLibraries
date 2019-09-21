using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using BasicGameFramework.Attributes;
using BaseSolitaireClassesCP.MainClasses;
using BasicGameFramework.BasicDrawables.Dictionary;
using BaseSolitaireClassesCP.Cards;

namespace EightOffSolitaireCP
{
    [SingletonGame]
    public class EightOffSolitaireSaveInfo : SolitaireSavedClass
    {
        //anything else needed to save a game will be here.
        public DeckRegularDict<SolitaireCard> ReserveList { get; set; } = new DeckRegularDict<SolitaireCard>();
    }
}
