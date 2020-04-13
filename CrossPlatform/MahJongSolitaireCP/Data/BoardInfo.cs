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
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BaseMahjongTilesCP;
//i think this is the most common things i like to do
namespace MahJongSolitaireCP.Data
{
    public class BoardInfo : ObservableObject
    {
        public enum EnumBoardCategory
        {
            FarLeft = 1,
            Regular = 2,
            FarRight = 3,
            VeryTop = 4
        }
        public int Floor { get; set; }
        public int RowStart { get; set; }
        public int HowManyColumns { get; set; }
        public int FrontTaken { get; set; }
        public int BackTaken { get; set; }
        public bool Enabled { get; set; } = false; // i think this is needed as well
        private DeckObservableDict<MahjongSolitaireTileInfo> _tileList = new DeckObservableDict<MahjongSolitaireTileInfo>();

        public DeckObservableDict<MahjongSolitaireTileInfo> TileList
        {
            get { return _tileList; }
            set
            {
                if (SetProperty(ref _tileList, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public EnumBoardCategory BoardCategory { get; set; } = EnumBoardCategory.Regular; // most are regular
        public int ColumnStart { get; set; }
    }
}
