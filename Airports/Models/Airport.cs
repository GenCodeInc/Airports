using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Airports.Models
{
    public class Airport
    {
        public string AirportName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string IATACode { get; set; }
        public string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AirportService
    {
        public List<Airport> Airports { get; private set; }

        public AirportService()
        {
            Airports = new List<Airport>();
        }

        public async Task LoadAirportsAsync()
        {
            try
            {
                CommonData.Logging.Write("LoadAirportsAsync");

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Airports.Resources.airports_large_usa.csv"; // Update the namespace and resource path accordingly

                // Write all embedded resources
                var resourceNames = assembly.GetManifestResourceNames();
                foreach (var name in resourceNames)
                {
                    CommonData.Logging.Write(name);
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                CommonData.Logging.Write("GetManifestResourceStream");
                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        CommonData.Logging.Write("Stream is null", TraceLevel.Error);
                        throw new ArgumentNullException("stream");
                    }

                    CommonData.Logging.Write("StreamReader");
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        CommonData.Logging.Write("CsvReader");
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            try
                            {
                                CommonData.Logging.Write("RegisterClassMap");
                                csv.Context.RegisterClassMap<AirportMap>();

                                CommonData.Logging.Write("GetRecords");
                                Airports = csv.GetRecords<Airport>().ToList();
                            }
                            catch (Exception ex)
                            {
                                CommonData.Logging.Write(ex);
                            }
                        }
                    }
                }

                stopwatch.Stop();

                CommonData.Logging.Write("Loaded");
                Console.WriteLine($"Loaded airports in {stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                CommonData.Logging.Write(ex.Message, TraceLevel.Error);
            }
        }

        public IEnumerable<Airport> GetNearbyAirports(double currentLatitude, double currentLongitude, double radius, bool inMiles = true)
        {
            double radiusInKm = inMiles ? radius * 1.60934 : radius;
            return Airports.Where(a => GetDistance(currentLatitude, currentLongitude, a.Latitude, a.Longitude) <= radiusInKm);
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Radius of the Earth in km
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lat2 - lon1);
            double a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }

    public sealed class AirportMap : ClassMap<Airport>
    {
        public AirportMap()
        {
            Map(m => m.AirportName).Name("name");
            Map(m => m.City).Name("municipality");
            Map(m => m.State).Name("iso_region");
            Map(m => m.IATACode).Name("iata_code");
            Map(m => m.Type).Name("type");
            Map(m => m.Latitude).Name("latitude_deg");
            Map(m => m.Longitude).Name("longitude_deg");
        }
    }
}
