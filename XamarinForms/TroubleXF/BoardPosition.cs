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
using TroubleCP;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using SkiaSharp;
namespace TroubleXF
{
    public class BoardPosition : IBoardPosition
    {
        int IBoardPosition.CalculateDiffTopRegular(int space, int recommendedAmount)
        {
            //try the old way.
            //if old way does not work, do new way.
            if (ScreenUsed == EnumScreen.LargeTablet)
            {
                return space switch
                {
                    27 => 0, 
                    41 => 0,
                    1 => 18,//blue/yellow start
                    16 => 18,
                    2 => 30,
                    15 => 30,
                    3 => 40,
                    14 => 40,
                    4 => 48,
                    13 => 48,
                    5 => -16, //green/red starts
                    12 => -16,
                    6 => -24,
                    11 => -24,
                    7 => -32,
                    10 => -32,
                    8 => -40,
                    9 => -40,
                    53 => 46, //blue yellow home
                    57 => 46,
                    54 => 56,
                    58 => 56,
                    55 => 65,
                    59 => 65,
                    56 => 74,
                    60 => 74,
                    49 => -40, //green/red home
                    45 => -40,
                    50 => -48,
                    46 => -48,
                    51 => -56,
                    47 => -56,
                    52 => -64,
                    48 => -64,
                    32 => 22, //top row.
                    33 => 22,
                    34 => 22,
                    35 => 22,
                    36 => 22,
                    18 => -20, //bottom row.
                    19 => -20,
                    20 => -20,
                    21 => -20,
                    22 => -20,
                    29 => -25, //row 1
                    39 => -25,
                    28 => -10, //row 2
                    40 => -10,
                    26 => 14, //row 4
                    42 => 14,
                    25 => 30, //row 5
                    43 => 30,
                    30 => 50, //d2
                    38 => 50,
                    31 => 36, //d1
                    37 => 36,
                    24 => -46, //d3
                    44 => -46,
                    23 => -30, //d4
                    17 => -30,
                    _ => 0
                };
            }
            return space switch
            {
                27 => 0,
                41 => 0,
                1 => 8,//blue/yellow start
                16 => 8,
                2 => 14,
                15 => 14,
                3 => 18,
                14 => 18,
                4 => 22,
                13 => 22,
                5 => -9, //green/red starts
                12 => -9,
                6 => -13,
                11 => -13,
                7 => -17,
                10 => -17, //start with start for phone now.
                8 => -21,
                9 => -21,
                53 => 22, //blue yellow home
                57 => 22,
                54 => 27,
                58 => 27,
                55 => 32,
                59 => 32,
                56 => 37,
                60 => 37,
                49 => -18, //green/red home
                45 => -18,
                50 => -22,
                46 => -22,
                51 => -26,
                47 => -26,
                52 => -30,
                48 => -30,
                32 => 10, //top row.
                33 => 10,
                34 => 10,
                35 => 10,
                36 => 10,
                18 => -9, //bottom row.
                19 => -9,
                20 => -9,
                21 => -9,
                22 => -9,
                29 => -12, //row 1
                39 => -12,
                28 => -4, //row 2
                40 => -4,
                26 => 6, //row 4
                42 => 6,
                25 => 14, //row 5
                43 => 14,
                30 => 24, //d2
                38 => 24,
                31 => 16, //d1
                37 => 16,
                24 => -22, //d3
                44 => -22,
                23 => -14, //d4
                17 => -14,
                _ => 0
            };
        }
        SKPoint IBoardPosition.RecommendedPointForDice(SKPoint pt_Center, float actualWidth, float actualHeight)
        {
            if (ScreenUsed == EnumScreen.SmallPhone)
                return new SKPoint(pt_Center.X - (actualWidth / 4.1f), pt_Center.Y - (actualHeight / 4.9f));
            return new SKPoint(pt_Center.X - (actualWidth / 4.5f), pt_Center.Y - (actualHeight / 4.7f));
        }
    }
}
