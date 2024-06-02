using Airports.Models;

namespace Airports;

public partial class App : Application
{
    public event EventHandler AppActivated;

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Created += (s, e) =>
        {
            CommonData.Logging.Write("-----------------> Created");

        };

        window.Activated += (s, e) =>
        {
            CommonData.Logging.Write("-----------------> Activated");

            OnAppActivated(EventArgs.Empty);
        };

        window.Backgrounding += (s, e) =>
        {
            CommonData.Logging.Write("-----------------> Backgrounding");
        };

        window.Resumed += (s, e) =>
        {
            CommonData.Logging.Write("-----------------> Resumed");
        };

        window.Stopped += (s, e) =>
        {
            CommonData.Logging.Write("-----------------> Stopped");
        };

        return window;
    }

    protected virtual void OnAppActivated(EventArgs e)
    {
        AppActivated?.Invoke(this, e);
    }
}
