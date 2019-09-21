using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
namespace ClueBoardGameCP
{
    public class WeaponInfo : ObservableObject
    {
        public string Name { get; set; } = ""; //this has to be repeated now though.
        public int Room { get; set; } // this is the room number
        private EnumWeaponList _Weapon;
        public EnumWeaponList Weapon
        {
            get
            {
                return _Weapon;
            }
            set
            {
                if (SetProperty(ref _Weapon, value) == true)
                {
                }
            }
        }
        public SKSize DefaultSize
        {
            get
            {
                if (Weapon == EnumWeaponList.None)
                    return new SKSize(55, 72); //default
                switch (Weapon)
                {
                    case EnumWeaponList.Candlestick:
                        {
                            return new SKSize(25, 35);
                        }

                    case EnumWeaponList.Knife:
                        {
                            return new SKSize(25, 22);
                        }

                    case EnumWeaponList.LeadPipe:
                        {
                            return new SKSize(15, 37);
                        }

                    case EnumWeaponList.Revolver:
                        {
                            return new SKSize(25, 15);
                        }

                    case EnumWeaponList.Rope:
                        {
                            return new SKSize(25, 20);
                        }

                    case EnumWeaponList.Wrench:
                        {
                            return new SKSize(25, 28);
                        }

                    default:
                        {
                            throw new BasicBlankException("Not supported");
                        }
                }
            }
        }
    }
}