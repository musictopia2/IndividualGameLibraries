using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using BasicGameFrameworkLibrary.DrawableListsObservable;
using ThreeLetterFunCP.Data;
using BasicGameFrameworkLibrary.CommandClasses;
//i think this is the most common things i like to do
namespace ThreeLetterFunCP.Logic
{
    public class TileBoardObservable : HandObservable<TileInformation>
    {
        public TileBoardObservable(CommandContainer command) : base(command)
        {
        }
        public void UpdateBoard()
        {
            ThreeLetterFunSaveInfo saveroot = cons!.Resolve<ThreeLetterFunSaveInfo>();

            if (saveroot.TileList.Count == 0)
            {
                HandList.Clear();
                return;
            }
            CustomBasicList<TileInformation> ThisList = new CustomBasicList<TileInformation>
                { saveroot.TileList.First(), saveroot.TileList[1] };
            HandList.ReplaceRange(ThisList);
        }
        public TileInformation? GetTile (bool isSelected)
        {
            if (isSelected)
            {
                if (HandList.First().IsSelected == true)
                    return HandList.First();
                else if (HandList[1].IsSelected == true)
                    return HandList[1];
                else
                    return null;
            }
            else if (HandList.First().IsSelected == false)
                return HandList.First();
            else if (HandList[1].IsSelected == false)
                return HandList[1];
            else
                return null;
        }
        public void Undo()
        {
            UnselectAllObjects();
            ThreeLetterFunSaveInfo saveroot = cons!.Resolve<ThreeLetterFunSaveInfo>(); //to stop the overflow issues by requiring reference to game class
            saveroot.TileList.ForEach(x => x.Visible = true);
        }

    }
}
