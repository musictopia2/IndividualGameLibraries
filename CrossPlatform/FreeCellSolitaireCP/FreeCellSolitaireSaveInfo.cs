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
using BasicGameFramework.MultiplePilesViewModels;
using BaseSolitaireClassesCP.Cards;

namespace FreeCellSolitaireCP
{
    [SingletonGame]
    public class FreeCellSolitaireSaveInfo : SolitaireSavedClass
    {
        //anything else needed to save a game will be here.
        public CustomBasicList<BasicPileInfo<SolitaireCard>> FreeCards = new CustomBasicList<BasicPileInfo<SolitaireCard>>();
    }
}