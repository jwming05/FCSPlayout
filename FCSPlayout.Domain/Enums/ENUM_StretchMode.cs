using System.ComponentModel;

namespace FCSPlayout.Domain
{
    public enum ENUM_StretchMode
    {
        Default = -1,
        //[LongDescription("Stretch")]
        [Description("Stretch")]
        Stretch,
        //[LongDescription("Crop")]
        [Description("Crop")]
        Crop,
        //[LongDescription("Preserve Aspect Ratio")]
        [Description("Preserve AR")]
        PreserveAspectRatio,
        //[LongDescription("Preserve Aspect Ratio No Letterbox")]
        [Description("Preserve AR Full")]
        PreserveAspectRatioNoLetterbox
    }
}
