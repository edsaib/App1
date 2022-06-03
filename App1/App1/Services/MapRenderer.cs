using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace App1.Services
{
    /// <summary>
    /// CustomMap definition with additional CustomPin information for device specific CustomMapRenderer
    /// </summary>
    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
    }

    /// <summary>
    /// Custom Pin definition with additional Name and Url
    /// </summary>
    public class CustomPin : Pin
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    /// <summary>
    /// Details for Custom Pins (not really necessary...)
    /// </summary>
    public class MapDetails
    {
        public MapDetails()
        {

        }

        public string PinLabel { get; set; }
        public string PinAddress { get; set; }
        public string PinName { get; set; }
        public string PinUrl { get; set; }
        public Position Position { get; set; }

    }



}
