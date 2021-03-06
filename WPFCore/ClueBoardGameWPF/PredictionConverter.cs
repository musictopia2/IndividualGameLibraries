﻿using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ClueBoardGameWPF
{
    public class PredictionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value.Equals(parameter))
                    return Brushes.LimeGreen;
                return Brushes.Aqua;
            }
            catch
            {
                return Brushes.Aqua;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}