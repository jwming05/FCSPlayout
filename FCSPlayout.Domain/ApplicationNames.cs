using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.Domain
{
    public class ApplicationNames
    {
        public const string FileImporter = "FileImporter";        // 入库
        public const string PlaybillEdition = "PlaybillEdition";  // 编单
        public const string PlayService = "PlayService";          // 播出

        public static string[] AllApplicationNames = new string[]
        {
            FileImporter,PlaybillEdition,PlayService
        };


    }
}
