using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using BasicGameFramework.Attributes;

namespace PokerCP
{
    [SingletonGame]
    public class PokerSaveInfo : ObservableObject
    {
        public CustomBasicList<int> DeckList { get; set; } = new CustomBasicList<int>(); //hopefully now its okay to use decklist since we don't have globals anymore
        //anything else needed to save a game will be here.

    }
}
