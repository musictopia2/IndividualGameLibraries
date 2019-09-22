using AggravationCP;
using BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses;
using CommonBasicStandardLibraries.Exceptions;
using static BasicGameFramework.StandardImplementations.CrossPlatform.DataClasses.GlobalScreenClass;
namespace AggravationXF
{
    public class BoardPosition : IBoardPosition
    {
        int IBoardPosition.GetDiffTopEdges(int space) //done
        {
            if (ScreenUsed == EnumScreen.SmallPhone)
            {
                switch (space)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return -2;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        return 2;
                    case 5:
                    case 16:
                        return 8;
                    case 6:
                    case 15:
                        return 3;
                    case 7:
                    case 14:
                        return -3;
                    case 8:
                    case 13:
                        return -8;
                    default:
                        return 0;
                }
            }
            switch (space)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    return -4;
                case 9:
                case 10:
                case 11:
                case 12:
                    return 4;
                case 5:
                case 16:
                    return 18;
                case 6:
                case 15:
                    return 8;
                case 7:
                case 14:
                    return -8;
                case 8:
                case 13:
                    return -18;
                default:
                    return 0;
            }
        }
        int IBoardPosition.GetDiffTopHome(int id) //done.
        {
            if (ScreenUsed == EnumScreen.LargeTablet)
            {
                return id switch
                {
                    1 => 19,
                    2 => 29,
                    3 => 39,
                    4 => 49,
                    -1 => -19,
                    -2 => -29,
                    -3 => -39,
                    -4 => -49,
                    _ => throw new BasicBlankException("Only 1 to 4 or -1 to -4 is supported for home coordinates")
                };
            }
            return id switch
            {
                1 => 9,
                2 => 13,
                3 => 17,
                4 => 21,
                -1 => -9,
                -2 => -13,
                -3 => -17,
                -4 => -21,
                _ => throw new BasicBlankException("Only 1 to 4 or -1 to -4 is supported for home coordinates")
            };
        }
        int IBoardPosition.GetDiffTopMiddle(int row) //focus on one alone.  once that is figured out, then rest should be easy since distance is the same.
        {
            if (ScreenUsed == EnumScreen.LargeTablet)
            {
                return row switch
                {
                    1 => 15,
                    2 => 25,
                    3 => 35,
                    4 => 45,
                    5 => 55,
                    6 => 65,
                    -1 => -15,
                    -2 => -25,
                    -3 => -35,
                    -4 => -45,
                    -5 => -55,
                    -6 => -65,
                    _ => throw new BasicBlankException("Only 1 to 7 or -1 to -7 is supported for home coordinates")
                };
            }
            return row switch //no support for small tablets anymore.
            {
                1 => 7,
                2 => 11,
                3 => 15,
                4 => 19,
                5 => 23,
                6 => 27,
                -1 => -7,
                -2 => -11,
                -3 => -15,
                -4 => -19,
                -5 => -23,
                -6 => -27,
                _ => throw new BasicBlankException("Only 1 to 4 or -1 to -4 is supported for home coordinates")
            };
        }
    }
}