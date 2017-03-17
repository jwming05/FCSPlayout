using FCSPlayout.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// LogView.xaml 的交互逻辑
    /// </summary>
    public partial class LogView : UserControl,ILog
    {
        private readonly MessageQueuedLog _innerLog;
        private FileInfo _file;
        private StreamWriter _writer;

        public LogView()
        {
            InitializeComponent();
            _innerLog = new MessageQueuedLog();
            _file = new FileInfo(DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");

            if (_file.Exists)
            {
                 _writer=new StreamWriter(_file.FullName, true, Encoding.UTF8);
            }
            else
            {
                _writer = new StreamWriter(_file.FullName,false, Encoding.UTF8);
            }
            GlobalEventAggregator.Instance.ApplicationExit += Instance_ApplicationExit;
        }

        private void Instance_ApplicationExit(object sender, EventArgs e)
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
            //throw new NotImplementedException();
        }

        public void Log(string message)
        {
            _innerLog.Log(message);
        }

        public void OnTimer()
        {
            var message = _innerLog.Take();
            if (message != null)
            {
                this.messageListBox.Items.Add(message);

                if (_writer != null)
                {
                    //_writer.WriteLineAsync()
                    _writer.WriteLine(message);
                }
            }
        }
    }
}
