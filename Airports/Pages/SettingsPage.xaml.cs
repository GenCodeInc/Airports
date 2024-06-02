using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;

namespace Airports.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private const string MapTypeKey = "MapType";
        private const string SpeedUnitsKey = "SpeedUnits";
        private const string PressureUnitsKey = "PressureUnits";

        public SettingsPage()
        {
            InitializeComponent();

            // Set default values if preferences are not set
            if (!Preferences.ContainsKey(MapTypeKey))
            {
                Preferences.Set(MapTypeKey, "Hybrid");
            }

            if (!Preferences.ContainsKey(SpeedUnitsKey))
            {
                Preferences.Set(SpeedUnitsKey, "mph");
            }

            if (!Preferences.ContainsKey(PressureUnitsKey))
            {
                Preferences.Set(PressureUnitsKey, "Millibar (mb)");
            }

            // Initialize UI with preferences
            MapTypePicker.SelectedItem = Preferences.Get(MapTypeKey, "Hybrid");
            SpeedUnitsPicker.SelectedItem = Preferences.Get(SpeedUnitsKey, "mph");
            PressureUnitsPicker.SelectedItem = Preferences.Get(PressureUnitsKey, "Millibar (mb)");

            // Handle selection changes
            MapTypePicker.SelectedIndexChanged += OnMapTypeSelectedIndexChanged;
            SpeedUnitsPicker.SelectedIndexChanged += OnSpeedUnitsSelectedIndexChanged;
            PressureUnitsPicker.SelectedIndexChanged += OnPressureUnitsSelectedIndexChanged;
        }

        private void OnMapTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (MapTypePicker.SelectedItem != null)
            {
                Preferences.Set(MapTypeKey, MapTypePicker.SelectedItem.ToString());
            }
        }

        private void OnSpeedUnitsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (SpeedUnitsPicker.SelectedItem != null)
            {
                Preferences.Set(SpeedUnitsKey, SpeedUnitsPicker.SelectedItem.ToString());
            }
        }

        private void OnPressureUnitsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (PressureUnitsPicker.SelectedItem != null)
            {
                Preferences.Set(PressureUnitsKey, PressureUnitsPicker.SelectedItem.ToString());
            }
        }
    }
}
