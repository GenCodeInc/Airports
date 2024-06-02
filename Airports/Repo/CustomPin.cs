namespace Airports;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Airports.Models;

public class CustomPin : Pin
{
    public static readonly BindableProperty MainPageStatusBarProperty =
    BindableProperty.Create(nameof(MainPageStatusBar), typeof(MainPageStatusBar), typeof(CustomPin));
    public MainPageStatusBar? MainPageStatusBar
    {
        get => (MainPageStatusBar?)GetValue(MainPageStatusBarProperty);
        set => SetValue(MainPageStatusBarProperty, value);
    }

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(CustomPin));

    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
}
