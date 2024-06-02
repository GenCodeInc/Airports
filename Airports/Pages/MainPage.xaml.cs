namespace Airports.Pages;
using Airports;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Maps;
using Airports.Models;
using Microsoft.Maui.Controls.Maps;

public partial class MainPage : ContentPage
{
    private string _windSpeed = "-";
    private string _direction = "-";
    private string _pressure = "-";
    private string _distance = "-";
    private AirportService? airportService = null;

    public string WindSpeed
    {
        get => _windSpeed;
        set
        {
            _windSpeed = value;
            OnPropertyChanged();
        }
    }

    public string Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            OnPropertyChanged();
        }
    }

    public string Pressure
    {
        get => _pressure;
        set
        {
            _pressure = value;
            OnPropertyChanged();
        }
    }

    public string Distance
    {
        get => _distance;
        set
        {
            _distance = value;
            OnPropertyChanged();
        }
    }

    Location? userLocation = null;

    public MainPage(MainPageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
        InitializeMap();

        ((App)Application.Current).AppActivated += async (s, e) => await OnAppActivated();

        CommonData.Logging.LoggingEvent += OnLoggingEvent;

        CommonData.Logging.Write("MainPage Constructor Called", System.Diagnostics.TraceLevel.Verbose);
        var customMapHandler = new CustomMapHandler();
    }

    /// <summary>
    /// Event handler for logging events.
    /// </summary>
    private void OnLoggingEvent(object? sender, LogEventArgs e)
    {
        //if(e.Level <= System.Diagnostics.TraceLevel.Warning)
        //    AddLogMessage(e.Message);
    }

    private async void OnClickAirportButtonClicked(object sender, EventArgs e)
    {
        foreach (var pin in MainMap.Pins.ToList())
        {
            if (pin is CustomPin)
            {
                if (pin.Label.Contains("Miami"))
                {
                    ((CustomPin)pin).SendMarkerClick();
                    
                    break;
                }
            }
        }
    }

    private void ForceUpdateMap()
    {
        CommonData.Logging.Write("-----------------> ForceUpdateMap");

        // Adjust the map's size to force a refresh
        var originalWidth = MainMap.Width;
        var originalHeight = MainMap.Height;
        MainMap.WidthRequest = originalWidth + 1;
        MainMap.HeightRequest = originalHeight + 1;
        MainMap.WidthRequest = originalWidth;
        MainMap.HeightRequest = originalHeight;

        (MainMap.Parent as IView)?.InvalidateMeasure();
        (MainMap.Parent as IView)?.InvalidateArrange();
    }

    /// <summary>
    /// Event handler for when the application is activated.
    /// </summary>
    private async Task OnAppActivated()
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await DrawAirportsOnMapAsync();
    }


    private Map MainMap;

    private void InitializeMap()
    {
        MainMap = new Map
        {
            IsShowingUser = true,
            MapType = MapType.Hybrid,
            VerticalOptions = LayoutOptions.FillAndExpand,
            ItemsSource = (BindingContext as MainPageViewModel)?.LocationPins
        };

        MainMap.ItemTemplate = new DataTemplate(() =>
        {
            var customPin = new CustomPin();
            customPin.ShowInfoWindow = true;
            customPin.SetBinding(CustomPin.LocationProperty, "Location");
            customPin.SetBinding(CustomPin.AddressProperty, "Address");
            customPin.SetBinding(CustomPin.LabelProperty, "Description");
            customPin.SetBinding(CustomPin.ImageSourceProperty, "ImageSource");
            return customPin;
        });

        // Add the map to the grid
        if (Content is Grid grid)
        {
            grid.Children.Insert(0, MainMap); // Insert the map at the first position
        }
    }

    /// <summary>
    /// Displays a toast message.
    /// </summary>
    private void ShowToast(string message = "")
    {
        var toast = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long, 12);
        toast.Show();
    }

    /// <summary>
    /// Asynchronously draws airports on the map.
    /// </summary>
    private async Task DrawAirportsOnMapAsync()
    {
        try
        {

            if(airportService == null)
            { 
                CommonData.Logging.Write("AirportService");
                airportService = new AirportService();

                if (airportService.Airports.Count == 0) 
                { 
                    CommonData.Logging.Write("LoadAirportsAsync");
                    await airportService.LoadAirportsAsync();
                }
            }

            // zoom over first
            ZoomToRegion(airportService.Airports);

            foreach (var airport in airportService.Airports)
            {
                CommonData.Logging.Write(airport.AirportName);
                await AddAirportPinAsync(airport, string.Empty);
            }
        }
        catch (Exception ex)
        {
            CommonData.Logging.Write(ex, System.Diagnostics.TraceLevel.Error);
            ShowToast("Error drawing airports on the map.");
        }
    }

    /// <summary>
    /// Updates the status bar with storm information.
    /// </summary>
    public void UpdateStatusBar(string title = "", string city = "", string iATACodeLabel = "", string state = "", string distance = "")
    {
        AirportNameLabel.Text = title;
        CityLabel.Text = city;
        IATACodeLabel.Text = iATACodeLabel;
        StateLabel.Text = state;
        DistanceLabel.Text = distance;
    }

    /// <summary>
    /// Zooms the map to fit the region of the airports.
    /// </summary>
    private void ZoomToRegion(List<Airport> airports)
    {
        foreach (var airport in airports)
        {
            try
            {
                double stormMinLat = airports.Min(c => c.Latitude);
                double stormMaxLat = airports.Max(c => c.Latitude);
                double stormMinLon = airports.Min(c => c.Longitude);
                double stormMaxLon = airports.Max(c => c.Longitude);

                double centerLat = (stormMinLat + stormMaxLat) / 2;
                double centerLon = (stormMinLon + stormMaxLon) / 2;
                double distanceLat = stormMaxLat - stormMinLat;
                double distanceLon = stormMaxLon - stormMinLon;

                // Add padding around the edges
                double paddingFactor = 1.2;
                double spanLat = distanceLat * paddingFactor;
                double spanLon = distanceLon * paddingFactor;

                // Set the map to the calculated region
                var mapSpan = new MapSpan(new Location(centerLat, centerLon), spanLat, spanLon);
                MainMap.MoveToRegion(mapSpan);

            }
            catch (Exception ex)
            {
                CommonData.Logging.Write(ex);
            }
        }
    }
    /// <summary>
    /// Adds a pin for the storm on the map.
    /// </summary>
    private async Task AddAirportPinAsync(Airport airport, string label = "")
    {
        try
        {
            CommonData.Logging.Write("AddAirportPinAsync");

            var stormIcon = "Airports.Resources.EmbeddedImages.airport.png";
            var stormLocation = new Location(airport.Latitude, airport.Longitude);

            var pin = new CustomPin
            {
                ShowInfoWindow = true,

                MainPageStatusBar = new MainPageStatusBar
                {
                    AirportName = airport.AirportName,
                    City = airport.City,
                    State = airport.State,
                    IATACode = airport.IATACode
                },
                Label = airport.AirportName + " (" + airport.City + " " + airport.State + ")",

                Location = new Location(airport.Latitude, airport.Longitude),
                Address = "Lat: " + stormLocation.Latitude.ToString("F1") +
                          " Lon: " + stormLocation.Longitude.ToString("F1"),
                ImageSource = ImageSource.FromResource(stormIcon)
            };

            pin.MarkerClicked += Pin_MarkerClicked1;
            pin.InfoWindowClicked += Pin_InfoWindowClicked;
            MainMap.Pins.Add(pin);

        }
        catch (Exception ex)
        {
            CommonData.Logging.Write(ex, System.Diagnostics.TraceLevel.Error);
        }
    }

    private void Pin_InfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        CommonData.Logging.Write("Pin_InfoWindowClicked");
    }

    private void Pin_MarkerClicked1(object? sender, EventArgs e)
    {

        CommonData.Logging.Write("Pin_MarkerClicked");

        var pin = (CustomPin)sender;
        pin.ShowInfoWindow = true;

        UpdateStatusBar(pin.MainPageStatusBar.AirportName,
                        pin.MainPageStatusBar.City,
                        pin.MainPageStatusBar.IATACode,
                        pin.MainPageStatusBar.State,
                        pin.MainPageStatusBar.Distance);
    }

    /// <summary>
    /// Calculates and displays the distance from the user's location to the storm location.
    /// </summary>
    public async Task<string> CalculateAndDisplayDistance(Location location)
    {
        return await GetDistanceFromFixedPoint(location);
    }

    /// <summary>
    /// Calculates the distance from a fixed point (user's location) to the storm location.
    /// </summary>
    public async Task<string> GetDistanceFromFixedPoint(Location stormLocation)
    {
        try
        {
            if (userLocation == null)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    userLocation = location;
                }
            }

            double distance = Location.CalculateDistance(stormLocation, userLocation, DistanceUnits.Miles);
            return $"{distance:F2} miles away.";
        }
        catch (Exception ex)
        {
            CommonData.Logging.Write(ex, System.Diagnostics.TraceLevel.Warning);
            return "Unable to determine your current location.";
        }
    }
}
