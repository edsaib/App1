using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using App1;
using App1.Droid;
using App1.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

// assembly: Attribute applies to the entire assembly
// Export renderer when a CustomMap instance is created
[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace App1.Droid
{
    /// <summary>
    /// CustomMapRenderer for Android devices
    /// </summary>
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<CustomPin> customPins;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        /// <summary>
        /// The MapRenderer class exposes the OnElementChanged method, which is called when the Xamarin.Forms custom map is created to render the corresponding native control.
        /// This method retrieves the list of custom pins.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            // Xamarin Forms element the renderer was attached to
            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            // Xamarin Forms element the renderer is attached to
            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
            }
        }

        /// <summary>
        /// This method will be invoked once the native map instance is available.
        /// This method registers Event handlers (OnInfoWindowClick) for the custom markers on the map,
        /// and calls the SetInfoWindowAdapter method, which registers the CustomMapRenderer (this).
        /// </summary>
        /// <param name="map"></param>
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        /// <summary>
        /// Costumize the marker with the given Pin information
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>Customized Marker object</returns>
        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
            return marker;
        }

        /// <summary>
        /// When the user clicks on the info window, the InfoWindowClick event fires, which in turn executes the OnInfoWindowClick method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        /// <summary>
        /// When a user taps on the marker, the GetInfoContents method is executed, provided that the GetInfoWindow method returns null.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns></returns>
        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                // Show specific Layouts for specific pins
                if (customPin.Name.Equals("Xamarin"))
                {
                    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                }
                else
                {
                    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                }

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }

        /// <summary>
        /// This method is called to return a custom info window for a given marker. 
        /// If it returns null, then the default window rendering will be used. 
        /// If it returns a View, then that View will be placed inside the info window frame.
        /// </summary>
        /// <param name="marker"></param>
        /// <returns>Returns null, thus the default window rendering will be used</returns>
        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }

    }
}