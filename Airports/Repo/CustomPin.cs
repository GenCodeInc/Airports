namespace Airports;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Airports.Models;

public interface ICustomPin
{
    event EventHandler MyMapClick;
    void OnMyMapClick();
    /// <summary>
    /// The physical address that is associated with this pin.
    /// </summary>
    string Address { get; }

    /// <summary>
    /// The label that is shown for this pin.
    /// </summary>
    string Label { get; }

    /// <summary>
    /// The geographical location of this pin.
    /// </summary>
    Location Location { get; }

    object? MarkerId { get; set; }

    bool SendMarkerClick();

    bool SendInfoWindowClick();
}

public class CustomPin : Pin, ICustomPin, IElement
{
    public bool ShowInfoWindow { get; set; } = true;

    public void SendMarkerClick()
    {
        // Trigger the MarkerClicked event
        MarkerClicked?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler MarkerClicked;

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

    // Implement the event and method from the interface
    public event EventHandler? MyMapClick;

    public void OnMyMapClick()
    {
        MyMapClick?.Invoke(this, EventArgs.Empty);
    }
}
