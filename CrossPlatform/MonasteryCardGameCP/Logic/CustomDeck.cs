using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using CommonBasicStandardLibraries.CollectionClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonasteryCardGameCP.Logic
{
    public class CustomDeck : IRegularDeckInfo
    {
        int IRegularDeckInfo.HowManyDecks => 2;

        bool IRegularDeckInfo.UseJokers => false;

        int IRegularDeckInfo.GetExtraJokers => 0;

        int IRegularDeckInfo.LowestNumber => 1;

        int IRegularDeckInfo.HighestNumber => 13; //try this way.

        CustomBasicList<ExcludeRCard> IRegularDeckInfo.ExcludeList => new CustomBasicList<ExcludeRCard>();

        CustomBasicList<EnumSuitList> IRegularDeckInfo.SuitList => Helpers.GetCompleteSuitList;

        int IDeckCount.GetDeckCount()
        {
            return 104;
        }
    }
}