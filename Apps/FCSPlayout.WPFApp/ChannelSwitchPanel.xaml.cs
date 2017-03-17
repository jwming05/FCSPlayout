using FCSPlayout.Entities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// ChannelSwitchPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelSwitchPanel : UserControl
    {
        private IEnumerable<ChannelInfo> _channels;
        private List<RadioButton> _radioButtons = new List<RadioButton>();

        public ChannelSwitchPanel()
        {
            InitializeComponent();
            _channels= ChannelSwitcher.Instance.Channels;

            foreach(var ch in _channels)
            {
                var r = new RadioButton();
                var isChecked = r.IsChecked ?? false;
                // 依据IsChecked属性值选择模板
                r.Template = Resources[isChecked ? "Checked" : "UnChecked"] as ControlTemplate;
                if (ch.Special)
                {
                    r.Content = "文件输出"; // ch.Title;
                }
                else
                {
                    r.Content = ch.Title;
                }
                r.Tag = ChannelSwitcher.Instance.GetInputInfo(ch); //.Id;
                r.IsEnabled = r.Tag != null; // ChannelSwitcher.Instance.HasOutput(ch);
                r.Click += OnButtonClick;
                this.rootPanel.Children.Add(r);

                _radioButtons.Add(r);
               
            }

            ChannelSwitcher.Instance.CurrentProgramChanged += OnCurrentProgramChanged;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                BMDSwitcherInputInfo info = rb.Tag as BMDSwitcherInputInfo;
                if (info != null)
                {
                    ChannelSwitcher.Instance.Switch(info.Name);

                    //if (info.Value == curProgram.value)
                    //{
                    //    r.IsChecked = true;
                    //    break;
                    //}
                }
            }
            //throw new NotImplementedException();
        }

        private void OnCurrentProgramChanged(object sender, EventArgs e)
        {
            var curProgram = ChannelSwitcher.Instance.CurrentProgram;
            foreach(var r in _radioButtons)
            {
                BMDSwitcherInputInfo info = r.Tag as BMDSwitcherInputInfo;
                if (info != null)
                {
                    if (info.Value == curProgram.value)
                    {
                        r.IsChecked = true;
                        break;
                    }
                }
            }
        }
    }
}
