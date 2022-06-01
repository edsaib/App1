using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Services
{
    public interface IOrientationHandler
    {
        void ForceLandscape();

        void ForcePortrait();

        void EnableOrientation();
    }
}
