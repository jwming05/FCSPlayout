using System;
using System.Collections.Generic;
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
using Prism.Interactivity.InteractionRequest;

namespace FCSPlayout.WPFApp
{
    /// <summary>
    /// PreviewControlWrapper.xaml 的交互逻辑
    /// </summary>
    public partial class PreviewControlWrapper : UserControl, IInteractionRequestAware
    {
        private PreviewRequestConfirmation _confirmation;

        public PreviewControlWrapper()
        {
            InitializeComponent();
        }

        public Action FinishInteraction
        {
            get;set;
        }

        public INotification Notification
        {
            get { return _confirmation; }
            set
            {
                _confirmation = (PreviewRequestConfirmation)value;
                if (_confirmation != null)
                {
                    this.playControl.Preview(_confirmation.PlayableItem);
                }
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _confirmation.Confirmed = true;
            this.playControl.Apply();
            this.FinishInteraction();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _confirmation.Confirmed = false;

            this.FinishInteraction();
        }
    }
}
