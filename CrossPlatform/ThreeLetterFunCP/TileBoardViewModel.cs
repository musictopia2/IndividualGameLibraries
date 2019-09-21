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
using BasicGameFramework.DrawableListsViewModels;
using BasicGameFramework.ViewModelInterfaces;

namespace ThreeLetterFunCP
{
    public class TileBoardViewModel : HandViewModel<TileInformation>
    {

        public void UpdateBoard()
        {
            if (MainGame.SaveRoot!.TileList.Count == 0)
            {
                HandList.Clear();
                return;
            }
            CustomBasicList<TileInformation> ThisList = new CustomBasicList<TileInformation>
                { MainGame.SaveRoot.TileList.First(), MainGame.SaveRoot.TileList[1] };
            HandList.ReplaceRange(ThisList);
        }
        public TileInformation? GetTile(bool IsSelected)
        {
            if (IsSelected == true)
            {
                if (HandList.First().IsSelected == true)
                    return HandList.First();
                else if (HandList[1].IsSelected == true)
                    return HandList[1];
                else
                    return null;
                //throw new BasicBlankException("Must have a selected tile");
            }
            else if (HandList.First().IsSelected == false)
                return HandList.First();
            else if (HandList[1].IsSelected == false)
                return HandList[1];
            else
                return null;
                //throw new BasicBlankException("Must have a tile that is not selected");
        }

        private readonly ThreeLetterFunMainGameClass MainGame;
        public TileBoardViewModel(IBasicGameVM ThisMod) : base(ThisMod)
        {
            AutoSelect = EnumAutoType.SelectOneOnly;
            Visible = true;
            MainGame = ThisMod.MainContainer!.Resolve<ThreeLetterFunMainGameClass>();
            //IsEnabled = true; //i think.  if i am wrong, rethink.
        }
        public void UnDo()
        {
            UnselectAllObjects();
            MainGame.SaveRoot!.TileList.ForEach(Items => Items.Visible = true);
        }
    }
}