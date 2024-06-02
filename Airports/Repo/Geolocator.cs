namespace Airports;

public static class Geolocator
{
	public static IGeolocator Default = new GeolocatorImplementation();
}