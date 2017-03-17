using FCSPlayout.AppInfrastructure;
using FCSPlayout.Domain;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : WPFApplicationBase
    {
        public static readonly string Name = "入库系统";

        private static WPFApplicationConfiguration CreateAppConfiguration()
        {
            return new WPFApplicationConfiguration
            {
                RequireGarbageCollection=true,
            };
        }

        public App():base(CreateAppConfiguration())
        {

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            this.Initialize();
            base.OnStartup(e);

            //var bootstrapper = new Bootstrapper();
            //bootstrapper.Run();
        }

        private void Initialize()
        {
            MLLicenseLib.MLLicenseManager.Instance.Timer();
            CheckMediaLooks();

            //FCSPlayout.Common.AppSettings.Instance.MediaFileDirectory = ConfigurationManager.AppSettings["MediaFileDirectory"];

            MediaFileDurationGetter.Current = new MLMediaFileDurationGetter();
            MediaFileService.DestinationStreamCreator = FileSystemDestinationStreamCreator.Instance;

            //LocalSettings.Instance.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            //Common.GlobalEventAggregator.Instance.RaiseApplicationExit();
        }

        private static void CheckMediaLooks()
        {
            RegistryKey root = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform");
            if (root == null)
            {
                root = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\MediaPlayer\Player\Schemes\mplatform");
            }

            if (root != null)
            {
                using (root)
                {
                    var valueNames = root.GetValueNames();
                    string name = "Runtime";
                    if (valueNames.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var value=root.GetValue(name);
                        //var t = value.GetType();

                        if (value ==null || !object.Equals(value, 1))
                        {
                            root.SetValue(name, 1, RegistryValueKind.DWord);
                            root.Flush();
                        }
                    }
                    else
                    {
                        root.SetValue(name, 1, RegistryValueKind.DWord);
                        root.Flush();
                    }
                }
            }
        }
    }

    public class FileSystemDestinationStreamCreator : IDestinationStreamCreator
    {
        private static readonly FileSystemDestinationStreamCreator _instance = new FileSystemDestinationStreamCreator();

        public static FileSystemDestinationStreamCreator Instance
        {
            get
            {
                return _instance;
            }
        }

        private FileSystemDestinationStreamCreator()
        {
        }

        public Stream Create(string destFileName, MediaFileStorage fileStorage)
        {
            return new FileStream(MediaFilePathResolver.Current.Resolve(destFileName, fileStorage),
                FileMode.CreateNew, FileAccess.Write, FileShare.None);
        }
    }
}
