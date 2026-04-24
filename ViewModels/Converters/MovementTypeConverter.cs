using inventoryManager.Models;  // Исправлено: маленькая i
using System;
using System.Globalization;
using System.Windows.Data;

namespace inventoryManager.ViewModels.Converters  // Исправлено: маленькая i
{
    public class MovementTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MovementType type)
            {
                return type == MovementType.In ? "ПРИХОД" : "РАСХОД";
            }
            return "НЕИЗВЕСТНО";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? str = value as string;  // Исправлено: допускает null
            if (str == "ПРИХОД") return MovementType.In;
            if (str == "РАСХОД") return MovementType.Out;
            return MovementType.In;
        }
    }
}