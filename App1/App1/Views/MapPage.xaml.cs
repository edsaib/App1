using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace App1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage(List<MapDetails> details)
        {
            InitializeComponent();

            customMap.CustomPins = new List<CustomPin> { };

            foreach (MapDetails detail in details)
            {
                CustomPin pin = new CustomPin
                {
                    Type = PinType.Place,
                    Position = detail.Position,
                    Label = detail.PinLabel,
                    Address = detail.PinAddress,
                    Name = "Xamarin",
                    Url = "http://xamarin.com/about/"
                };

                customMap.CustomPins.Add(pin);
                customMap.Pins.Add(pin);
            }

            //customMap.CustomPins = new List<CustomPin> { pin };
            //customMap.Pins.Add(pin);
            // Move map view to region of given position
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(details[0].Position, Distance.FromMiles(1.0)));

            // Xamarin Forms customMap instance is rendered with device specific CustomMapRenderer.
            // Set the content of this page to the Xamarin Forms customMap element.
            Content = customMap;
        }

        /// <summary>
        /// When this page is available, force Landscape mode on phone
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            DependencyService.Get<IOrientationHandler>().ForceLandscape();
        }

        /// <summary>
        /// When this page is disappearing from the navigation stack, enable orientation change of phone again.
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DependencyService.Get<IOrientationHandler>().EnableOrientation();
        }

    }
}