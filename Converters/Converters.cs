using System.Globalization;
using System.Windows.Data;
using BasicApps.Models;

namespace BasicApps.Converters;

public sealed class BoolToOpacityConverter : IValueConverter
{
    public double TrueValue { get; set; } = 1.0;
    public double FalseValue { get; set; } = 0.5;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && b ? TrueValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class TileTypeToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TileType type) return "Question";
        
        return type switch
        {
            TileType.AudioMicMute => "Mic",
            TileType.AudioOutputSwitcher => "Speaker",
            TileType.DisplayBrightness => "Sun",
            TileType.DisplayRefreshRate => "Monitor",
            TileType.DisplayHdrToggle => "Hdr",
            TileType.SystemKeepAwake => "Coffee",
            TileType.SystemProcessKiller => "Close",
            _ => "Cog"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}