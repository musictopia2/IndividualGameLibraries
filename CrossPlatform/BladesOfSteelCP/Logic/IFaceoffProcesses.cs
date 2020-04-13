using BasicGameFrameworkLibrary.RegularDeckOfCards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BladesOfSteelCP.Logic
{
    public interface IFaceoffProcesses
    {
        Task FaceOffCardAsync(RegularSimpleCard card);

    }
}