using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airports.Repo
{
    public class UnitConverter
    {
        // 1 millibar (mb) is equal to 1 hectopascal (hPa)
        public double MillibarsToHectopascals(double millibars)
        {
            return millibars;
        }

        // 1 millibar (mb) is approximately equal to 0.02953 inches of mercury (inHg)
        public double MillibarsToInchesOfMercury(double millibars)
        {
            return millibars * 0.02953;
        }

        // 1 mile per hour (mph) is equal to 1.60934 kilometers per hour (km/h)
        public double MilesPerHourToKilometersPerHour(double mph)
        {
            return mph * 1.60934;
        }

        // 1 mile per hour (mph) is equal to 0.868976 knots (nautical miles per hour)
        public double MilesPerHourToKnots(double mph)
        {
            return mph * 0.868976;
        }
    }
}
