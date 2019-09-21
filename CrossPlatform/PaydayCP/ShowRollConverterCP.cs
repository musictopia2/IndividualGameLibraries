using CommonBasicStandardLibraries.CommonConverters;
using System;
using System.Globalization;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace PaydayCP
{
    public abstract class ShowRollConverterCP : IConverterCP
    {
        PaydayViewModel? _thisMod;
        PaydayMainGameClass? _mainGame;
        public VisibleTranslation? VisibleDelegate;
        private object FinalResults(bool value)
        {
            if (VisibleDelegate == null)
                return value;
            return VisibleDelegate(value);
        }
        public object Convert(object value, Type TargetType, object Parameter, CultureInfo culture)
        {

            if (_thisMod == null)
                _thisMod = Resolve<PaydayViewModel>();
            if (_thisMod.MailPile == null)
                return FinalResults(false);
            if (_mainGame == null)
                _mainGame = Resolve<PaydayMainGameClass>();
            if (_mainGame.SaveRoot!.GameStatus == EnumStatus.ChooseDeal || _thisMod.MailPile.Visible == false || _thisMod.DealPile!.Visible == false)
                return FinalResults(false);
            return true;
        }
        public object ConvertBack(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DealConverterCP : IConverterCP //this is tablet only.
    {
        private PaydayMainGameClass? mainGame;
        public object Convert(object value, Type TargetType, object Parameter, CultureInfo culture) //has to be public or overrided does not work
        {
            if (mainGame == null)
                mainGame = Resolve<PaydayMainGameClass>();
            return mainGame.SaveRoot!.GameStatus == EnumStatus.ChooseBuy;
        }
        public object ConvertBack(object value, Type TargetType, object Parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}