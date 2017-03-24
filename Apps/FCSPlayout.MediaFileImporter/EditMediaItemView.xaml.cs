
using FCSPlayout.Domain;
using FCSPlayout.Entities;

using FCSPlayout.WPF.Core;
//using FCSPlayout.WPF.Core;
using Prism.Interactivity.InteractionRequest;
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

namespace FCSPlayout.MediaFileImporter
{
    /// <summary>
    /// EditMediaItemView.xaml 的交互逻辑
    /// </summary>
    public partial class EditMediaItemView : UserControl, Prism.Interactivity.InteractionRequest.IInteractionRequestAware
    {
        private EditMediaItemConfirmation _notification;

        public INotification Notification
        {
            get { return _notification; }
            set
            {

                _notification = value as EditMediaItemConfirmation;
                if (_notification != null)
                {
                    //this.playerControl.Init(((FileMediaSource)_notification.Source).Path,
                    //    _notification.PlayRange);
                    //var src = (FileMediaSource)_notification.Source;
                    this.playerControl.Init(_notification.FilePath, _notification.PlayRange, PlayoutRepository.GetMPlaylistSettings());

                    //mw1.playerControl.Init(_notification.FilePath, _notification.PlayRange, PlayoutRepository.GetMPlaylistSettings());

                }
            }
        }
     
     
        public Action FinishInteraction
        {
            get; set;
        }

        
        public EditMediaItemView()
        {
            InitializeComponent();
          
            ev = this;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var confrimation = this.Notification as EditMediaItemConfirmation;
            if (confrimation != null)
            {
                confrimation.Confirmed = true;
                confrimation.PlayRange =this.playerControl.PlayRange;// mw1.playerControl.PlayRange;// 

                
            }
            this.FinishInteraction();
        






        }

     



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.FinishInteraction();
        }

        //public EditMediaItemView mw1
        //{
        //    get
        //    {
        //        return MainWindow.mw.playControl4;
        //    }
        //    set
        //    {
        //        MainWindow.mw.playControl4 = value;

        //        //_playRange = _playRange.ModifyByStopPosition(value);
        //    }
        //}

        public object MediaItemView { get; private set; }

        //public EditMediaItemView ev1 {

        //    get {
        //        return ev;
        //    }
        //    set {
        //        ev1 = value;
        //    }

        //}

        public static EditMediaItemView ev;
    }
}
