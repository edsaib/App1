using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace App1.Droid
{
    class BaseDependencyImplementation : Object
    {
        public Activity GetActivity()
        {
            var activity = (Activity)Forms.Context;
            return activity;
        }
    }
}