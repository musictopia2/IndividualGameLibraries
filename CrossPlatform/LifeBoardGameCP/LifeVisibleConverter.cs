using CommonBasicStandardLibraries.CommonConverters;
using CommonBasicStandardLibraries.Exceptions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace LifeBoardGameCP
{
    public abstract class LifeVisibleConverter : EnumVisibleConverter<EnumWhatStatus>
    {
        private EnumWhatStatus GameStatus => _mainGame.GameStatus;
        public EnumVisibleCategory Category { get; set; }
        public bool IsOpposites { get; set; } = false;
        private readonly LifeBoardGameMainGameClass _mainGame;
        public LifeVisibleConverter()
        {
            _mainGame = Resolve<LifeBoardGameMainGameClass>();
        }
        protected override bool Convert(EnumWhatStatus enumSent, EnumWhatStatus parameter)
        {
            if (enumSent == EnumWhatStatus.None)
                return false;
            if (enumSent == EnumWhatStatus.NeedChooseGender && Category == EnumVisibleCategory.Gender && IsOpposites == false)
                return true;
            if (enumSent == EnumWhatStatus.NeedChooseGender || enumSent == EnumWhatStatus.None)
            {
                if (IsOpposites == false)
                    return false;
            }
            if (enumSent == EnumWhatStatus.NeedChooseGender && IsOpposites && Category == EnumVisibleCategory.Gender)
                return false;
            else if (enumSent == EnumWhatStatus.NeedChooseGender && IsOpposites)
                return true;
            switch (Category)
            {
                case EnumVisibleCategory.Gender:
                    return IsOpposites;
                case EnumVisibleCategory.GameBoard:
                    if (_mainGame.ThisData!.IsXamarinForms == false)
                        return true;
                    if (GameStatus == EnumWhatStatus.NeedChooseSalary || GameStatus == EnumWhatStatus.NeedTradeSalary || GameStatus == EnumWhatStatus.NeedStealTile || GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedSellBuyHouse || GameStatus == EnumWhatStatus.NeedChooseFirstCareer || GameStatus == EnumWhatStatus.NeedChooseStock || GameStatus == EnumWhatStatus.NeedNewCareer || GameStatus == EnumWhatStatus.NeedReturnStock)
                        return false;
                    return true;
                case EnumVisibleCategory.SpinButton:
                    if (GameStatus == EnumWhatStatus.NeedChooseHouse  || GameStatus == EnumWhatStatus.NeedSellHouse || GameStatus == EnumWhatStatus.NeedSellBuyHouse)
                        return true;
                    return false;
                case EnumVisibleCategory.Spinner:
                    if (enumSent == EnumWhatStatus.MakingMove)
                        return true;
                    if (_mainGame.ThisData!.IsXamarinForms)
                    {
                        return GameStatus == EnumWhatStatus.NeedToSpin || GameStatus == EnumWhatStatus.NeedFindSellPrice;
                    }
                    if (GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedToSpin || GameStatus == EnumWhatStatus.NeedFindSellPrice || GameStatus == EnumWhatStatus.NeedSellHouse || GameStatus == EnumWhatStatus.NeedSellBuyHouse)
                        return true;
                    return false;
                case EnumVisibleCategory.SubmitOption:
                    if (GameStatus == EnumWhatStatus.NeedNewCareer || GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedReturnStock || GameStatus == EnumWhatStatus.NeedChooseFirstCareer || GameStatus == EnumWhatStatus.NeedChooseSalary || GameStatus == EnumWhatStatus.NeedChooseStock)
                        return true;
                    return false;
                case EnumVisibleCategory.Scoreboard:
                    if (GameStatus == EnumWhatStatus.NeedNewCareer || GameStatus == EnumWhatStatus.NeedChooseHouse || GameStatus == EnumWhatStatus.NeedReturnStock || GameStatus == EnumWhatStatus.NeedChooseFirstCareer || GameStatus == EnumWhatStatus.NeedChooseSalary || GameStatus == EnumWhatStatus.NeedChooseStock)
                        return false;
                    if (GameStatus == EnumWhatStatus.NeedStealTile || GameStatus == EnumWhatStatus.NeedTradeSalary)
                        return false;
                    return true;
                case EnumVisibleCategory.PopUp:
                    return enumSent == EnumWhatStatus.NeedTradeSalary || enumSent == EnumWhatStatus.NeedStealTile;
                case EnumVisibleCategory.CardList:
                    return enumSent == EnumWhatStatus.NeedChooseSalary || enumSent == EnumWhatStatus.NeedChooseHouse || enumSent == EnumWhatStatus.NeedSellHouse || enumSent == EnumWhatStatus.NeedChooseFirstCareer || enumSent == EnumWhatStatus.NeedChooseStock || enumSent == EnumWhatStatus.NeedNewCareer || enumSent == EnumWhatStatus.NeedReturnStock || enumSent == EnumWhatStatus.NeedChooseSalary;
                default:
                    throw new BasicBlankException("Unable to figure out whether to make visible for life converter.  Rethink");
            }
        }
    }
}