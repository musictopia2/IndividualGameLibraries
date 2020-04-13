using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GolfCardGameCP.Logic
{
    public interface IBeginningProcesses
    {
        Task SelectBeginningAsync(int player, DeckRegularDict<RegularSimpleCard> selectList, DeckRegularDict<RegularSimpleCard> unselectList);
    }
}