using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileComparator
{
    class MD5Generator
    {
        public Task<string> GenerateAsync(string fileName)
        {
            return Task.Run<string>(() => Generate(fileName));
        }

        public string Generate(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,FileShare.Read))
            {
                MD5 md5 = MD5.Create();
                md5.Initialize();

                return Generate(fs);
                //return BytesToStr(md5.ComputeHash(fs));
            }
        }

        public Task<string> GenerateAsync(Stream stream)
        {
            return Task.Run<string>(() => Generate(stream));
        }

        public string Generate(Stream stream)
        {
            MD5 md5 = MD5.Create();

            byte[] buffer = new byte[1024];

            int readCount = stream.Read(buffer, 0, buffer.Length);

            while (readCount > 0)
            {
                md5.TransformBlock(buffer, 0, readCount, null, 0);
                readCount = stream.Read(buffer, 0, buffer.Length);
            }

            md5.TransformFinalBlock(buffer, 0, 0);
            return BytesToStr(md5.Hash);
        }

        public static string BytesToStr(byte[] bytes)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                str.AppendFormat("{0:X2}", bytes[i]);

            return str.ToString();
        }
    }
}
