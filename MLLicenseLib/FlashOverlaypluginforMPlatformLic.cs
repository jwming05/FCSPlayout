using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MLPROXYLib;
using System;
using System.Diagnostics;

namespace MLLicenseLib
{
    public class FlashOverlaypluginforMPlatformLic
    {
        private static CoMLProxyClass m_objMLProxy;

        private static string strLicInfo = "[MediaLooks]\r\nLicense.ProductName=Flash Overlay plugin for MPlatform\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={177B2753-64BD-46D3-950D-FA81FB3B9C65}\r\nLicense.Key={8D674A69-A853-1505-ABD7-D16FBFE514F0}\r\nLicense.Name=MediaLooks Character Generator\r\nLicense.AllowedObject={0F56D2E7-033C-4A05-BCDA-DF58C9BBF06F}\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Flash\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=A79005C8BEC3219722460FC13AAFC38C66087E20A12B7F23B579A2FCFA10B08A541423D4D1546A45B905EC183770B431E4A495EED0A30A11A478A5B99ABA114D73957CF9770921544613B6C31ECAFE8B6469019866297602A960118030FEBA1C75DC6D1A33335602FB2C585DCF79F6DF0CDB95AEB5AC141FDD4D2294FE7AC96A\r\n\r\n";

        [DebuggerNonUserCode]
        public FlashOverlaypluginforMPlatformLic()
        {
        }

        public static void IntializeProtection()
        {
            if (FlashOverlaypluginforMPlatformLic.m_objMLProxy == null)
            {
                FlashOverlaypluginforMPlatformLic.m_objMLProxy = new CoMLProxyClass();
                FlashOverlaypluginforMPlatformLic.m_objMLProxy.PutString(FlashOverlaypluginforMPlatformLic.strLicInfo);
            }
            FlashOverlaypluginforMPlatformLic.UpdatePersonalProtection();
        }

        private static void UpdatePersonalProtection()
        {
            checked
            {
                try
                {
                    int num = 0;
                    int num2 = 0;
                    FlashOverlaypluginforMPlatformLic.m_objMLProxy.GetData(out num, out num2);
                    long num3 = unchecked((long)Conversion.Fix(num)) * 44817467L % 43373189L;
                    long num4 = unchecked((long)Conversion.Fix(num2)) * 47128061L % 42408581L;
                    uint num5 = FlashOverlaypluginforMPlatformLic.SummBits((uint)(num3 + num4));
                    long number = unchecked((long)Conversion.Fix(checked(num - 29))) * unchecked((long)(checked(num - 23))) % unchecked((long)num2);
                    int number2 = new Random().Next(32767);
                    int nSecondRes;
                    if (unchecked((ulong)num5) > 0uL)
                    {
                        nSecondRes = (int)Conversion.Fix(number) + Conversion.Fix(number2) * 1;
                    }
                    else
                    {
                        nSecondRes = (int)Conversion.Fix(number) + Conversion.Fix(number2) * -1;
                    }
                    FlashOverlaypluginforMPlatformLic.m_objMLProxy.SetData(num, num2, (int)Conversion.Fix(number), nSecondRes);
                }
                catch (Exception expr_BE)
                {
                    ProjectData.SetProjectError(expr_BE);
                    ProjectData.ClearProjectError();
                }
            }
        }

        private static uint SummBits(uint _nValue)
        {
            uint num = 0u;
            while ((ulong)_nValue > 0uL)
            {
                num = checked((uint)(unchecked((ulong)num) + (unchecked((ulong)_nValue) & 1uL)));
                _nValue >>= 1;
            }
            return checked((uint)(unchecked((ulong)num) % 2uL));
        }
    }
}
