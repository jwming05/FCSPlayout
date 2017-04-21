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
    public class LiveDeviceComboBox:DeviceComboBoxBase
    {
        // Using a DependencyProperty as the backing store for LiveDeviceType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LiveDeviceTypeProperty =
            DependencyProperty.Register("LiveDeviceType", typeof(LiveDeviceType), typeof(LiveDeviceComboBox), 
            new FrameworkPropertyMetadata(LiveDeviceType.None, OnLiveDeviceTypePropertyChanged));

        private static void OnLiveDeviceTypePropertyChanged(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            ((LiveDeviceComboBox)dpObj).OnLiveDeviceTypeChanged((LiveDeviceType)e.OldValue, (LiveDeviceType)e.NewValue);
        }

        private void OnLiveDeviceTypeChanged(LiveDeviceType oldValue, LiveDeviceType newValue)
        {
            base.DeviceType = GetDeviceType(this.LiveDeviceType);
        }

        private string GetDeviceType(LiveDeviceType liveDeviceType)
        {
            var descriptionAttr = GetCustomAttribute<DescriptionAttribute>(liveDeviceType);
            return descriptionAttr == null ? null : descriptionAttr.Description;
        }

        public LiveDeviceType LiveDeviceType
        {
            get { return (LiveDeviceType)GetValue(LiveDeviceTypeProperty); }
            set { SetValue(LiveDeviceTypeProperty, value); }
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

    public enum LiveDeviceType
    {
        None,
        [Description("video")]
        Video,
        [Description("audio")]
        Audio,
        [Description("video::line-in")]
        VideoLineIn,
        [Description("audio::line-in")]
        AudioLineIn
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
