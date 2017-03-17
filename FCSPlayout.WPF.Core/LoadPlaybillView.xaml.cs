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
using FCSPlayout.WPF.Core;

namespace FCSPlayout.WPF.Core
{
    /// <summary>
    /// LoadPlaybillView.xaml 的交互逻辑
    /// </summary>
    public partial class LoadPlaybillView : UserControl,Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        private LoadPlaybillConfirmation _confirmation;
        public LoadPlaybillView()
        {
            InitializeComponent();
        }

        public Action FinishInteraction
        {
            get;set;
        }

        public INotification Notification
        {
            get
            {
                return _confirmation;
            }

            set
            {
                _confirmation = (LoadPlaybillConfirmation)value;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            _confirmation.Confirmed = true;

            if (this.FinishInteraction != null)
            {
                this.FinishInteraction();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _confirmation.Confirmed = false;
            if (this.FinishInteraction != null)
            {
                this.FinishInteraction();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.btnOk.IsEnabled = this.dgPlaybill.SelectedItem != null;
        }
    }
}
