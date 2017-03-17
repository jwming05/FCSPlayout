using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MLPROXYLib;
using System;
using System.Diagnostics;

namespace MLLicenseLib
{
    public class MPlatformSDKLic
    {
        private static CoMLProxyClass m_objMLProxy;

        private static string strLicInfo = "[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={E60CFFAA-3BB0-4AFA-94FA-3D04109D7F3B}\r\nLicense.Key={C782E526-EEAD-7056-D318-95902C30A406}\r\nLicense.Name=MPlaylist Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=9DD05DB187A3D07270143007E941EBD0603745E1E1C2DDD99BBB0B89FB6F466D367C03A942ACEFC87A61CC203383D2BB042CC5D2A16D03469997B190EE17CAE74DAD8DB9CD0197EFC92D28073AD30841495AF0CBBB38729AE249CC5D9DEF5FCC730D36698450B2BEBDFEC0F6F292D980B625E83A0F9722C38EBADA57B44AB131\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={42B040BE-5246-4AC9-98FE-F330A82FB426}\r\nLicense.Key={71470B78-5A3B-E0EC-2316-0F5B3ECB8CF5}\r\nLicense.Name=MFile Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=5CA1DFF8174FD47E233F0B9411F85C46A416ECF9483134F8E34A09650752AF900D428542CAEFF0549EF0AFC04DB02F4DB3D6C754F7E018A7731CBDDA3FCFC08393327138185C0D23C1FC52CEE1A0207415F4E11C21372778B529E98EDC48B8151B0B669391D1521E917DCCDB6ECDFD976D771EBA33BA7E1CD36E1D71A05D7E1F\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={BD7D9E62-0843-4CA1-A597-B10F446A6AC7}\r\nLicense.Key={DD907003-728E-D2AE-5423-95B98B5A74DF}\r\nLicense.Name=MLive Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=9308BE839F906BCC99E6C98C852C20C3271B0BA28635CAC4DCF8FB73BB4FC34448D234B803AB99F77BC665EDF42400915F0162FDACDCB38BE03BE17A84F1A5BD6A2920A654C9D7A963C8E9FAF761B51000046A68E23A817DD41E3D89CF622A8EEF64FE4894489AE478181676B83A807516ACE60ABAEA043AB9D0FC30709C3EB0\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={3E04C40E-83CF-4F1E-A858-348401828F27}\r\nLicense.Key={F82B2B58-4E74-19CC-A111-1CCCB36B77A4}\r\nLicense.Name=MRenderer Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=D0F3C22894433EF27BB8EFCF9EF897E5A3B87468FF2039F421A974DF6F85744D502EC13A6DD2A033D75BC02154C41CC1BC9677B25FCE01938F729C4E9BFBD10E800EE71A61EEBCF61687A6E00B3377AE85E112282A361702B8E8C5915FA624650CE8DCAF33654E1B0871ACC1D02A06B83308A9E15C6C213D9856C66F2DB2CD00\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={5BEEE6F9-4733-44EF-8905-4084918C0FE3}\r\nLicense.Key={75D04182-9824-DBD3-ABC8-C1722DA86EA9}\r\nLicense.Name=MTransportDS Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=54EF8B4D8CFEE415405F3D9850C42D49836022998ECCEF3DE72369F44B488CD90BA1C7E00D920F05386A464748BFFFA072E54A75E0A3FE2E2B960795ED04C3BB40A4A5F0360527EB4B8ACC3253ADF4D98454E624B0EE11FF8EE2EBE8483B150E3C5968D1FD781851D72CC1C5F3F8E031390E72048F7E2FBB25821B35B42DC4A2\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={633A6CF8-7146-49B7-89EC-675D83DA7F38}\r\nLicense.Key={809B9F2C-4EBA-0996-4D6C-6B08F2534E5D}\r\nLicense.Name=MMixer Module (BETA)\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=C4EF681CF8B51F8F95CADED640DFE0E4F8013F860BFD945FC93909DA0BDBC22FD1377EEC039460A5CC17CBC69FB0F63145CC472B5898252DC16B85173E4EA9F139DF88D261D86FFC70D2D3A84DC755A7F58665446EF0671754E937D29B08363933EFA2961AFA4D7618E2072A09E5B40D275A52CE5B84C37EC92A4DC80574C835\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={A046A389-10C6-4F17-B3F0-9C7D54B23791}\r\nLicense.Key={4BE2A635-17D1-15F0-EB8F-915D136156CA}\r\nLicense.Name=MediaLooks Character Generator\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Professional\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=10C473E3F02FABC4DAB47DA90F6F757BEAC8445A1172A48952CABF52792ED497F00068E9703A1018E9DE183E5C5503ED7BE12B8A7D343728AC6AC927450B1BCA3E59AE6B72B4020FB8F7EC82707F5875CC085C87C98F2B2FB51DAA16D507F00EF80C3981800B352E080A82ABF29CF322CFB90582A3804213DE726E670F5D0353\r\n\r\n[MediaLooks]\r\nLicense.ProductName=MPlatform SDK\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={17EC726F-EBE2-45E9-897B-6C8894E974E6}\r\nLicense.Key={DD907003-43FA-8483-D2C4-47C83529C42C}\r\nLicense.Name=MPlatform Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Standard\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=9EECF5AE5531B428F53BD5E28C1A9C65D3238E702F1766BCAA68C55EC191309C303366E2A49011E18A842EA508AE4B2E8ACB364864979A2F20E2743D2A62AEE3DF9FD14E63B035478EB490A5859B67BE70D9717A5C92695268BE419EE9F94C9DEF3273F9E14828298BF8EF9A442EC52EEF7293F12360AF834AEA4A2D07089F02\r\n\r\n";

        [DebuggerNonUserCode]
        public MPlatformSDKLic()
        {
        }

        public static void IntializeProtection()
        {
            if (MPlatformSDKLic.m_objMLProxy == null)
            {
                MPlatformSDKLic.m_objMLProxy = new CoMLProxyClass();
                MPlatformSDKLic.m_objMLProxy.PutString(MPlatformSDKLic.strLicInfo);
            }
            MPlatformSDKLic.UpdatePersonalProtection();
        }

        private static void UpdatePersonalProtection()
        {
            checked
            {
                try
                {
                    int num = 0;
                    int num2 = 0;
                    MPlatformSDKLic.m_objMLProxy.GetData(out num, out num2);
                    long num3 = unchecked((long)Conversion.Fix(num)) * 44817467L % 43373189L;
                    long num4 = unchecked((long)Conversion.Fix(num2)) * 47128061L % 42408581L;
                    uint num5 = MPlatformSDKLic.SummBits((uint)(num3 + num4));
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
                    MPlatformSDKLic.m_objMLProxy.SetData(num, num2, (int)Conversion.Fix(number), nSecondRes);
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
