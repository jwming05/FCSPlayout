using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MLPROXYLib;
using System;
using System.Diagnostics;

namespace MLLicenseLib
{
    public class EncoderlibLic
    {
        private static CoMLProxyClass m_objMLProxy;

        private static string strLicInfo = "[MediaLooks]\r\nLicense.ProductName=Encoder lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={3E04C40E-83CF-4F1E-A858-348401828F27}\r\nLicense.Key={A8A0F447-D6D8-E159-73E4-1E36F9223AA5}\r\nLicense.Name=MRenderer Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=MWriter\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=1C0465278ABAD29F2B0CCE91631446E165EB3B36309BD98A766FC8334240A7D60ECAFA1011E44B648A9AFA11E54517ACE21607C2A092B9BFEA2797CFEB3FF037531DB3ACC4C81705D9B6355763BD0A8CA2F4EF87C0A82EA63673E7BDD7C11457D97A982DB9AC222D82E21752C3B69BEC71066BE1E00A7438B18E8E3F88014AC0\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Encoder lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={BF10E911-9B4E-4ADC-A7C6-835927BF3700}\r\nLicense.Key={58DEC546-63BE-8F12-DAE5-B55B58D3DC22}\r\nLicense.Name=MWriter Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=MWriter\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=3E1E30146485D0BD1E322200A92351394909D6DB59A7380383975FDD6BD2A004FE7F9A83F9684434A1CA0760BC83EAAA0AECC107397B9385961ED009EA1D26F7890076C777A1508A8EEDE525723C32A7D8EB44338FAB5F40EBC0D8865C1819B861D610A6691C2919E4F16AD03BDA313842C8C4ABD5A5EF85BDAA6E07C58099C5\r\n\r\n";

        [DebuggerNonUserCode]
        public EncoderlibLic()
        {
        }

        public static void IntializeProtection()
        {
            if (EncoderlibLic.m_objMLProxy == null)
            {
                EncoderlibLic.m_objMLProxy = new CoMLProxyClass();
                EncoderlibLic.m_objMLProxy.PutString(EncoderlibLic.strLicInfo);
            }
            EncoderlibLic.UpdatePersonalProtection();
        }

        private static void UpdatePersonalProtection()
        {
            checked
            {
                try
                {
                    int num = 0;
                    int num2 = 0;
                    EncoderlibLic.m_objMLProxy.GetData(out num, out num2);
                    long num3 = unchecked((long)Conversion.Fix(num)) * 44817467L % 43373189L;
                    long num4 = unchecked((long)Conversion.Fix(num2)) * 47128061L % 42408581L;
                    uint num5 = EncoderlibLic.SummBits((uint)(num3 + num4));
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
                    EncoderlibLic.m_objMLProxy.SetData(num, num2, (int)Conversion.Fix(number), nSecondRes);
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
