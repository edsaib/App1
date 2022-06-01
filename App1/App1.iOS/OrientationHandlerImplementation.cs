using Foundation;
using System;
using System.Collections.Generic;
using App1.Services;
using System.Linq;
using System.Text;
using UIKit;

namespace App1.iOS
{
    class OrientationHandlerImplementation : IOrientationHandler
    {
        public void EnableOrientation()
        {
            return;
        }

        public void ForceLandscape()
        {
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeLeft), new NSString("orientation"));
        }

        public void ForcePortrait()
        {
            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));
        }
    }
}