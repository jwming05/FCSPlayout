using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MLPROXYLib;
using System;
using System.Diagnostics;

namespace MLLicenseLib
{
    public class DecoderlibLic
    {
        private static CoMLProxyClass m_objMLProxy;

        private static string strLicInfo = "[MediaLooks]\r\nLicense.ProductName=Decoder lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={E60CFFAA-3BB0-4AFA-94FA-3D04109D7F3B}\r\nLicense.Key={D06DCE2D-5F21-D962-9C13-89328566F8E1}\r\nLicense.Name=MPlaylist Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=MDecoders\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=56E449664CB56B1879EE8C7E9D8BF107BF63509F41F88CB89573A81EE9CC40F0A723C3980F12CF25D6AB536BC90BD867CEFE23B7F51B76463D4433DAD5C9E3F11FA348EEF1289649952E3997F26C67ED5C0E590BEC17F33D103E91AB3777331CF5641CDB7450BD2453A0BED074BDF5223D42D5EA003B07914821F2B78741CA5C\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Decoder lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={42B040BE-5246-4AC9-98FE-F330A82FB426}\r\nLicense.Key={95D7BE3D-C743-C09D-04D0-16C0F0F36C7B}\r\nLicense.Name=MFile Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=MDecoders\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=38C74B7ED9877EEF84E50841844747A7F567356FA5D67951039FEE3132A1EFAA79ACC70CEACBFD979D765E1D0E3AF71D0C85F17046A593B5C839446B5882A87812BD62A89824023E9F4B106D5A0AB1329F6A9F19A725C2A11D7B593B5211366A4A6126DB6C87999316B72B0A1EDF02A0416A434DE1D486451A11984B35F734C6\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Decoder lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={633A6CF8-7146-49B7-89EC-675D83DA7F38}\r\nLicense.Key={455A7083-0719-1347-6DBF-80DCDDF133F8}\r\nLicense.Name=MMixer Module (BETA)\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=MDecoders\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=0E04922F946C083647BB099F98DC8CFFDAC8898CBBAE7F4B556D4CBA6C8790BEEF78970EF4432123DCF739E9DA79896DA1B2A45FB37B84B5783D840DC9B84A590B26FF060F7F5CA5CF9D36C833911628674A1E8FECC77CEEA8A22EECF5CADDDF1EBB6533FED927FD3487ADCD3CF4CF5B52211D3967E90B784F779538AA6231AC\r\n\r\n";

        [DebuggerNonUserCode]
        public DecoderlibLic()
        {
        }

        public static void IntializeProtection()
        {
            if (DecoderlibLic.m_objMLProxy == null)
            {
                DecoderlibLic.m_objMLProxy = new CoMLProxyClass();
                DecoderlibLic.m_objMLProxy.PutString(DecoderlibLic.strLicInfo);
            }
            DecoderlibLic.UpdatePersonalProtection();
        }

        private static void UpdatePersonalProtection()
        {
            checked
            {
                try
                {
                    int num = 0;
                    int num2 = 0;
                    DecoderlibLic.m_objMLProxy.GetData(out num, out num2);
                    long num3 = unchecked((long)Conversion.Fix(num)) * 44817467L % 43373189L;
                    long num4 = unchecked((long)Conversion.Fix(num2)) * 47128061L % 42408581L;
                    uint num5 = DecoderlibLic.SummBits((uint)(num3 + num4));
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
                    DecoderlibLic.m_objMLProxy.SetData(num, num2, (int)Conversion.Fix(number), nSecondRes);
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
