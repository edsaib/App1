using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Services
{
    /// <summary>
    /// Interface to control the orientation on devices
    /// </summary>
    public interface IOrientationHandler
    {
        void ForceLandscape();

        void ForcePortrait();

        void EnableOrientation();
    }
}
