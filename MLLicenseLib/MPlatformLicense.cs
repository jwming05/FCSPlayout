using System;
using System.Runtime.InteropServices;

namespace MLLicenseLib
{
    [ClassInterface(ClassInterfaceType.None), ComVisible(true), Guid("F42F21A4-F520-4D11-AA37-A94E262AECAD")]
    public class MPlatformLicense : IMPlatformLicense
    {
        private double dLastMPlatformLicenseCheck;

        private double dCheckMPlatformLicenseInterval;

        public MPlatformLicense()
        {
            this.dCheckMPlatformLicenseInterval = 5.0;
        }

        private void InitializeProtection()
        {
            this.dLastMPlatformLicenseCheck = DateTime.UtcNow.ToOADate();
            MPlatformSDKLic.IntializeProtection();
            EncoderlibLic.IntializeProtection();
            DecoderlibLic.IntializeProtection();
            FlashOverlaypluginforMPlatformLic.IntializeProtection();
            ClosedCaptionslibLic.IntializeProtection();
        }

        public void Timer()
        {
            double num = DateTime.UtcNow.ToOADate();
            if (num - this.dLastMPlatformLicenseCheck >= this.dCheckMPlatformLicenseInterval / 1440.0)
            {
                this.dCheckMPlatformLicenseInterval = 60.0;
                this.dLastMPlatformLicenseCheck = num;
                this.InitializeProtection();
            }
        }
    }
}
