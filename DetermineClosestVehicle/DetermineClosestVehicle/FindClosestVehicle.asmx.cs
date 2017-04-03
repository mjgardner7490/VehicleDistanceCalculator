using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DetermineClosestVehicle
{
    /// <summary>
    /// Summary description for FindClosestVehicle
    /// </summary>
    [WebService(Namespace = "http://vehicledistancecalculator.azurewebsites.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FindClosestVehicle : System.Web.Services.WebService
    {
        private double _pickupLocationLat;
        private double _pickupLocationLong;

        [WebMethod]
        public VinLocation FindVehicle(FindVehiclePayload FindVehiclePayload)
        {
            _pickupLocationLat = FindVehiclePayload.PickupLocationGPS.Latitude;
            _pickupLocationLong = FindVehiclePayload.PickupLocationGPS.Longitude;

            var currClosestVehicle = 100000000.0;
            var vinLocationClosestVehicle = new VinLocation();
            foreach (var vinLocation in FindVehiclePayload.VinLocations)
            {
                var distance = DistanceInKmBetweenGPSCoordinates(_pickupLocationLat, _pickupLocationLong, vinLocation.GPS.Latitude, vinLocation.GPS.Longitude);

                if (distance < currClosestVehicle)
                {
                    currClosestVehicle = distance;
                    vinLocationClosestVehicle = vinLocation;
                }
            }

            return vinLocationClosestVehicle;
        }
        double DistanceInKmBetweenGPSCoordinates(double lat1, double lon1, double lat2, double lon2)
        {
            var earthRadiusKm = 6371;

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            lat1 = DegreesToRadians(lat1);
            lat2 = DegreesToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        private double DegreesToRadians(double coordDifference)
        {
            return coordDifference * Math.PI / 180;
        }

    }

    public class GPSData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class VinLocation
    {
        public string Vin { get; set; }
        public GPSData GPS { get; set; }
    }

    public class FindVehiclePayload
    {
        public GPSData PickupLocationGPS { get; set; }
        public List<VinLocation> VinLocations { get; set; }
    }
}
