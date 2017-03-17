using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MLPROXYLib;
using System;
using System.Diagnostics;

namespace MLLicenseLib
{
    public class ClosedCaptionslibLic
    {
        private static CoMLProxyClass m_objMLProxy;

        private static string strLicInfo = "[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={42B040BE-5246-4AC9-98FE-F330A82FB426}\r\nLicense.Key={F82B2B58-83FB-D5A9-6686-74B8DD745BFE}\r\nLicense.Name=MFile Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=23C8767CE41A2C6EE1BEEED5A0F4B7432488BD79B9360D409DDE4EF9FF514440F5AFF23FE666DAC2CEACC445038C8A7027D8DA9EFF59FED1AE1B463C500113DF07FEDF58F1317291C053573ABA23E77716DECF0449EDD3E7C53C46E4C22F80EA319FA236D512FF70F504D0E65455918170D4A1F167A0EF6C79EABCC19837553C\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={E60CFFAA-3BB0-4AFA-94FA-3D04109D7F3B}\r\nLicense.Key={8A8A0C4F-8820-5D5D-1E67-325A09DFDF1B}\r\nLicense.Name=MPlaylist Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=DAAD7D0EDDE2080BA5698675603F6422D181618CEFB0CBF00A7DFDDC462EA85F8F6A32B3AA3892CCB7395F80D607F7CE30432319F93A5D1F06BC44602191437F4EA8BDA908D17865FD8AF6CC47D5DAA7D8117797D5A40AF507B6E79341CE61D4564B2BC286EF75325A845A3FC61B9D77CA56CB70290403E9082B1167736C5D80\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={BD7D9E62-0843-4CA1-A597-B10F446A6AC7}\r\nLicense.Key={033ED5B6-17BC-E43B-146F-E330BEE6BFD1}\r\nLicense.Name=MLive Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=105D6CFB17C47A17D2AE70A9B201E0F7A18B844755D0FAFE374D700D5A358804081EDEC5373181A12731039E34BFD8A0ED2EC231F1619AB93361E2733C0D56E62E18E2CFE762D844E2361501CDD1D17FC612CCD350DE9F0DDD3D672BE1B29CC7D090FB90FAB451B837720EC0E711C6E3C6DDE6B042F915522944A09D20F00ACB\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={3E04C40E-83CF-4F1E-A858-348401828F27}\r\nLicense.Key={E2CC9714-9FC1-C957-AF92-8DE17745E3EE}\r\nLicense.Name=MRenderer Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=A34287FEB12F6A00AE04B95301006AFA7695BC5898A649C1C43BDE0BC14861DB59C1CF0F1C90C729DA87FAF2DFB44854250C8E7ED26F83E3023A762998F0627D587DDACDB2488A2D9626A5A7691E6D27AB5369575CAE399A701D4593582478251233091482A5799F88ABF99253160F4F90D6B9E1E9EABBE3A7D0E84299EF7FC7\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={749DA3A9-B106-4BD7-911F-1CE5D344B640}\r\nLicense.Key={8C0E4102-C6BF-4423-CF2A-0D86D4DDDE0E}\r\nLicense.Name=MFormats Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=9B33F6E9D3CAD6C765DCC290947CD455DB907BE0E4DB5309265C27C71410F361ADBFBA480487A34EF758FFF39769677937F2A5A57D6C76013FA98156893F35BA2B47A0816DBBF7DB7A132D2C3A77FCCDACBF82A736D8D94B3E8B81E57B703DE0DFC03324201BC2519D7175B255068A32D83DF7B8F1DB8E987BE133696897C500\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={50B89F22-CD11-465B-A52A-C3258C382B52}\r\nLicense.Key={B903DD4E-FE72-29FE-A365-8DC0692728D0}\r\nLicense.Name=MFReader Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=B3E84C1867ADC0F19248866A09553DEC8C1FDF1C364163D10DC6ED354F01506F5AE869E9D9127D5D51669E51037CD187279F2391DB2DF8D3BE6261215241FB8D5655496525C457261705F3132D8BE1E90D9A5FDBDD6842DCA2A2DC6C02B6BB713CDB88E51EDD6E8A111100B7E78D62C0AEC124065292D684461C77801B7F426D\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={8530EF48-E87E-4B7C-AB2B-F4C1AE4FBD2E}\r\nLicense.Key={A3323261-35A6-0DE2-FF68-D11F13A35E02}\r\nLicense.Name=MFRenderer Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=8E0E0EA72CE7AD5620654285851A5E3234139FD3D62EF126BC409C440C67296D30041F8BB3DA28CF7FE21D34E0EC2A0DC921C77DFCAB09A805B5C4FE9B7CCD1C16BFAAD0E6BEE6B12AC5F206D11B2652BD8FFE7095F06B338A06824FE14D2FBEFF5236AB9D32537DC075175D89E750D58FEBFCD7F801A50024D49C0AD00102AD\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={0568680D-BA7F-4FAA-83B2-B78252FB2FBE}\r\nLicense.Key={8A8A0C4F-860B-CE29-3574-301FF3AD0F86}\r\nLicense.Name=MFWriter Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=772EF97F122CC9AC440EA114F0DDC9E4DFB0E89D5591141D5276AF9058B38399B6B883DBEBC21B91A0E68736747DA5838037696F75137C0BC2FAC6CC0E323620A4E0D363403B9068E923791CDFE40A2E55ED6F238565DCD5302BE16435A9AB5F63AB044CFF320365F9044A99A101F81929EEDB106513AD8B69EA14AE4507F852\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={5BEEE6F9-4733-44EF-8905-4084918C0FE3}\r\nLicense.Key={D4FF0437-65C5-7C33-2042-218921893C63}\r\nLicense.Name=MTransportDS Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=29E743DE344DC4E0DE770E1C6D98347BE694ABE0D33245A431850CFEED2FF7066B4F56FF603D5807DD80D2B874DDB7D79037DFE17D3816390893DC8275AB84BEF7047373A9A185C73BF7607CDD87E6CD390D3AF465E5F14A315F8142E7C444EF8E0DC9DEC781281B50534D9354BAF4993E8FC5A1CD89A693DC432295DFB98413\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={A046A389-10C6-4F17-B3F0-9C7D54B23791}\r\nLicense.Key={809B9F2C-5E68-24FD-BBBA-FC7C86D4A988}\r\nLicense.Name=MediaLooks Character Generator\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=7F9FBBF6F379A44961869A6C12C3CDE2FA95049D943C1E61EFEA0C284566486D61B7A3CF4DB6E7CE32C63FA09B9EEE7DA1982343044B78AA0C34E838C7A2BF04F7F737E467BFD65DAFE760DA58D4951385F10CE874863DE90D985E0D6BACF0AAA2C885C701C682AD5613B789ACB875BE106AF2F47D2E8999FAA9588804132127\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={633A6CF8-7146-49B7-89EC-675D83DA7F38}\r\nLicense.Key={A3323261-2872-BD90-AF7B-E3179917AECE}\r\nLicense.Name=MMixer Module (BETA)\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=5EC4E286B8792106A3CEF8F20DB16D8006410DAA1AF1D685B532C2F6CDCF8F0F83BA95EB75B5EBD5F99E87834B165DBC5B6674F1689B9FCBD8D0FB8E7F392CCB6028072AA73F6A2AD99491D0868250DF590D467D174919F12B0266CD98276C1C42F47F20056D0B93CEA611D5166B761EB1D4924912F96F82C93A9F0E68074333\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={BF10E911-9B4E-4ADC-A7C6-835927BF3700}\r\nLicense.Key={D06DCE2D-AE78-7033-F8DD-A8C3D50D04E0}\r\nLicense.Name=MWriter Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=ED991A716FDD33241E30D9CDF33480642A938772D8DD964D1F3BD48A424E2607866ACFFFBC66416139E40E825C0B084A823F76A5BF95F9213A9A795094149C0FF5074A9B3AA2D43125DA633E6CF48BD8BDD700C8407BB74E6F365F99DDA89073B768DB2DA1CD932843A0465718932DF2AB5DB779B65951E9B3ABA9F6895D6D3F\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={17EC726F-EBE2-45E9-897B-6C8894E974E6}\r\nLicense.Key={F3AF781B-9FD0-C7B3-3A2D-FE7DAD48A7DB}\r\nLicense.Name=MPlatform Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=7CB555A54DBE43314F36533218A006C4542AE35F72E0D0B3E816986A4E641F79E79403ADAE15F920D21D1BDA9076AE5153BF93EF1F987A3F04EC3652A236ECA7DC34C57232294C34FF30CE2227CAF83E2D7A2941A762083A0AFAC40578602175472F59E66BCBC8DDEF98263E208E6C4A8E65C0CC85CF363C503FFE6EE4E92402\r\n\r\n[MediaLooks]\r\nLicense.ProductName=Closed Captions lib\r\nLicense.IssuedTo=Axel Technology SLR\r\nLicense.CompanyID=169\r\nLicense.UUID={A157EAA7-79D9-46AA-9038-CE682388BA58}\r\nLicense.Key={08AD9FAC-48FE-792E-F8DD-A8C35A084BB9}\r\nLicense.Name=MComposer Module\r\nLicense.UpdateExpirationDate=August 23, 2016\r\nLicense.Edition=Closed_Captions\r\nLicense.AllowedModule=*.*\r\nLicense.Signature=73DCC8A89FB0830AA3A1E1990B9E85EB16EDEFE0A8D24C0AE575F5A3B636A5CC594F5F0907543FBF060BA275C518629A3B783035014F7B6F251D39D38F92900F1599A7D21B154D4036D6C48FAED5F932B594F9A9110D8E34FEFD00E505057D767C0D2CA142BC6F5674ACA72D207D920E8409FB3AA2BF7FCE1B0F2F82207C28B8\r\n\r\n";

        [DebuggerNonUserCode]
        public ClosedCaptionslibLic()
        {
        }

        public static void IntializeProtection()
        {
            if (ClosedCaptionslibLic.m_objMLProxy == null)
            {
                ClosedCaptionslibLic.m_objMLProxy = new CoMLProxyClass();
                ClosedCaptionslibLic.m_objMLProxy.PutString(ClosedCaptionslibLic.strLicInfo);
            }
            ClosedCaptionslibLic.UpdatePersonalProtection();
        }

        private static void UpdatePersonalProtection()
        {
            checked
            {
                try
                {
                    int num = 0;
                    int num2 = 0;
                    ClosedCaptionslibLic.m_objMLProxy.GetData(out num, out num2);
                    long num3 = unchecked((long)Conversion.Fix(num)) * 44817467L % 43373189L;
                    long num4 = unchecked((long)Conversion.Fix(num2)) * 47128061L % 42408581L;
                    uint num5 = ClosedCaptionslibLic.SummBits((uint)(num3 + num4));
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
                    ClosedCaptionslibLic.m_objMLProxy.SetData(num, num2, (int)Conversion.Fix(number), nSecondRes);
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
