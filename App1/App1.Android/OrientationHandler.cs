﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App1.Droid;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(OrientationHandler))]
namespace App1.Droid
{
    class OrientationHandler : BaseDependencyImplementation, IOrientationHandler
    {
        public void EnableOrientation()
        {
            GetActivity().RequestedOrientation = ScreenOrientation.Unspecified;
        }

        public void ForceLandscape()
        {
            GetActivity().RequestedOrientation = ScreenOrientation.Landscape;
        }

        public void ForcePortrait()
        {
            GetActivity().RequestedOrientation = ScreenOrientation.Portrait;
        }
    }
}