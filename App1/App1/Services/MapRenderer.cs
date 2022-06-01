using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace App1.Services
{
    public class CustomMap : Map
    {
        public List<CustomPin> CustomPins { get; set; }
    }

    public class CustomPin : Pin
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

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
