using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FCSPlayout.WPFApp
{
    public class RendererDeviceComboBox:DeviceComboBoxBase
    {
        // Using a DependencyProperty as the backing store for LiveDeviceType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RendererDeviceTypeProperty =
            DependencyProperty.Register("RendererDeviceType", typeof(RendererDeviceType), typeof(RendererDeviceComboBox), 
            new FrameworkPropertyMetadata(RendererDeviceType.None, OnRendererDeviceTypePropertyChanged));

        private static void OnRendererDeviceTypePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((RendererDeviceComboBox)dpObj).OnRendererDeviceTypeChanged((RendererDeviceType)e.OldValue, (RendererDeviceType)e.NewValue);
        }

        private void OnRendererDeviceTypeChanged(RendererDeviceType oldValue, RendererDeviceType newValue)
        {
            base.DeviceType = GetDeviceType(this.RendererDeviceType);
        }

        private string GetDeviceType(RendererDeviceType liveDeviceType)
        {
            var descriptionAttr = GetCustomAttribute<DescriptionAttribute>(liveDeviceType);
            return descriptionAttr == null ? null : descriptionAttr.Description;
        }

        public RendererDeviceType RendererDeviceType
        {
            get { return (RendererDeviceType)GetValue(RendererDeviceTypeProperty); }
            set { SetValue(RendererDeviceTypeProperty, value); }
        }

        private static TAttribute GetCustomAttribute<TAttribute>(Enum @enum) where TAttribute : Attribute
        {
            TAttribute result = null;
            Type attributeType = typeof(TAttribute);
            FieldInfo field = @enum.GetType().GetFields()
                .Where(f => !f.IsSpecialName && f.IsLiteral)
                .SingleOrDefault(f => object.Equals(f.GetValue(null), @enum));
            if (field != null)
            {
                result = field.GetCustomAttribute<TAttribute>();
            }
            return result;
        }
    }

    public enum RendererDeviceType
    {
        None,
        [Description("renderer")]
        Renderer,
        [Description("renderer::line-out")]
        RendererLineOut,
        [Description("renderer::keying")]
        RendererKeying,
        [Description("renderer::line-in")]
        RendererLineIn
    }

    /*
     "video" - currently set video device 
"video::line-in" - currently set input line of video device 
"audio" - currently set audio device 
"audio::line-in" - currently set input line of audio device 

"renderer" - currently set renderer 
"renderer::line-out" - currently set renderer output line 
"renderer::keying" - currently set keying state value 
"renderer::line-in" - currently set renderer input like (for keying) 
     */
}
