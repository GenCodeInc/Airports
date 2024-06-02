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

    Location? userLocation = null;

    public MainPage(MainPageViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
        InitializeMap();

        ((App)Application.Current).AppActivated += async (s, e) => await OnAppActivated();

        CommonData.Logging.LoggingEvent += OnLoggingEvent;

        CommonData.Logging.Write("MainPage Constructor Called", System.Diagnostics.TraceLevel.Verbose);
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
                    // Sends event...event fires fine...but the InfoWindow doesn't popup.
                    ((CustomPin)pin).SendMarkerClick();

                    // Constants
                    const double milesToLatitudeDegrees = 69.0;
                    const double milesToLongitudeDegrees = 69.0;

                    // Calculate the spans
                    double spanLat = 10 / milesToLatitudeDegrees;
                    double spanLon = 10 / (milesToLongitudeDegrees * Math.Cos(pin.Location.Latitude * Math.PI / 180));

                    // Create the MapSpan
                    var mapSpan = new MapSpan(new Location(pin.Location.Latitude, pin.Location.Longitude), spanLat, spanLon);
                    MainMap.MoveToRegion(mapSpan);
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


    //private Map MainMap;

    private void InitializeMap()
    {
        //MainMap = new Map
        //{
        //    IsShowingUser = true,
        //    MapType = MapType.Hybrid,
        //    VerticalOptions = LayoutOptions.FillAndExpand,
        //    ItemsSource = (BindingContext as MainPageViewModel)?.LocationPins
        //};

        //MainMap.ItemTemplate = new DataTemplate(() =>
        //{
        //    var customPin = new CustomPin();
        //    customPin.ShowInfoWindow = true;
        //    customPin.SetBinding(CustomPin.LocationProperty, "Location");
        //    customPin.SetBinding(CustomPin.AddressProperty, "Address");
        //    customPin.SetBinding(CustomPin.LabelProperty, "Description");
        //    customPin.SetBinding(CustomPin.ImageSourceProperty, "ImageSource");
        //    return customPin;
        //});

        //// Add the map to the grid
        //if (Content is Grid grid)
        //{
        //    grid.Children.Insert(0, MainMap); // Insert the map at the first position
        //}
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
    /// Updates the status bar with information.
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
                double airportMinLat = airports.Min(c => c.Latitude);
                double airportMaxLat = airports.Max(c => c.Latitude);
                double airportMinLon = airports.Min(c => c.Longitude);
                double airportMaxLon = airports.Max(c => c.Longitude);

                double centerLat = (airportMinLat + airportMaxLat) / 2;
                double centerLon = (airportMinLon + airportMaxLon) / 2;
                double distanceLat = airportMaxLat - airportMinLat;
                double distanceLon = airportMaxLon - airportMinLon;

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
    /// Adds a pin for the on the map.
    /// </summary>
    private async Task AddAirportPinAsync(Airport airport, string label = "")
    {
        try
        {
            CommonData.Logging.Write("AddAirportPinAsync");

            var airportIcon = "Airports.Resources.EmbeddedImages.airport.png";
            var airportLocation = new Location(airport.Latitude, airport.Longitude);

            CustomPin pin = new CustomPin
            {
                MainPageStatusBar = new MainPageStatusBar
                {
                    AirportName = airport.AirportName,
                    City = airport.City,
                    State = airport.State,
                    IATACode = airport.IATACode
                },
                Label = airport.AirportName + " (" + airport.City + " " + airport.State + ")",

                Location = new Location(airport.Latitude, airport.Longitude),
                Address = "Lat: " + airportLocation.Latitude.ToString("F2") +
                          " Lon: " + airportLocation.Longitude.ToString("F2"),
                ImageSource = ImageSource.FromResource(airportIcon)
            };

            pin.MarkerClicked += Pin_MarkerClicked;
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

    private void Pin_MarkerClicked(object? sender, EventArgs e)
    {
    CommonData.Logging.Write("Pin_MarkerClicked");

        var pin = (CustomPin)sender;


        UpdateStatusBar(pin.MainPageStatusBar.AirportName,
                        pin.MainPageStatusBar.City,
                        pin.MainPageStatusBar.IATACode,
                        pin.MainPageStatusBar.State,
                        pin.MainPageStatusBar.Distance);
    }

    /// <summary>
    /// Calculates and displays the distance from the user's location to the location.
    /// </summary>
    public async Task<string> CalculateAndDisplayDistance(Location location)
    {
        return await GetDistanceFromFixedPoint(location);
    }

    /// <summary>
    /// Calculates the distance from a fixed point (user's location) to the location.
    /// </summary>
    public async Task<string> GetDistanceFromFixedPoint(Location airportLocation)
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

            double distance = Location.CalculateDistance(airportLocation, userLocation, DistanceUnits.Miles);
            return $"{distance:F2} miles away.";
        }
        catch (Exception ex)
        {
            CommonData.Logging.Write(ex, System.Diagnostics.TraceLevel.Warning);
            return "Unable to determine your current location.";
        }
    }
}
