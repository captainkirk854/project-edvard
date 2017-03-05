namespace UX.Convertors
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Swiss Army Knife for Bindings
    /// </summary>
    /// <remarks>
    ///  Can be used within bindings and/or in code-behind to give more concise property setters
    ///  Adapted from: https://www.codeproject.com/articles/92944/a-universal-value-converter-for-wpf
    ///   Sample Usage:
    ///     xmlns:localconv="clr-namespace:UX.Convertors"
    ///     UserControl.Resources
    ///       localconv:UniversalValueConverter x:Key="SuperConv"
    ///     UserControl.Resources
    ///    e.g. 1
    ///     TextBox  x:Name="inputcolorTextBox" Text="Red"  Margin="359,165,364,9"
    ///     Rectangle Fill="{Binding ElementName=inputcolorTextBox, Path=Text, Converter={StaticResource SuperConv}}" Margin="159,165,486,10"
    ///    e.g. 2
    ///     TextBox x:Name="geometryText" Text="M 100,20 C 10,2.5 40,35 40,17 H 28" Margin="1,165,686,10"
    ///     Path Data="{Binding ElementName=geometryText, Path=Text, Converter={StaticResource SuperConv}}" Height="1170" Stretch="Fill" Width="2280"
    ///    e.g. 3
    ///     TextBox x:Name="dashText" Text="2 2 4 5" Margin="359,140,328,35"
    ///     Line StrokeDashArray="{Binding ElementName=dashText, Path=Text, Converter={StaticResource SuperConv}}"
    ///    e.g. 4
    ///     var conv = new UniversalValueConverter(); 
    ///     var convertedValue = conv.Convert(value, property.PropertyType, null, CultureInfo.InvariantCulture);
    ///     element.SetValue(property, convertedValue);
    /// </remarks>
    public class ValueToType : IValueConverter
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
        /// Simple IValueConverter implementation making use of framework type 
        /// converters to allow conversion between a large range of source/target types.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private object ConvertValueToType(object value, Type targetType)
        {
            // obtain converter for target type ..
            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                // determine if supplied value is of a suitable type ..
                if (converter.CanConvertFrom(value.GetType()))
                {
                    // return converted value ..
                    return converter.ConvertFrom(value);
                }
                else
                {
                    // try to convert from string representation ..
                    return converter.ConvertFrom(value.ToString());
                }
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}