namespace UX.Convertors
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class BooleanToColour : IValueConverter
    {
        /// <summary>
        /// Perform Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertValueToType(value, targetType);
        }

        /// <summary>
        /// Perform ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert Value to Type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object ConvertValueToType(object value, Type targetType)
        {
            if (value is bool)
            {
                return ((bool)value) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Goldenrod);
            }

            return value;
        }
    }
}