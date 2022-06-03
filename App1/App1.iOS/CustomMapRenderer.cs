using System;
using System.Collections.Generic;
using CoreGraphics;
using App1;
using App1.iOS;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
using App1.Services;

// assembly: Attribute applies to the entire assembly
// Export renderer for CustomMap instance
[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace App1.iOS
{
    /// <summary>
    /// CustomMapRenderer for iOS devices
    /// </summary>
    class CustomMapRenderer : MapRenderer
    {
        // Pin view shown on the map
        UIView customPinView;
        // Holds information for custom markers
        List<CustomPin> customPins;

        /// <summary>
        /// The MapRenderer class exposes the OnElementChanged method, which is called when the Xamarin.Forms custom map is created to render the corresponding native control.
        /// This method registers properties and event handlers for this CustomMapRenderer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            // Xamarin Forms element the renderer was attached to
            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
            }

            // Xamarin Forms element the renderer is attached to
            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                customPins = formsMap.CustomPins;

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }

        /// <summary>
        /// This method is called when the location of the annotation becomes visible on the map, and is used to customize the annotation prior to display.
        /// The callout displays the Label and Address properties of the Pin instance, with optional left and right accessory views.
        /// </summary>
        /// <param name="mapView"></param>
        /// <param name="annotation">Contains the annotation data</param>
        /// <returns>Returns an MKAnnotationView for display on the map</returns>
        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
                return null;

            var customPin = GetCustomPin(annotation as MKPointAnnotation);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            // build annotationView
            annotationView = mapView.DequeueReusableAnnotation(customPin.Name);
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Name);
                annotationView.Image = UIImage.FromFile("pin.png");
                annotationView.CalloutOffset = new CGPoint(0, 0);
                //annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("monkey.png"));
                annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                ((CustomMKAnnotationView)annotationView).Name = customPin.Name;
                ((CustomMKAnnotationView)annotationView).Url = customPin.Url;
            }
            annotationView.CanShowCallout = true;

            return annotationView;
        }

        /// <summary>
        /// When the user taps on the Information button in the right callout accessory view, the CalloutAccessoryControlTapped event fires, which in turn executes the OnCalloutAccessoryControlTapped method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            CustomMKAnnotationView customView = e.View as CustomMKAnnotationView;
            if (!string.IsNullOrWhiteSpace(customView.Url))
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
            }
        }

        /// <summary>
        /// When the user taps on the annotation, the DidSelectAnnotationView event fires, which in turn executes the OnDidSelectAnnotationView method.
        /// Constructs the customPinView instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            CustomMKAnnotationView customView = e.View as CustomMKAnnotationView;
            customPinView = new UIView();

            if (customView.Name.Equals("Xamarin"))
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                var image = new UIImageView(new CGRect(0, 0, 200, 84));
                image.Image = UIImage.FromFile("xamarin.png");
                customPinView.AddSubview(image);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                e.View.AddSubview(customPinView);
            }
        }

        /// <summary>
        /// When the annotation is displayed and the user taps on the map, the DidDeselectAnnotationView event fires, which in turn executes the OnDidDeselectAnnotationView method
        /// Deconstructs the customPinView instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (!e.View.Selected)
            {
                customPinView.RemoveFromSuperview();
                customPinView.Dispose();
                customPinView = null;
            }
        }

        /// <summary>
        /// Returns the CustomPin instance of a given annotation. Position of annotation must match that of the pin to be returned.
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
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