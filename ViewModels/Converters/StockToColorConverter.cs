using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace inventoryManager.ViewModels.Converters  // Исправлено: маленькая i
{
    public class StockToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                if (quantity == 0)
                    return new SolidColorBrush(Color.FromRgb(220, 53, 69));
                if (quantity <= 5)
                    return new SolidColorBrush(Color.FromRgb(255, 193, 7));
                if (quantity <= 10)
                    return new SolidColorBrush(Color.FromRgb(23, 162, 184));
                return new SolidColorBrush(Color.FromRgb(40, 167, 69));
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}