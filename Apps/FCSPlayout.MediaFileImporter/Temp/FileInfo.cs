using MPLATFORMLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCSPlayout.MediaFileImporter
{
    public static class FileInfoSingleton
    {
        private static FileInfo _instance = null;

        public static FileInfo Instance
        {
            get
            {
                if (FileInfoSingleton._instance == null)
                {
                    FileInfoSingleton._instance = new FileInfo();
                }
                return FileInfoSingleton._instance;
            }
        }
    }

    public class FileInfo
    {
        private MPlaylist pList = null;

        public FileInfo()
        {
            this.pList = new MPlaylistClass();
        }

        public bool GetMediaInfo(string sFilePath, out System.Collections.Generic.Dictionary<string, string> mediaInfos, out string sInform)
        {
            mediaInfos = new System.Collections.Generic.Dictionary<string, string>();
            sInform = string.Empty;
            bool result = false;

            if (!string.IsNullOrEmpty(sFilePath) && this.pList != null)
            {
                try
                {
                    int index = 0;
                    MItem mItem = null;
                    this.pList.PlaylistAdd(null, sFilePath, string.Empty, ref index, out mItem);
                    if (mItem != null)
                    {
                        MItemClass mItemClass = (MItemClass)mItem;
                        if (mItemClass != null)
                        {
                            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                            result = FillInfoArray(string.Empty, mItemClass, mediaInfos, stringBuilder);
                            sInform = stringBuilder.ToString();
                        }
                    }
                }
                catch
                {
                    //mediaInfos = new System.Collections.Generic.Dictionary<string, string>();
                }
                finally
                {
                    this.pList.PlaylistRemoveByIndex(0, -1);
                }
            }
            return result;
        }

        public int GetPropertyIntValue(System.Collections.Generic.Dictionary<string, string> info, string sName)
        {
            int value = 0;
            if (info != null && !string.IsNullOrEmpty(sName) && info.ContainsKey(sName))
            {
                if (!int.TryParse(info[sName], out value))
                {
                    value = 0;
                }
            }
            return value;
        }

        public double GetPropertyDoubleValue(System.Collections.Generic.Dictionary<string, string> info, string sName)
        {
            double value = 0.0;
            if (info != null && !string.IsNullOrEmpty(sName) && info.ContainsKey(sName))
            {
                if (!double.TryParse(info[sName], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
                {
                    value = 0.0;
                }
            }
            return value;
        }

        public string GetPropertyValue(System.Collections.Generic.Dictionary<string, string> info, string sName)
        {
            string result;
            if (info == null || string.IsNullOrEmpty(sName) || !info.ContainsKey(sName))
            {
                result = string.Empty;
            }
            else
            {
                result = info[sName];
            }
            return result;
        }

        private static bool FillInfoArray(string sNode, IMProps mpProps, System.Collections.Generic.Dictionary<string, string> info, System.Text.StringBuilder sInform)
        {
            //if (mpProps == null || info == null)
            //{
            //    return false;
            //}

            int propsCount = 0;
            try
            {
                mpProps.PropsGetCount(sNode, out propsCount);
            }
            catch
            {
                propsCount = 0;
            }

            if (propsCount <= 0)
            {
                return false;
            }

            string propName = string.Empty;
            string propValue = string.Empty;

            for (int i = 0; i < propsCount; i++)
            {
                int num2 = 0;
                mpProps.PropsGetByIndex(sNode, i, out propName, out propValue, out num2);
                if (string.Compare(propName, "file") == 0)
                {
                    FillInfoArray("file::info", mpProps, info, sInform);
                }

                if (string.IsNullOrEmpty(propValue))
                {
                    string newNode = string.IsNullOrEmpty(sNode) ? propName : (sNode + "::" + propName);
                    if (!FillInfoArray(newNode, mpProps, info, sInform))
                    {//在新节点下没有属性（或无法获取属性）时执行。
                        FillInfo(sNode, info, sInform, propName, propValue);
                    }
                }
                else
                {
                    string[] array = new string[]
                            {
                                    propValue
                            };

                    if (propValue.Contains("="))
                    {
                        array = propValue.Split(new char[]
                        {
                                        ' '
                        }, System.StringSplitOptions.RemoveEmptyEntries);
                    }

                    for (int j = 0; j < array.Length; j++)
                    {
                        string nameValuePair = array[j];

                        if (!string.IsNullOrEmpty(nameValuePair))
                        {
                            string[] nameValueArray = nameValuePair.Split(new char[]
                            {
                                                '='
                            }, System.StringSplitOptions.RemoveEmptyEntries);

                            if (nameValueArray != null)
                            {
                                string key = propName;
                                string value = string.Empty;

                                if (nameValueArray.Length == 1)
                                {
                                    value = nameValueArray[0];
                                }
                                else if (nameValueArray.Length == 2)
                                {
                                    string name = nameValueArray[0];
                                    if (string.Compare(name, propName) != 0)
                                    {
                                        name = propName + "::" + name;
                                    }
                                    key = name;
                                    value = nameValueArray[1];
                                }

                                FillInfo(sNode, info, sInform, key, value);
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static void FillInfo(string sNode, Dictionary<string, string> info, StringBuilder sInform, string key, string value)
        {
            if (!string.IsNullOrEmpty(sNode))
            {
                key = sNode + "::" + key;
            }
            if (!info.ContainsKey(key))
            {
                info.Add(key, string.Empty);
            }
            info[key] = value;
            if (key.StartsWith("file::info::"))
            {
                sInform.Append(key.Replace("file::info::", ""))
                    .Append("\t")
                    .AppendLine(value);
            }
        }
    }
}
