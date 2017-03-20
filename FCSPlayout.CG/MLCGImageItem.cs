using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MLCHARGENLib;
using System.IO;

namespace FCSPlayout.CG
{
    [Serializable]
    public class MLCGImageItem : MLCGGenericItem
    {
        private static string _tempDir = null;

        public string File
        {
            get { return base.FileNameOrItemDesc; }
            set { base.FileNameOrItemDesc = value; }
        }

        protected override MLCGGenericItem CloneInternal()
        {
            return new MLCGImageItem();
        }

        protected override void InitInternal(XElement element)
        {
            base.InitInternal(element);
            
            var image = element.Element("image");
            var fileName =Guid.NewGuid().ToString("N")+image.Attribute("extension").Value;
            var imageContent = ((XText)image.FirstNode).Value;

            SaveFile(fileName, imageContent);
        }

        private void SaveFile(string fileName, string imageContent)
        {
            string filePath =GetFilePath(fileName);
            var bytes=Convert.FromBase64String(imageContent);
            var fileInfo = new FileInfo(filePath);

            using(var fs = fileInfo.Create())
            {
                fs.Write(bytes, 0, bytes.Length);
            }

            this.File = filePath;
        }

        private string GetFilePath(string fileName)
        {
            return System.IO.Path.Combine(GetTempDir(), fileName);
        }

        private string GetTempDir()
        {
            if (_tempDir == null)
            {
                var dir1 = System.Diagnostics.Process.GetCurrentProcess().ProcessName.Replace(".", "_");
                var tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), dir1, "cgImages");

                // Reset directory
                if (System.IO.Directory.Exists(tempDir))
                {
                    System.IO.Directory.Delete(tempDir, true);
                    System.Threading.Thread.Sleep(500);
                }

                System.IO.Directory.CreateDirectory(tempDir);
                _tempDir = tempDir;
            }
            return _tempDir;
        }
        protected override void ToXElementInternal(XElement element)
        {
            base.ToXElementInternal(element);
            XElement child = CreateImageElement();
            element.Add(child);
        }

        private XElement CreateImageElement()
        {
            XElement elem = new XElement("image");
            elem.Add(new XAttribute("extension", System.IO.Path.GetExtension(this.File)));
            XText textNode = new XText(EncodeImageContent());
            elem.Add(textNode);
            return elem;
        }

        private string EncodeImageContent()
        {
            var fileInfo = new FileInfo(this.File);
            byte[] buffer = new byte[fileInfo.Length];
            using (var fs = fileInfo.OpenRead())
            {
                fs.Read(buffer, 0, buffer.Length);
            }

            return Convert.ToBase64String(buffer, Base64FormattingOptions.InsertLineBreaks);
        }
    }
}
