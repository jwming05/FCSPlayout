using System;
using System.Windows.Data;
using System.Windows.Media;

namespace FCSPlayout.WPFApp.Views
{
    [ValueConversion(typeof(int), typeof(Brushes))]
    //[ValueConversion(typeof(int), typeof(Brushes))]
    ////IValueConverter StyleSelector
    //class BGConverter : IValueConverter 
    //{
    //    public Style Style1 { get; set; }

    //    public Style Style2 { get; set; }
    //    public Style Style3 { get; set; }
    //    public Style Style4 { get; set; }

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        int surplus = (int)value;
    //        if (surplus < 10)
    //        {
    //            return Brushes.Yellow;
    //        }
    //        else if (surplus > 100)
    //        {
    //            return Brushes.Yellow;
    //        }
    //        else
    //        {
    //            return Brushes.White;
    //        }
    //    }

    //}


    ////public override Style SelectStyle(object item, System.Windows.DependencyObject container)

    ////    {



    ////        var surplus = (BindablePlayItem)item;

    ////        //BindingExpression b = ((DataGridRow)container).GetBindingExpression();
    ////        //b.UpdateTarget();


    ////        if (surplus.ItemStatus == PlayItemStatus.None)
    ////        {
    ////            //[Authorize(Roles = "Admin")]

    ////    Style1 = (Style)((DataGridRow)container).FindResource("style1");

    ////            //Brush br = new SolidColorBrush(Color.FromRgb(59, 59, 59));
    ////            return Style1;

    ////        }
    ////        if (surplus.ItemStatus == PlayItemStatus.Played)
    ////        {

    ////            //Brush br = new SolidColorBrush(Color.FromRgb(40, 40, 40));
    ////            Style2 = (Style)((DataGridRow)container).FindResource("style2");
    ////            return Style2;
    ////        }
    ////        else if (surplus.ItemStatus == PlayItemStatus.Playing)
    ////        {
    ////            //Brush br = new SolidColorBrush(Color.FromRgb(197, 31, 31));
    ////            Style3 = (Style)((DataGridRow)container).FindResource("style3");
    ////            return Style3;
    ////        }
    ////        else
    ////        {
    ////            Style4 = (Style)((DataGridRow)container).FindResource("style4");
    ////            return Style4;
    ////        }
    ////    }








    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {

    //        return null;
    //    }

    class BGConverter : IValueConverter
    {
   
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
               var surplus =(PlayItemStatus)value;
     
            if (surplus==PlayItemStatus.None)
            {
                Brush br = new SolidColorBrush(Color.FromRgb(59, 59, 59));
                return br;
            }
            else if (surplus==PlayItemStatus.Playing)
            {
                Brush br = new SolidColorBrush(Color.FromRgb(197, 31, 31));
                
                return br;
            }
            else if (surplus==PlayItemStatus.Unloaded) {



                //Brush br = new SolidColorBrush(Color.FromRgb(40, 40, 40));
                Brush br = new SolidColorBrush(Color.FromRgb(112, 108, 108));
                return br;
            }
            //else if (surplus == PlayItemStatus.Next)
            //{



            //    return Brushes.Green;
            //}
            else
            {
                return Brushes.Green;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }
}



























