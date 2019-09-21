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
using Newtonsoft.Json;

namespace PersianSolitaireCP
{
    [SingletonGame]
    public class PersianSolitaireSaveInfo : SolitaireSavedClass
    {
        //anything else needed to save a game will be here.
        private int _DealNumber;

        public int DealNumber
        {
            get { return _DealNumber; }
            set
            {
                if (SetProperty(ref _DealNumber, value))
                {
                    //can decide what to do when property changes
                    if (MainMod == null)
                        return;

                }

            }
        }
        [JsonIgnore]
        internal PersianSolitaireViewModel? MainMod;

    }
}
